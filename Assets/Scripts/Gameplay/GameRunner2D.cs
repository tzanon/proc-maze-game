using UnityEngine;
using System.Collections;
using UnityEngine.UI;

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
        int currentX = _player.x;
        int currentY = _player.y;

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

    private void TakeInput(KeyCode key, int newX, int newY)
    {
        if (Level.InBounds(newX, newY) &&
            !_grid[_player.y, _player.x].WallsBetweenTilesExist(_grid[newY, newX]))
        {
            UpdatePlayerPosition(_grid[newY, newX]);
        }
    }

    private void UpdatePlayerPosition(TileScript tile)
    {
        //Debug.Log("Current position: " + Player.x + ", " + Player.y);
        _player.x = tile.X;
        _player.y = tile.Y;
        _player.transform.position = tile.transform.position;
        //Debug.Log("New position: " + Player.x + ", " + Player.y);
        if (tile == Level.endTile)
        {
            Debug.Log("Maze solved!");
            GameWon();
        }
    }

}
