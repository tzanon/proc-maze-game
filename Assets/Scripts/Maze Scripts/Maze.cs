using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

/**
 * GENERAL C# "CONVENTIONS:"
 * PascalCase for public member variables (string MyName = "James")
 * camelCase for local variables (string myName = "James")
 * _leadingUnderscore for private member variables (string _myName = "James")
 * 
 */

// a class to store information about a maze object
public class Maze : MonoBehaviour
{
    public TileScript TileTemplate;
    private TileScript[,] _grid;
    [HideInInspector]
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

    [HideInInspector]
    public const int minHeight = 6, minWidth = 6,
        maxHeight = 16, maxWidth = 32;

    public bool withDelay;

    // for constructing the grid in the scene
    //private const float _startX = -8.25f;
    //private const float _startY = -4f;
    //private Vector3 incr;
    //private Vector2 position;

    #region generation variables

    public Text stackCount;
    public Text setCount;

    private const float delay = 0.025f; // delay for "animation"
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
        withDelay = true;
    }

    #region general utility methods

    public void MakeSmallMaze()
    {
        MakeBlankGrid(minHeight, minWidth);

        //StartGeneration();
    }

    public void MakeMedMaze()
    {
        MakeBlankGrid(maxHeight / 2, maxWidth / 2);

        StartGeneration();
    }

    public void MakeLargeMaze()
    {
        MakeBlankGrid(maxHeight, maxWidth);

        StartGeneration();
    }

    // makes a new grid from scratch and generates a maze in it
    public void MakeRandomMaze()
    {
        int randHeight = UnityEngine.Random.Range(minHeight, maxHeight);
        int randWidth = UnityEngine.Random.Range(minWidth, maxWidth);

        MakeBlankGrid(randHeight, randWidth);

        StartGeneration();
    }

    // creates a blank grid in preparation for generation or editing
    public void MakeBlankGrid(int gridHeight, int gridWidth)
    {
        Debug.Log("making the codes...");
        string[,] gridCodes = new string[gridHeight, gridWidth];

        for (int i = 0; i < gridHeight; i++)
        {
            for (int j = 0; j < gridWidth; j++)
            {
                gridCodes[i, j] = "1111";
            }
        }

        MakeGridFromCodes(gridCodes);
        MarkAllTilesUnvisited();
        PrepareForGeneration();
    }

    // "fills" the grid by instantiating each tile with properties specified
    public void MakeGridFromCodes(string[,] gridCodes)
    {
        DestroyGrid(); // previous grid's tiles will be removed
        Grid = new TileScript[gridCodes.GetLength(0), gridCodes.GetLength(1)];
        Debug.Log("new height is " + Height);
        Debug.Log("new width is " + Width);

        Vector3 position = TileTemplate.StartingPosition;
        Vector3 xIncr = TileTemplate.HorizontalSpawnIncrement;
        Vector3 yIncr = TileTemplate.VerticalSpawnIncrement;

        for (int i = 0; i < Height; i++)
        {
            for (int j = 0; j < Width; j++)
            {
                TileScript newTile = Grid[i, j] = Instantiate(TileTemplate, position, Quaternion.identity) as TileScript;
                newTile.X = j;
                newTile.Y = i;
                newTile.SetWallsFromCode(gridCodes[i, j]);

                position += xIncr;
            }
            position.x = TileTemplate.StartingPosition.x;
            position += yIncr;
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
                if (Grid[i, j] != null) Destroy(Grid[i, j].gameObject);
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
            if (gridTile.gameObject == tile.gameObject) return true;

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
        return (!isGenerating && GridContains(startTile) && GridContains(endTile));
    }

    public void DisplayCoordinates()
    {
        foreach (TileScript tile in Grid)
        {
            Debug.Log("(" + tile.X + ", " + tile.Y + ")");
        }
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

    // starts the generation process
    public void StartGeneration()
    {
        if (!isGenerating) StartCoroutine(GenerateMaze());
    }

    // generates a maze using randomized depth-first search with backtracking
    // ***DO NOT MODIFY THIS METHOD***
    private IEnumerator GenerateMaze()
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

        PrepareForGeneration();

        isGenerating = true;

        int initY = 0;
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


        startTile = Grid[initY, initX];
        startTile.MakeStartTile();

        int endY = Grid.GetLength(0) - 1;
        int endX = UnityEngine.Random.Range(0, Grid.GetLength(1));
        endTile = Grid[endY, endX];
        endTile.MakeEndTile();
        
        // change the colour of the start and end tiles

        isGenerating = false;
    }

    // gets all of the unvisited neighbour tiles of the given tile
    public List<TileScript> GetUnvisitedNeighbours(TileScript currentTile)
    {
        List<TileScript> unvisitedNeighbours = new List<TileScript>();
        int x = currentTile.X;
        int y = currentTile.Y;

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

    // ***this should be in the scripts for the file loading screens
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

                // set each tile's walls
                string[,] gridCodes = new string[gridHeight, gridWidth];
                for (int i = 0; i < gridHeight; i++)
                {
                    for (int j = 0; j < gridWidth; j++)
                    {
                        gridCodes[i, j] = mazeFile.ReadLine();
                    }
                }
                this.MakeGridFromCodes(gridCodes);

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
