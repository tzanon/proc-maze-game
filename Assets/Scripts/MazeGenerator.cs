using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class MazeGenerator : MonoBehaviour
{
    public Text stackCount;
    public Text setCount;

    public float delay;

    [Range(1, 24)]
    public int gridWidth = 21;

    [Range(1, 12)]
    public int gridHeight = 11;
    public TileScript Tile;

    private bool isGenerating;

    private const float startX = -8.25f;
    private const float startY = 4f;
    private const float incr = 0.535f;
    private float currX, currY;
    private Vector2 position;

    private TileScript[,] grid;
    private Stack<TileScript> tileStack = new Stack<TileScript>();
    private HashSet<TileScript> unvisitedTiles = new HashSet<TileScript>();

    void Start()
    {
        currX = startX;
        currY = startY;
        position = new Vector2(currX, currY);

        grid = new TileScript[gridHeight, gridWidth];
        CreateGrid();

        UpdateStackCount();
        UpdateUnvisitedTiles();

        isGenerating = false;
    }

    void UpdateStackCount()
    {
        stackCount.text = "Tiles on stack: " + tileStack.Count;
    }

    void UpdateUnvisitedTiles()
    {
        setCount.text = "Unvisited tiles: " + unvisitedTiles.Count;
    }

    void CreateGrid()
    {
        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                TileScript newTile = grid[i, j] = Instantiate(Tile, position, Quaternion.identity) as TileScript;
                newTile.x = j;
                newTile.y = i;
                unvisitedTiles.Add(newTile);
                UpdateUnvisitedTiles();
                position.x += incr;
            }
            position.x = startX;
            position.y -= incr;
        }
    }
    
    public void ResetGrid()
    {
        tileStack.Clear();
        UpdateStackCount();

        unvisitedTiles.Clear();
        UpdateUnvisitedTiles();

        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                TileScript tile = grid[i, j];
                tile.ResetTile();
                unvisitedTiles.Add(tile);
                UpdateUnvisitedTiles();
            }
            
        }

    }

    public void GenerateMazeWithDelay()
    {
        
        if (!isGenerating) StartCoroutine(GenerateMaze(true));
    }

    public void GenerateMazeWithoutDelay()
    {
        if (!isGenerating) StartCoroutine(GenerateMaze(false));
    }

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

        int initY = grid.GetLength(0) - 1;
        int initX = Random.Range(0, grid.GetLength(1));

        TileScript currentTile = grid[initY, initX];
        unvisitedTiles.Remove(currentTile);
        UpdateUnvisitedTiles();
        currentTile.Visit();

        while (unvisitedTiles.Count > 0)
        {
            if (withDelay) yield return new WaitForSeconds(delay);

            List<TileScript> unvisitedNeighbours = GetUnvisitedNeighbours(currentTile);

            if (unvisitedNeighbours.Count > 0)
            {
                int neighbourNum = Random.Range(0, unvisitedNeighbours.Count);

                TileScript neighbourTile = unvisitedNeighbours[neighbourNum];
                tileStack.Push(currentTile);
                UpdateStackCount();
                currentTile.OnStack();

                currentTile.DeleteWallsBetweenTiles(neighbourTile);

                
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
                int rand = Random.Range(0, unvisited.GetLength(0));

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


    List<TileScript> GetUnvisitedNeighbours(TileScript currentTile)
    {
        List<TileScript> unvisitedNeighbours = new List<TileScript>();
        int x = currentTile.x;
        int y = currentTile.y;

        if (InYBounds(y) && InXBounds(x+1) && unvisitedTiles.Contains(grid[y, x+1])) unvisitedNeighbours.Add(grid[y, x+1]);
        if (InYBounds(y) && InXBounds(x-1) && unvisitedTiles.Contains(grid[y, x-1])) unvisitedNeighbours.Add(grid[y, x-1]);
        if (InYBounds(y+1) && InXBounds(x) && unvisitedTiles.Contains(grid[y+1, x])) unvisitedNeighbours.Add(grid[y+1, x]);
        if (InYBounds(y-1) && InXBounds(x) && unvisitedTiles.Contains(grid[y-1, x])) unvisitedNeighbours.Add(grid[y-1, x]);

        return unvisitedNeighbours;
    }

    bool InXBounds(int x)
    {
        return
            (x >= 0 &&
            x < grid.GetLength(1));
    }

    bool InYBounds(int y)
    {
        return
            (y >= 0 &&
            y < grid.GetLength(0));
    }

}
