using UnityEngine;
using System.Collections.Generic;

public class TileScript : MonoBehaviour
{

    private SpriteRenderer spriteRenderer;
    private WallScript leftWall, rightWall, upWall, downWall;
    private float wallPos = 0.265f;

    public Sprite startingSprite;
    public Sprite visitedSprite;
    public Sprite stackSprite;

    [HideInInspector]
    public int x, y;

    public enum Directions { Centre = 0, Up = 1, Right = 2, Down = 3, Left = 4 };

    public WallScript wall;
    private Dictionary<Directions, WallScript> walls = new Dictionary<Directions, WallScript>();
    private Dictionary<Directions, WallScript> correspondingWalls = new Dictionary<Directions, WallScript>();

    void Awake ()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        CreateWalls();
    }

    void CreateWalls()
    {
        walls.Clear();
        correspondingWalls.Clear();

        if (leftWall != null) Destroy(leftWall.gameObject);
        if (rightWall != null) Destroy(rightWall.gameObject);
        if (upWall != null) Destroy(upWall.gameObject);
        if (downWall != null) Destroy(downWall.gameObject);

        leftWall = Instantiate(wall, this.transform, false) as WallScript;
        leftWall.transform.localPosition = new Vector3(-wallPos, 0, 0);
        leftWall.transform.localRotation = Quaternion.Euler(0, 0, 90);
        walls.Add(Directions.Left, leftWall);
        correspondingWalls.Add(Directions.Right, leftWall);

        rightWall = Instantiate(wall, this.transform, false) as WallScript;
        rightWall.transform.localPosition = new Vector3(wallPos, 0, 0);
        rightWall.transform.localRotation = Quaternion.Euler(0, 0, 90);
        walls.Add(Directions.Right, rightWall);
        correspondingWalls.Add(Directions.Left, rightWall);

        upWall = Instantiate(wall, this.transform, false) as WallScript;
        upWall.transform.localPosition = new Vector3(0, wallPos, 0);
        walls.Add(Directions.Up, upWall);
        correspondingWalls.Add(Directions.Down, upWall);

        downWall = Instantiate(wall, this.transform, false) as WallScript;
        downWall.transform.localPosition = new Vector3(0, -wallPos, 0);
        walls.Add(Directions.Down, downWall);
        correspondingWalls.Add(Directions.Up, downWall);

    }

    public void Visit()
    {
        spriteRenderer.sprite = visitedSprite;
    }

    public void OnStack()
    {
        spriteRenderer.sprite = stackSprite;
    }

    public void ResetTile()
    {
        spriteRenderer.sprite = startingSprite;
        CreateWalls();
    }

    public void DeleteWallsBetweenTiles(TileScript otherTile)
    {
        Directions direction;

        if (otherTile.x - this.x > 0) direction = Directions.Right;
        else if (otherTile.x - this.x < 0) direction = Directions.Left;
        else if (otherTile.y - this.y > 0) direction = Directions.Down;
        else if (otherTile.y - this.y < 0) direction = Directions.Up;
        else return; // the two tiles must be the same

        this.DeleteWall(direction);
        otherTile.DeleteCorrespondingWall(direction);

    }

    public void DeleteWall(Directions direction)
    {
        Destroy(walls[direction].gameObject);
        //if (walls.Remove(direction)) Debug.Log("wall successfully removed");
    }

    public void DeleteCorrespondingWall(Directions direction)
    {
        Destroy(correspondingWalls[direction].gameObject);
        //if (correspondingWalls.Remove(direction)) Debug.Log("wall successfully removed");

    }

}
