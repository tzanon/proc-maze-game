using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameRunner : MonoBehaviour
{
    
    private Text _winText;
    private Maze _level;
    private TileScript[,] grid;

    private PlayerController Player;

    public Maze Level
    {
        get { return _level; }
        set
        {
            if (!value.Playable())
                Debug.LogError("This level is not playable.", value);
            else
            {
                _level = value;
                grid = value.Grid;
            }
        }
    }
    public PlayerController PlayerTemplate;
    public Text WinText
    {
        get { return _winText; }
        set
        {
            _winText = value;
            _winText.gameObject.SetActive(false);
        }
    }

    // Use this for initialization
    void Start()
    {
        Debug.Log("start tile is " + Level.startTile);
        Player = Instantiate(PlayerTemplate) as PlayerController;
        UpdatePlayerPosition(Level.startTile);
	}

	// Handle player input
	void Update()
    {
        int currentX = Player.x;
        int currentY = Player.y;

	    if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            TakeInput(KeyCode.UpArrow, currentX, currentY + 1);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            TakeInput(KeyCode.DownArrow, currentX, currentY - 1);
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            TakeInput(KeyCode.LeftArrow, currentX - 1, currentY);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            TakeInput(KeyCode.RightArrow, currentX + 1, currentY);
        }

    }

    private void TakeInput(KeyCode key, int newX, int newY)
    {
        if (Level.InBounds(newX, newY) &&
            !grid[Player.y, Player.x].WallsBetweenTilesExist(grid[newY, newX]))
        {
            UpdatePlayerPosition(grid[newY, newX]);
        }
    }

    private void UpdatePlayerPosition(TileScript tile)
    {
        //Debug.Log("Current position: " + Player.x + ", " + Player.y);
        Player.x = tile.x;
        Player.y = tile.y;
        Player.transform.position = tile.transform.position;
        //Debug.Log("New position: " + Player.x + ", " + Player.y);
        if (tile == Level.endTile)
        {
            Debug.Log("Maze solved!");
            GameWon();
        }
    }

    public void GameWon()
    {
        // display win text
        StartCoroutine(DisplayWon());
    }

    public void EndGame()
    {
        Destroy(Player.gameObject);
        Destroy(this.gameObject);
    }

    private IEnumerator DisplayWon()
    {
        WinText.gameObject.SetActive(true);
        yield return new WaitForSeconds(3);
        WinText.gameObject.SetActive(false);
        EndGame();
    }

}
