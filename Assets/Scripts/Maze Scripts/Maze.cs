using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

// a class to store information about a maze object
public class Maze : MonoBehaviour
{

    private TileScript[,] _grid;
    public TileScript startTile, endTile;

    public int Height
    {
        get
        {
            if (_grid != null) return _grid.GetLength(0);
            else return -1;
        }
    }
    public int Width
    {
        get
        {
            if (_grid != null) return _grid.GetLength(1);
            else return -1;
        }
    }
    public TileScript[,] Grid
    {
        get { return _grid; }
        private set { _grid = value; }
    }

    // for constructing the grid in the scene
    private const float startX = -8.25f;
    private const float startY = 4f;
    private const float incr = 0.535f;
    private Vector2 position;

    #region generation variables

    public Text stackCount;
    public Text setCount;

    private const float delay = 0.05f; // delay for "animation"
    private bool isGenerating;
    private Stack<TileScript> tileStack = new Stack<TileScript>();
    private HashSet<TileScript> unvisitedTiles = new HashSet<TileScript>();

    #endregion

    void Start()
    {
        // maze generation things
        UpdateStackCount();
        UpdateUnvisitedTiles();
        isGenerating = false;
    }

    #region general utility methods

    // initializes the grid with height h and width w
    public void InitializeMaze(int h, int w)
    {
        Grid = new TileScript[h, w];
    }

    public void MakeNewGrid(int h, int w)
    {
        if (GridExists()) DestroyGrid();

        Grid = new TileScript[h, w];

        CreateBlankGrid();
    }

    // creates a blank grid in preparation for generation or editing
    public void CreateBlankGrid()
    {
        string[,] gridCodes = new string[Height, Width];

        for (int i = 0; i < Height; i++)
        {
            for (int j = 0; j < Width; j++)
            {
                gridCodes[i, j] = "1111";
            }
        }

        CreateGridFromCodes(gridCodes);
        MarkAllTilesUnvisited();
        PrepareForGeneration();
    }

    public void CreateGridFromCodes(string[,] gridCodes)
    {
        // grid must exist and code array must have same dimensions as the grid
        if (Grid == null || gridCodes.GetLength(0) != Height || gridCodes.GetLength(1) != Width) return;

        position = new Vector2(startX, startY);

        for (int i = 0; i < Grid.GetLength(0); i++)
        {
            for (int j = 0; j < Grid.GetLength(1); j++)
            {
                TileScript newTile = Grid[i, j] = Instantiate(Grid[i, j], position, Quaternion.identity) as TileScript;
                newTile.x = j;
                newTile.y = i;
                newTile.SetWallsFromCode(gridCodes[i, j]);

                position.x += incr;
            }
            position.x = startX;
            position.y -= incr;
        }
        // set start and end tile now
    }

    // destroys every tile in the grid
    public void DestroyGrid()
    {
        if (Grid == null) return;

        for (int i = 0; i < Grid.GetLength(0); i++)
        {
            for (int j = 0; j < Grid.GetLength(1); j++)
            {
                if (Grid[i,j] != null) Destroy(Grid[i, j].gameObject);
            }
        }

        // reset utiity data structures
        tileStack.Clear();
        unvisitedTiles.Clear();

        // update GUI display
        UpdateStackCount();
        UpdateUnvisitedTiles();

    }

    // determines if the given coordinates are within the grid's bounds
    public bool InBounds(int x, int y)
    {
        return (
            x >= 0 &&
            x < Grid.GetLength(1) &&
            y >= 0 &&
            y < Grid.GetLength(0)
            );
    }

    // checks if the given tile is in this grid
    // DEBUGGING: try this with tile.gameObject and with just tile
    public bool GridContains(TileScript tile)
    {
        if (tile == null || !GridExists()) return false;

        foreach (TileScript gridTile in Grid)
            if (gridTile == tile.gameObject) return true;

        return false;
    }

    // the grid exists only if all of its tiles have been instantiated
    // DEBUGGING: try this with tile.gameObject and with just tile
    public bool GridExists()
    {
        if (Grid == null) return false;

        foreach (TileScript tile in Grid)
            if (tile.gameObject == null) return false;

        return true;
    }

