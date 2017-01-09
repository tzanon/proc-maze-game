using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class MazeGenerator : MonoBehaviour
{

    public Text stackCount;
    public Text setCount;
    public InputField saveFileField;
    public InputField loadFileField;
    public TileScript tile;

    private const float delay = 0.05f; // delay for "animation"
    private Maze _maze; // the maze object to be used

    public Maze Maze
    {
        get { return _maze; }
    }

    private bool isGenerating;
    private Stack<TileScript> tileStack = new Stack<TileScript>();
    private HashSet<TileScript> unvisitedTiles = new HashSet<TileScript>();

    void Start()
    {
        UpdateStackCount();
        UpdateUnvisitedTiles();

        isGenerating = false;
    }

    // debugging GUI update functions
    private void UpdateStackCount()
    {
        stackCount.text = "Tiles on stack: " + tileStack.Count;
    }
    private void UpdateUnvisitedTiles()
    {
        setCount.text = "Unvisited tiles: " + unvisitedTiles.Count;
    }

    // prepare a grid for the algorithm to work on
    public void InitializeGrid()
    {
        if (isGenerating || _maze == null) return;
        _maze.CreateBlankGrid(); // create the actual grid
        MarkTilesUnvisited();
    }

    // reset the grid to its original state
    public void ResetGrid()
    {
        if (isGenerating || _maze == null) return; // don't reset if generating a maze...

        // reset utiity data structures
        tileStack.Clear();
        unvisitedTiles.Clear();

        // reset the grid's walls
        _maze.ResetGrid();
        MarkTilesUnvisited();

        // update GUI display
        UpdateStackCount();
        UpdateUnvisitedTiles();
    }

    // destroys the grid
    public void DestroyGrid()
    {
        if (isGenerating || _maze == null) return;

        // reset utiity data structures
        tileStack.Clear();
        unvisitedTiles.Clear();

        // destroy the grid's tiles
        _maze.DestroyGrid();

        // update GUI display
        UpdateStackCount();
        UpdateUnvisitedTiles();

    }

    // marks all tiles in the grid as unvisited
    private void MarkTilesUnvisited()
    {
        TileScript[,] grid = _maze.Grid;
        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                TileScript tile = grid[i, j];
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

        TileScript[,] grid = _maze.Grid;

        int initY = grid.GetLength(0) - 1;
        int initX = UnityEngine.Random.Range(0, grid.GetLength(1));

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

        TileScript[,] grid = _maze.Grid;

        if (_maze.InBounds(x + 1, y) && unvisitedTiles.Contains(grid[y, x + 1])) unvisitedNeighbours.Add(grid[y, x + 1]);
        if (_maze.InBounds(x - 1, y) && unvisitedTiles.Contains(grid[y, x - 1])) unvisitedNeighbours.Add(grid[y, x - 1]);
        if (_maze.InBounds(x, y + 1) && unvisitedTiles.Contains(grid[y + 1, x])) unvisitedNeighbours.Add(grid[y + 1, x]);
        if (_maze.InBounds(x, y - 1) && unvisitedTiles.Contains(grid[y - 1, x])) unvisitedNeighbours.Add(grid[y - 1, x]);

        return unvisitedNeighbours;
    }

    public void Test()
    {
        /*
        DestroyGrid();
        gridHeight = 5;
        gridWidth = 5;
        grid = new TileScript[gridHeight, gridWidth];
        CreateGrid();
        */
    }

}
