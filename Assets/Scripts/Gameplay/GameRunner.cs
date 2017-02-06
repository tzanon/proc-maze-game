using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public abstract class GameRunner : MonoBehaviour {

    protected Text _winText;
    protected Maze _level;
    protected TileScript[,] _grid;
    protected PlayerController _player;

    

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
    public PlayerController Player
    {
        get { return _player; }
        protected set { _player = value; }
    }

    // Use this for initialization
    protected virtual void Start ()
    {
        Debug.Log("start tile is " + Level.startTile);
        _player = Instantiate(PlayerTemplate) as PlayerController;
        PlacePlayer();
    }
	
	// Update is called once per frame
	void Update () {
	    
	}

    protected abstract void PlacePlayer();

    public void GameWon()
    {
        StartCoroutine(DisplayWon());
    }

    public virtual void EndGame()
    {
        Destroy(_player.gameObject);
        Destroy(this.gameObject);
    }

    protected IEnumerator DisplayWon()
    {
        WinText.gameObject.SetActive(true);
        yield return new WaitForSeconds(3);
        WinText.gameObject.SetActive(false);
        EndGame();
    }

}