    // maze is playable only if its start and end tiles are defined
    public bool Playable()
    {
        return (GridContains(startTile) && GridContains(endTile));
    }

    #endregion


    #region maze generation methods

    // debugging GUI update functions
    private void UpdateStackCount()
    {
        if (stackCount != null)
            stackCount.text = "Tiles on stack: " + tileStack.Count;
    }
    private void UpdateUnvisitedTiles()
    {
        if (setCount != null)
            setCount.text = "Unvisited tiles: " + unvisitedTiles.Count;
    }

    // reset the grid to a blank state
    public void ResetGrid()
    {
        if (isGenerating || !GridExists()) return;

        // reset the grid's walls
        for (int i = 0; i < Grid.GetLength(0); i++)
        {
            for (int j = 0; j < Grid.GetLength(1); j++)
            {
                Grid[i, j].ResetTile();
            }
        }
        
        PrepareForGeneration();
    }

    // sets the data structures used in the generation process
    public void PrepareForGeneration()
    {
        // make maze blank

        // reset utiity data structures
        tileStack.Clear();
        unvisitedTiles.Clear();

        // mark all tiles as unvisited
        MarkAllTilesUnvisited();

        // update GUI display
        UpdateStackCount();
        UpdateUnvisitedTiles();

    }

    // marks all tiles in the grid as unvisited
    private void MarkAllTilesUnvisited()
    {
        for (int i = 0; i < Grid.GetLength(0); i++)
        {
            for (int j = 0; j < Grid.GetLength(1); j++)
            {
                TileScript tile = Grid[i, j];
                tile.UnVisit();
                unvisitedTiles.Add(tile);
            }
        }
    }

    // make a maze with an animation showing the generation process
    public void GenerateMazeWithDelay()
    {
        if (!isGenerating) StartCoroutine(GenerateMaze(true));
    }

    // make a maze with no animation
    public void GenerateMazeWithoutDelay()
    {
        if (!isGenerating) StartCoroutine(GenerateMaze(false));
    }

    // generates a maze using randomized depth-first search with backtracking
    private IEnumerator GenerateMaze(bool withDelay)
    {
        /*
         Generate Maze with Backtracking (from Wikipedia):
            1. Make the initial cell the current cell and mark it as visited
            2. While there are unvisited cells
                1. If the current cell has any neighbours which have not been visited
                    1. Choose randomly one of the unvisited neighbours
                    2. Push the current cell to the stack
                    3. Remove the wall between the current cell and the chosen cell
                    4. Make the chosen cell the current cell and mark it as visited
                2. Else if stack is not empty
                    1. Pop a cell from the stack
                    2. Make it the current cell
                3. Else
                    1. Pick a random unvisited cell, make it the current cell and mark it as visited
         */

        isGenerating = true;

        int initY = Grid.GetLength(0) - 1;
        int initX = UnityEngine.Random.Range(0, Grid.GetLength(1));

        TileScript currentTile = Grid[initY, initX];
        unvisitedTiles.Remove(currentTile);
        UpdateUnvisitedTiles();
        currentTile.Visit();

        while (unvisitedTiles.Count > 0)
        {
            if (withDelay) yield return new WaitForSeconds(delay);

            List<TileScript> unvisitedNeighbours = GetUnvisitedNeighbours(currentTile);

            if (unvisitedNeighbours.Count > 0)
            {
                int neighbourNum = UnityEngine.Random.Range(0, unvisitedNeighbours.Count);

                TileScript neighbourTile = unvisitedNeighbours[neighbourNum];
                tileStack.Push(currentTile);
                UpdateStackCount();
                currentTile.OnStack();

                currentTile.RemoveWallsBetweenTiles(neighbourTile);


                currentTile = neighbourTile;
                unvisitedTiles.Remove(currentTile);
                UpdateUnvisitedTiles();
                currentTile.Visit();
            }
            else if (tileStack.Count > 0)
            {
                currentTile.Visit();
                currentTile = tileStack.Pop();
                UpdateStackCount();
            }
            else
            {
                TileScript[] unvisited = new TileScript[unvisitedTiles.Count];
                unvisitedTiles.CopyTo(unvisited);
                int rand = UnityEngine.Random.Range(0, unvisited.GetLength(0));

                currentTile = unvisited[rand];
                unvisitedTiles.Remove(currentTile);
                UpdateUnvisitedTiles();
            }

        }

        while (tileStack.Count > 0)
        {
            if (withDelay) yield return new WaitForSeconds(delay);
            currentTile.Visit();
            currentTile = tileStack.Pop();
            UpdateStackCount();
        }

        if (withDelay) yield return new WaitForSeconds(delay);
        currentTile.Visit();

        isGenerating = false;
    }

