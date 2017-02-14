using UnityEngine;
using UnityEngine.UI;

public class PlayInitializer3D : MonoBehaviour
{
    public Maze MazeTemplate;
    public GameRunner3D RunnerTemplate;
    public TileScript3D TileTemplate;
    public PlayerController3D PlayerTemplate;

    public Canvas Display;
    public Text WinText;
    public InputField HeightSetter;
    public InputField WidthSetter;
    public Text StackSizeText;
    public Text UnvisitedTilesText;

    [SerializeField]
    private Camera TopViewCam;
    private Camera playerCamera;
    private PlayerController3D player;
    private PlayLevelManager manager;
    private Maze level;
    private GameRunner3D runner;
    private string filename;

    private TileScript3D[,] testGrid;
    private Camera[] cameras;
    private int cameraIndex;

    // Use this for initialization
    private void Start()
    {
        level = Instantiate(MazeTemplate) as Maze;
        level.withDelay = true;

        /*
        player = Instantiate(PlayerTemplate, new Vector3(-12, 1, 0), Quaternion.identity) as PlayerController3D;
        playerCamera = player.GetComponent<Camera>();

        TopViewCam.enabled = true;
        playerCamera.enabled = false;
        */

        level.MakeBlankGrid(15, 15);

        level.stackCount = StackSizeText;
        level.setCount = UnvisitedTilesText;
        
        RepositionTopViewCam();

        level.PrepareForGeneration();
        
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //CycleCameras();
        }
    }

    private void CycleCameras()
    {
        TopViewCam.enabled = !TopViewCam.enabled;
        playerCamera.enabled = !playerCamera.enabled;
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
        TopViewCam.transform.rotation = Quaternion.Euler(90, 0, 0);
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
        RepositionTopViewCam();
    }
    
    public void PlayLevel()
    {
        if (!level.Playable()) return;
        Display.gameObject.SetActive(false);
        TopViewCam.enabled = false;
        runner = Instantiate(RunnerTemplate) as GameRunner3D;
        runner.Level = level;
        runner.initializer = this;
        runner.WinText = WinText;
    }

    public void StopPlaying()
    {
        if (runner == null) return;
        Display.gameObject.SetActive(true);
        //runner.EndGame();
        TopViewCam.enabled = true;
        Destroy(runner.gameObject);
    }

}
