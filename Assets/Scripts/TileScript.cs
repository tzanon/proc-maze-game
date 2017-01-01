﻿using UnityEngine;
using System.Collections.Generic;

public class TileScript : MonoBehaviour
{

    private SpriteRenderer spriteRenderer;
    private float wallPos = 0.265f;
    
    private WallScript[] walls = new WallScript[4];

    public Sprite startingSprite;
    public Sprite visitedSprite;
    public Sprite stackSprite;

    [HideInInspector]
    public int x, y;

    public enum Directions { Up, Right, Down, Left, Centre };

    public WallScript wall;
    private Dictionary<Directions, WallScript> wallDirs = new Dictionary<Directions, WallScript>();
    private Dictionary<Directions, WallScript> correspondingWallDirs = new Dictionary<Directions, WallScript>();
    private Dictionary<Directions, Directions> correspondingDirs = new Dictionary<Directions, Directions>();

    void Awake ()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        correspondingDirs.Add(Directions.Up, Directions.Down);
        correspondingDirs.Add(Directions.Right, Directions.Left);
        correspondingDirs.Add(Directions.Down, Directions.Up);
        correspondingDirs.Add(Directions.Left, Directions.Right);

        CreateWalls();
    }

    void InitWall(Directions dir, Vector3 offset, int rotation)
    {
        int d = (int)dir;
        walls[d] = Instantiate(wall, this.transform, false) as WallScript;
        walls[d].transform.localPosition = offset;
        walls[d].transform.localRotation = Quaternion.Euler(0, 0, rotation);
        wallDirs.Add(dir, walls[d]);
        correspondingWallDirs.Add(correspondingDirs[dir], walls[d]);
    }

    void CreateWalls()
    {
        wallDirs.Clear();
        correspondingWallDirs.Clear();

        WallScript upWall = walls[(int)Directions.Up];
        WallScript rightWall = walls[(int)Directions.Right];
        WallScript downWall = walls[(int)Directions.Down];
        WallScript leftWall = walls[(int)Directions.Left];
        
        if (leftWall != null) Destroy(leftWall.gameObject);
        if (rightWall != null) Destroy(rightWall.gameObject);
        if (upWall != null) Destroy(upWall.gameObject);
        if (downWall != null) Destroy(downWall.gameObject);

        InitWall(Directions.Up, new Vector3(0, wallPos, 0), 0);
        InitWall(Directions.Right, new Vector3(wallPos, 0, 0), 90);
        InitWall(Directions.Down, new Vector3(0, -wallPos, 0), 0);
        InitWall(Directions.Left, new Vector3(-wallPos, 0, 0), 90);

    }

    void ReactivateWalls()
    {
        for (int i = 0; i < walls.Length; i++)
        {
            walls[i].gameObject.SetActive(true);
        }
    }

    private char WallExists(WallScript wall)
    {
        if (wall.gameObject.activeSelf) return '1';
        else return '0';
    }

    public string GetWallCode()
    {
        string result = "";

        for (int i = 0; i < 4; i++)
        {
            result += WallExists(walls[i]);
        }

        return result;
    }

    public void RemoveWallsBetweenTiles(TileScript otherTile)
    {
        Directions direction;

        Vector3 thisPos = this.transform.position;
        Vector3 otherPos = otherTile.transform.position;

        if (otherPos.x - thisPos.x > 0) direction = Directions.Right;
        else if (otherPos.x - thisPos.x < 0) direction = Directions.Left;
        else if (otherPos.y - thisPos.y > 0) direction = Directions.Up;
        else if (otherPos.y - thisPos.y < 0) direction = Directions.Down;
        else return; // the two tiles must be the same

        this.RemoveWall(direction);
        otherTile.RemoveCorrespondingWall(direction);

    }

    private void RemoveWall(Directions direction)
    {
        wallDirs[direction].gameObject.SetActive(false);
        //if (walls.Remove(direction)) Debug.Log("wall successfully removed");
    }

    private void RemoveCorrespondingWall(Directions direction)
    {
        correspondingWallDirs[direction].gameObject.SetActive(false);
        //Destroy(correspondingWallDirs[direction].gameObject);
        //if (correspondingWalls.Remove(direction)) Debug.Log("wall successfully removed");
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
        ReactivateWalls();
    }

}