    // gets all of the unvisited neighbour tiles of the given tile
    public List<TileScript> GetUnvisitedNeighbours(TileScript currentTile)
    {
        List<TileScript> unvisitedNeighbours = new List<TileScript>();
        int x = currentTile.x;
        int y = currentTile.y;

        if (InBounds(x + 1, y) && unvisitedTiles.Contains(Grid[y, x + 1])) unvisitedNeighbours.Add(Grid[y, x + 1]);
        if (InBounds(x - 1, y) && unvisitedTiles.Contains(Grid[y, x - 1])) unvisitedNeighbours.Add(Grid[y, x - 1]);
        if (InBounds(x, y + 1) && unvisitedTiles.Contains(Grid[y + 1, x])) unvisitedNeighbours.Add(Grid[y + 1, x]);
        if (InBounds(x, y - 1) && unvisitedTiles.Contains(Grid[y - 1, x])) unvisitedNeighbours.Add(Grid[y - 1, x]);

        return unvisitedNeighbours;
    }


    #endregion


    #region file IO methods

    //all these need work...

    public void SaveMaze(string filename)
    {

        int gridHeight = _grid.GetLength(0);
        int gridWidth = _grid.GetLength(1);

        string filePath = @"MazeFiles\" + filename + ".txt";
        StreamWriter mazeFile = new StreamWriter(filePath);

        mazeFile.WriteLine(gridHeight);
        mazeFile.WriteLine(gridWidth);

        mazeFile.WriteLine();

        for (int i = 0; i < gridHeight; i++)
        {
            for (int j = 0; j < gridWidth; j++)
            {
                string wallCode = _grid[i, j].GetWallCode();
                mazeFile.WriteLine(wallCode);
            }
        }

        mazeFile.Close();

        Debug.Log("Saving file...");
    }

    // this should be in a different script...
    public string[] GetMazeFiles(bool onlyPlayable)
    {
        string[] files = new string[10];

        /*
        string[] mazeFiles = Directory.GetFiles(@"MazeFiles\", ".txt")
            .Select(Path.GetFileName)
            .ToArray();
        */

        return files;
    }

    // creates a grid from information read from a file
    public void LoadMaze(string fname)
    {
        string filePath = @"MazeFiles\" + fname + ".txt"; // this will need to be modified

        StreamReader mazeFile;

        try
        {
            using (StreamReader mazeFileReader = new StreamReader(filePath))
            {
                Debug.Log("Loading file...");
                mazeFile = new StreamReader(filePath);

                // initialize a grid with the stored height and width
                int gridHeight = Int32.Parse(mazeFile.ReadLine());
                int gridWidth = Int32.Parse(mazeFile.ReadLine());
                this.InitializeMaze(gridHeight, gridWidth);

                // set each tile's walls
                string[,] gridCodes = new string[gridHeight, gridWidth];
                for (int i = 0; i < gridHeight; i++)
                {
                    for (int j = 0; j < gridWidth; j++)
                    {
                        gridCodes[i, j] = mazeFile.ReadLine();
                    }
                }
                this.CreateGridFromCodes(gridCodes);

                // set the start and end tile
            }
        }
        catch (IOException e)
        {
            Console.WriteLine("File not found.");
            Console.WriteLine(e.Message);
        }
    }

    #endregion

}
