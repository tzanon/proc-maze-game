using UnityEngine;

public class GameRunner2D : GameRunner
{

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
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

    protected override void PlacePlayer()
    {
        UpdatePlayerPosition(Level.startTile);
    }

    protected override void PlaceDestintation()
    {
        Destination = Instantiate(DestTemplate, EndTile.transform.position, Quaternion.identity) as DestinationScript;
        Destination.transform.parent = EndTile.transform;
    }

    private void TakeInput(KeyCode key, int newX, int newY)
    {
        if (Level.InBounds(newX, newY) &&
            !_grid[Player.y, Player.x].WallsBetweenTilesExist(_grid[newY, newX]))
        {
            UpdatePlayerPosition(_grid[newY, newX]);
        }
    }

    private void UpdatePlayerPosition(TileScript tile)
    {
        //Debug.Log("Current position: " + Player.x + ", " + Player.y);
        Player.x = tile.X;
        Player.y = tile.Y;
        Player.transform.position = tile.transform.position;
        //Debug.Log("New position: " + Player.x + ", " + Player.y);
        if (tile == Level.endTile)
        {
            Debug.Log("Maze solved!");
            GameWon();
        }
    }

}
