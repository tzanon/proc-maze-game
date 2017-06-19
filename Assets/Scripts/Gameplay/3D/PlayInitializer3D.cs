
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PlayInitializer3D : MonoBehaviour
{
    public Maze mazeTemplate;
    public GameRunner3D runnerTemplate;
    public TileScript3D tileTemplate;
    public PlayerController3D playerTemplate;

    public CanvasRenderer mazeOptions, graphOptions;

    public Canvas userInterfaceCanvas;
    public CanvasRenderer mazeDisplay;
    public GameObject playDisplay;
    public InputField heightSetter;
    public InputField widthSetter;
    public Text stackSizeText;
    public Text unvisitedTilesText;

    public InputField saveFilenameSetter;
    public InputField loadFilenameSetter;

    public ScrollRect mazeSelector;
    private VerticalLayoutGroup selectorContent;
    public CanvasRenderer selectorCloser;
    public Button fileButton;

    [SerializeField]
    private Camera topViewCam;
    private Camera playerCamera;
    private PlayerController3D player;
    private PlayLevelManager manager;
    private Maze level;
    private GameRunner3D runner;

    private TileScript3D[,] testGrid;
    private Camera[] cameras;
    private int cameraIndex;

    private void Start()
    {
        selectorContent = mazeSelector.GetComponentInChildren<VerticalLayoutGroup>();
        saveFilenameSetter.transform.parent.gameObject.SetActive(false);
        loadFilenameSetter.transform.parent.gameObject.SetActive(false);
        mazeSelector.gameObject.SetActive(false);
        selectorCloser.gameObject.SetActive(false);

        level = Instantiate(mazeTemplate) as Maze;
        level.withDelay = true;

        /*
        player = Instantiate(PlayerTemplate, new Vector3(-12, 1, 0), Quaternion.identity) as PlayerController3D;
        playerCamera = player.GetComponent<Camera>();

        TopViewCam.enabled = true;
        playerCamera.enabled = false;
        */

        level.MakeBlankGrid(15, 15);

        level.stackCount = stackSizeText;
        level.setCount = unvisitedTilesText;
        
        this.RepositionTopViewCam();

        level.PrepareForGeneration();

        mazeDisplay.gameObject.SetActive(true);
        playDisplay.SetActive(false);
    }

    private void RepositionTopViewCam()
    {
        TileScript[,] grid = level.Grid;

        float camX = (grid[0, 0].transform.position.x +
            grid[0, grid.GetLength(1) - 1].transform.position.x) / 2;

        float camZ = (grid[0, 0].transform.position.z +
            grid[grid.GetLength(0) - 1, 0].transform.position.z) / 2;

        float gridSize = Mathf.Max(grid.GetLength(0), grid.GetLength(1));

        topViewCam.transform.position = new Vector3(camX, 3 * gridSize, camZ);
        topViewCam.transform.rotation = Quaternion.Euler(90, 0, 0);
        topViewCam.orthographicSize = gridSize;
    }

    private void TestTiles()
    {
        TileScript3D downTile = Instantiate(tileTemplate, Vector3.zero, Quaternion.identity) as TileScript3D;
        downTile.X = 0; downTile.Y = 0;

        TileScript3D upTile = Instantiate(tileTemplate, new Vector3(0, 0, 3), Quaternion.identity) as TileScript3D;
        upTile.X = 0; upTile.Y = 1;

        TileScript3D rightTile = Instantiate(tileTemplate, new Vector3(3, 0, 0), Quaternion.identity) as TileScript3D;
        rightTile.X = 1; rightTile.Y = 0;

        TileScript3D leftTile = Instantiate(tileTemplate, new Vector3(-3, 0, 0), Quaternion.identity) as TileScript3D;
        leftTile.X = -1; leftTile.Y = 0;

        upTile.RemoveWallsBetweenTiles(downTile);
        downTile.RemoveWallsBetweenTiles(leftTile);
        downTile.RemoveWallsBetweenTiles(rightTile);
    }

    public void DisplaySaveFilenameInput()
    {
        saveFilenameSetter.transform.parent.gameObject.SetActive(true);
    }

    public void CloseSaveInput()
    {
        saveFilenameSetter.transform.parent.gameObject.SetActive(false);
    }

    public void DisplayFilenameSelector()
    {
        mazeSelector.gameObject.SetActive(true);
        selectorCloser.gameObject.SetActive(true);

        string[] filepaths = level.GetMazeFiles();

        foreach (string filepath in filepaths)
        {
            string filename = filepath.Substring(17);
            Button button = Instantiate(fileButton, selectorContent.transform) as Button;
            button.onClick.AddListener(() => LoadFile(filename));
            Text buttonText = button.GetComponentInChildren<Text>();
            buttonText.text = filename;
        }
    }

    public void CloseFilenameSelector()
    {
        Button[] loadButtons = selectorContent.GetComponentsInChildren<Button>();
        foreach (Button button in loadButtons) Destroy(button.gameObject);

        selectorCloser.gameObject.SetActive(false);
        mazeSelector.gameObject.SetActive(false);
    }

    // deprecated
    public void DisplayLoadFilenameInput()
    {
        loadFilenameSetter.transform.parent.gameObject.SetActive(true);
    }

    // deprecated
    public void CloseLoadInput()
    {
        loadFilenameSetter.transform.parent.gameObject.SetActive(false);
    }

    public void SaveFile()
    {
        string filename = saveFilenameSetter.text;
        if (filename == "") return;
        saveFilenameSetter.text = "";
        saveFilenameSetter.transform.parent.gameObject.SetActive(false);
        level.SaveMaze(filename);
    }

    public void LoadFile()
    {
        Debug.Log("loading a file...");
        string filename = loadFilenameSetter.text;
        loadFilenameSetter.text = "";
        loadFilenameSetter.transform.parent.gameObject.SetActive(false);
        if (filename == "") return;
        level.LoadMaze(filename);
        CloseFilenameSelector();
    }

    public void LoadFile(string fname)
    {
        if (fname == "") return;
        level.LoadMaze(fname);
        CloseFilenameSelector();
        RepositionTopViewCam();
    }

    public void Generate()
    {
        if (runner != null) return;
        level.StartGeneration();
    }

    public void ResetMaze()
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

        bool isHeightNumeric = int.TryParse(heightSetter.text, out newHeight);
        bool isWidthNumeric = int.TryParse(widthSetter.text, out newWidth);

        if (!isHeightNumeric) newHeight = Maze.minHeight;
        else if (newHeight < Maze.minHeight) newHeight = Maze.minHeight;
        else if (newHeight > Maze.maxHeight) newHeight = Maze.maxHeight;

        if (!isWidthNumeric) newWidth = Maze.minWidth;
        else if (newWidth < Maze.minWidth) newWidth = Maze.minWidth;
        else if (newWidth > Maze.maxWidth) newWidth = Maze.maxWidth;

        if (newHeight == level.Height) Debug.Log("No change in height");
        if (newWidth == level.Width) Debug.Log("No change in width");

        level.MakeBlankGrid(newHeight, newWidth);
        RepositionTopViewCam();
    }


    #region maze generation GUI



    #endregion
    

    #region gameplay HUD



    #endregion


    #region maze playing options GUI

    public void ShowGraphOptions()
    {
        mazeOptions.gameObject.SetActive(false);
        graphOptions.gameObject.SetActive(true);
        level.ShowGraph();
    }

    public void PlayLevel()
    {
        if (!level.Playable()) return;
        level.HideGraph();
        mazeDisplay.gameObject.SetActive(false);
        playDisplay.SetActive(true);
        topViewCam.enabled = false;
        runner = Instantiate(runnerTemplate) as GameRunner3D;
        runner.Level = level;
        runner.initializer = this;
        runner.WinText = null;
    }

    public void StopPlaying()
    {
        if (runner == null) return;
        mazeDisplay.gameObject.SetActive(true);
        playDisplay.SetActive(false);
        topViewCam.enabled = true;
        Destroy(runner.gameObject);
    }

    public void SolveMazeLevel()
    {
        level.SolveMaze();
    }

    #endregion


    #region graph options GUI

    public void ShowMazeOptions()
    {
        mazeOptions.gameObject.SetActive(true);
        graphOptions.gameObject.SetActive(false);
        level.ShowMaze();
    }

    public void SolveGraphLevel()
    {
        level.SolveGraph();
    }

    public void RunDFS()
    {
        level.DFS();
    }

    public void RunBFS()
    {
        level.BFS();
    }

    #endregion

}
