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

    private PlayLevelManager manager;
    private Maze level;
    private GameRunner runner;
    private string filename;

    private TileScript3D[,] testGrid;

    // Use this for initialization
    void Start()
    {
        level = Instantiate(MazeTemplate) as Maze;
        //TileScript3D newTile = Instantiate(TileTemplate, Vector3.zero, Quaternion.identity) as TileScript3D;
        level.MakeBlankGrid(6, 6);
        //level.stackCount = StackSizeText;
        //level.setCount = UnvisitedTilesText;

        level.PrepareForGeneration();
        //level.StartGeneration();
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
