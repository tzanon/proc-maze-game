using UnityEngine;

// a class to store information about a maze object
public class Maze : MonoBehaviour
{
    private TileScript[,] _grid;
    public TileScript startTile, endTile;
    
    // for constructing the grid in the scene
    private const float startX = -8.25f;
    private const float startY = 4f;
    private const float incr = 0.535f;
    private Vector2 position;

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
    }

    // sets the maze's properties
    public void InitializeMaze(int h, int w)
    {
        _grid = new TileScript[h, w];
    }

    // creates a blank canvas grid
    public void CreateBlankGrid()
    {
        if (_grid == null) return;

        position = new Vector2(startX, startY);

        for (int i = 0; i < _grid.GetLength(0); i++)
        {
            for (int j = 0; j < _grid.GetLength(1); j++)
            {
                TileScript newTile = _grid[i, j] = Instantiate(_grid[i, j], position, Quaternion.identity) as TileScript;
                newTile.x = j;
                newTile.y = i;

                position.x += incr;
            }
            position.x = startX;
            position.y -= incr;
        }
    }

    public void CreateGridFromFileInfo(string[,] gridCodes)
    {
        // code array must have same dimensions as the grid
        if (_grid == null || gridCodes.GetLength(0) != Height || gridCodes.GetLength(1) != Width) return;

        position = new Vector2(startX, startY);

        for (int i = 0; i < _grid.GetLength(0); i++)
        {
            for (int j = 0; j < _grid.GetLength(1); j++)
            {
                TileScript newTile = _grid[i, j] = Instantiate(_grid[i, j], position, Quaternion.identity) as TileScript;
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

    // resets the grid to its original state
    public void ResetGrid()
    {
        for (int i = 0; i < _grid.GetLength(0); i++)
        {
            for (int j = 0; j < _grid.GetLength(1); j++)
            {
                _grid[i, j].ResetTile();
            }
        }
    }

    // destroys every tile in the grid
    public void DestroyGrid()
    {
        for (int i = 0; i < _grid.GetLength(0); i++)
        {
            for (int j = 0; j < _grid.GetLength(1); j++)
            {
                Destroy(_grid[i, j].gameObject);
            }
        }
    }

    // determines if the given coordinates are within the grid's bounds
    public bool InBounds(int x, int y)
    {
        return (
            x >= 0 &&
            x < _grid.GetLength(1) &&
            y >= 0 &&
            y < _grid.GetLength(0)
            );
    }

    // checks if the given tile is in this grid
    public bool GridContains(TileScript tile)
    {
        for (int i = 0; i < _grid.GetLength(0); i++)
            for (int j = 0; j < _grid.GetLength(1); j++)
                if (_grid[i, j] == tile) return true;
        return false;
    }

    // maze is playable only if its start and end tiles are defined
    public bool Playable()
    {
        return (startTile.gameObject != null && endTile.gameObject != null);
    }

}
