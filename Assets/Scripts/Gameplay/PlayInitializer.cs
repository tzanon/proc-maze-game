using UnityEngine;
using UnityEngine.UI;

public class PlayInitializer : MonoBehaviour
{

    public Maze MazeTemplate;
    public GameRunner RunnerTemplate;
    public TileScript3D TileTemplate;

    public InputField HeightSetter;
    public InputField WidthSetter;
    public Text StackSizeText;
    public Text UnvisitedTilesText;

    public Camera TopViewCam;

    private PlayLevelManager manager;
    private Maze level;
    private GameRunner runner;
    private string filename;

    private TileScript3D[,] testGrid;

    // Use this for initialization
    void Start()
    {
        level = Instantiate(MazeTemplate) as Maze;
        level.withDelay = true;

        level.MakeBlankGrid(30, 30);
        //level.stackCount = StackSizeText;
        //level.setCount = UnvisitedTilesText;

        RepositionTopViewCam();

        level.PrepareForGeneration();
        level.StartGeneration();
    }

    private void RepositionTopViewCam()
    {
        TileScript[,] grid = level.Grid;

        float camX = (grid[0, 0].transform.position.x +
            grid[0, grid.GetLength(1) - 1].transform.position.x) / 2;

        float camZ = (grid[0, 0].transform.position.z +
            grid[grid.GetLength(0) - 1, 0].transform.position.z) / 2;

        float gridSize = Mathf.Max(grid.GetLength(0), grid.GetLength(1));

        TopViewCam.transform.position = new Vector3(camX, 3 * gridSize, camZ);
        TopViewCam.orthographicSize = gridSize;
        
    }

    private void TestTiles()
    {
        TileScript3D downTile = Instantiate(TileTemplate, Vector3.zero, Quaternion.identity) as TileScript3D;
        downTile.X = 0; downTile.Y = 0;

        TileScript3D upTile = Instantiate(TileTemplate, new Vector3(0, 0, 3), Quaternion.identity) as TileScript3D;
        upTile.X = 0; upTile.Y = 1;

        TileScript3D rightTile = Instantiate(TileTemplate, new Vector3(3, 0, 0), Quaternion.identity) as TileScript3D;
        rightTile.X = 1; rightTile.Y = 0;

        TileScript3D leftTile = Instantiate(TileTemplate, new Vector3(-3, 0, 0), Quaternion.identity) as TileScript3D;
        leftTile.X = -1; leftTile.Y = 0;

        upTile.RemoveWallsBetweenTiles(downTile);
        downTile.RemoveWallsBetweenTiles(leftTile);
        downTile.RemoveWallsBetweenTiles(rightTile);
    }

    public void Generate()
    {
        if (runner != null) return;
        level.StartGeneration();
    }

    public void Reset()
    {
        if (runner != null) return;
        level.ResetGrid();
    }

    public void ToggleAnim()
    {
        level.withDelay = !level.withDelay;
    }

    public void Destroy()
    {
        if (runner != null) return;
        level.DestroyGrid();
    }
    
    public void UpdateGridDimensions()
    {
        if (runner != null) return;
        int newHeight;
        int newWidth;

        bool isHeightNumeric = int.TryParse(HeightSetter.text, out newHeight);
        bool isWidthNumeric = int.TryParse(WidthSetter.text, out newWidth);

        if (!isHeightNumeric) newHeight = Maze.minHeight;
        else if (newHeight < Maze.minHeight) newHeight = Maze.minHeight;
        else if (newHeight > Maze.maxHeight) newHeight = Maze.maxHeight;

        if (!isWidthNumeric) newWidth = Maze.minWidth;
        else if (newWidth < Maze.minWidth) newWidth = Maze.minWidth;
        else if (newWidth > Maze.maxWidth) newWidth = Maze.maxWidth;

        if (newHeight == level.Height) Debug.Log("No change in height");
        if (newWidth == level.Width) Debug.Log("No change in width");

        level.MakeBlankGrid(newHeight, newWidth);
    }

    /*
    public void PlayLevel()
    {
        if (runner != null || !level.Playable()) return;

        runner = Instantiate(RunnerTemplate) as GameRunner;
        runner.Level = level;
        runner.WinText = WinText;
    }

    public void StopPlaying()
    {
        if (runner == null) return;
        runner.EndGame();
    }
    */
}
