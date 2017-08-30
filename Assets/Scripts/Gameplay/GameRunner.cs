using UnityEngine;
using UnityEngine.UI;

public class GameRunner : MonoBehaviour
{
    protected Text winText;
    protected Maze level;

    private PlayerController player;
    private DestinationScript destination;

    public PlayerController playerTemplate;
    public DestinationScript destTemplate;

    public Maze Level
    {
        get { return level; }
        set
        {
            if (!value.Playable())
                Debug.LogError("This level is not playable.", value);
            else
                level = value;
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
    }

    protected TileScript EndTile
    {
        get { return level.EndTile; }
    }

    [HideInInspector]
    public PlayInitializer3D initializer;
    private const float actorHeight = 1f;

    protected virtual void Start ()
    {
        player = Instantiate(playerTemplate) as PlayerController;
        player.mazeGraph = level.GraphRepresentation;

        PlaceDestintation();
        PlacePlayer();

        Player.GetComponent<Camera>().enabled = true;
    }
    
	protected virtual void Update ()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            EndGame();
        }
    }

    protected virtual void PlacePlayer()
    {
        Player.transform.position = Level.StartTile.transform.position + new Vector3(0, actorHeight, 0);
    }

    protected virtual void PlaceDestintation()
    {
        destination = Instantiate(destTemplate, EndTile.transform.position + new Vector3(0, 1, 0), Quaternion.identity) as DestinationScript;
        destination.transform.parent = EndTile.transform;
        destination.Level = this.Level;
        destination.Runner = this;
    }

    protected virtual void PlacePowerups()
    {

    }

    public virtual void EndGame()
    {
        Player.GetComponent<Camera>().enabled = false;
        Destroy(destination.gameObject);
        Destroy(player.gameObject);

        initializer.StopPlaying();
    }

}
