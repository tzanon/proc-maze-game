using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public abstract class GameRunner : MonoBehaviour {

    protected Text _winText;
    protected Maze _level;
    protected TileScript[,] _grid;
    private PlayerController _player;
    private TileScript _endTile;
    private DestinationScript _destination;

    public DestinationScript Destination
    {
        get { return _destination; }
        protected set { _destination = value; }
    }


    public PlayerController PlayerTemplate;
    public DestinationScript DestTemplate;

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
                _grid = value.Grid;
            }
        }
    }
    public Text WinText
    {
        get { return _winText; }
        set
        {
            _winText = value;
            _winText.gameObject.SetActive(false);
        }
    }
    public PlayerController Player
    {
        get { return _player; }
        protected set { _player = value; }
    }
    public TileScript EndTile
    {
        get { return _endTile; }
        protected set { _endTile = value; }
    }

    // Use this for initialization
    protected virtual void Start ()
    {
        Debug.Log("start tile is " + Level.startTile);
        Player = Instantiate(PlayerTemplate) as PlayerController;
        EndTile = Level.endTile;
        PlaceDestintation();
        PlacePlayer();
    }
	
	// Update is called once per frame
	void Update () {
	    
	}

    protected abstract void PlacePlayer();

    protected abstract void PlaceDestintation();

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
        WinText.gameObject.SetActive(true);
        yield return new WaitForSeconds(3);
        WinText.gameObject.SetActive(false);
        EndGame();
    }

}
