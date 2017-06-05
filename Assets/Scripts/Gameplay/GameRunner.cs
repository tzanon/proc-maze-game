using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public abstract class GameRunner : MonoBehaviour {

    protected Text winText;
    protected Maze level;
    protected TileScript[,] grid;
    private PlayerController player;
    private TileScript endTile;
    private DestinationScript destination;

    public DestinationScript Destination
    {
        get { return destination; }
        protected set { destination = value; }
    }

    public PlayerController PlayerTemplate;
    public DestinationScript DestTemplate;

    public Maze Level
    {
        get { return level; }
        set
        {
            if (!value.Playable())
                Debug.LogError("This level is not playable.", value);
            else
            {
                level = value;
                grid = value.Grid;
            }
        }
    }
    public Text WinText
    {
        get { return winText; }
        set
        {
            if (value == null) return;
            winText = value;
            winText.gameObject.SetActive(false);
        }
    }
    public PlayerController Player
    {
        get { return player; }
        protected set { player = value; }
    }
    public TileScript EndTile
    {
        get { return endTile; }
        protected set { endTile = value; }
    }

    protected virtual void Start ()
    {
        Debug.Log("start tile is " + Level.startTile);
        Player = Instantiate(PlayerTemplate) as PlayerController;
        EndTile = Level.endTile;
        PlaceDestintation();
        PlacePlayer();
    }
    
	protected virtual void Update ()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Debug.Log("quitting...");
            EndGame();
        }
    }

    protected abstract void PlacePlayer();

    protected abstract void PlaceDestintation();

    protected abstract void PlacePowerups();

    protected abstract void PlaceEnemies();

    public void GameWon()
    {
        StartCoroutine(DisplayWon());
    }

    public virtual void EndGame()
    {
        Destroy(Player.gameObject);
        Destroy(Destination);
    }

    protected IEnumerator DisplayWon()
    {
        if (WinText != null)
        {
            WinText.gameObject.SetActive(true);
            yield return new WaitForSeconds(3);
            WinText.gameObject.SetActive(false);
        }
        EndGame();
    }

}
