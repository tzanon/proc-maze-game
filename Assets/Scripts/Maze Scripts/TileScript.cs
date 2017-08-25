using UnityEngine;
using System.Collections.Generic;

public abstract class TileScript : MonoBehaviour {

    public const string VerticalCorridor = "0101", HorizontalCorridor = "1010";

    public static readonly HashSet<string> CorridorCodes = new HashSet<string>()
    {
        { "1010" },
        { "0101" }
    };

    [HideInInspector]
    public int X, Y;

    public WallScript WallTemplate;

    protected WallScript[] walls = new WallScript[4];
    protected Dictionary<Direction, WallScript> wallDirs = new Dictionary<Direction, WallScript>();
    protected Dictionary<Direction, WallScript> correspondingWallDirs = new Dictionary<Direction, WallScript>();
    
    public static readonly Dictionary<Direction, Direction> CorrespondingDirs = new Dictionary<Direction, Direction>
    {
        { Direction.North, Direction.South },
        { Direction.East, Direction.West },
        { Direction.South, Direction.North },
        { Direction.West, Direction.East }
    };

    public abstract Vector3 StartingPosition
    {
        get;
    }
    public abstract Vector3 HorizontalSpawnIncrement
    {
        get;
    }
    public abstract Vector3 VerticalSpawnIncrement
    {
        get;
    }

    protected virtual void Awake()
    {
    }

    protected virtual void InitWall(Direction dir, Vector3 offset, Vector3 rotation)
    {
        int d = (int)dir;

        walls[d] = Instantiate(WallTemplate, this.transform, false) as WallScript;
        walls[d].transform.localPosition = offset;
        walls[d].transform.localRotation = Quaternion.Euler(rotation);

        wallDirs.Add(dir, walls[d]);
        correspondingWallDirs.Add(CorrespondingDirs[dir], walls[d]);
    }

    protected abstract void CreateWalls();

    public bool WallActive(Direction d)
    {
        return walls[(int)d].gameObject.activeSelf;
    }

    public string GetWallCode()
    {
        string code = "";

        for (int i = 0; i < walls.Length; i++)
        {
            if (walls[i].gameObject.activeSelf) code += '1';
            else code += '0';
        }

        return code;
    }

    public int GetNumActiveWalls()
    {
        int num = 0;

        for (int i = 0; i < walls.Length; i++)
            if (walls[i].gameObject.activeSelf) num++;

        return num;
    }

    public int GetNumExistingWalls()
    {
        int num = 0;

        for (int i = 0; i < walls.Length; i++)
            if (walls[i].gameObject != null) num++;

        return num;
    }

    public void SetWallsFromCode(string code)
    {
        for (int i = 0; i < walls.Length; i++)
        {
            if (code[i] == '1') walls[i].gameObject.SetActive(true);
            else walls[i].gameObject.SetActive(false);
        }
    }

    public void RemoveWallsBetweenTiles(TileScript otherTile)
    {
        Direction direction;

        Vector3 thisPos = this.transform.position;
        Vector3 otherPos = otherTile.transform.position;

        if      (otherTile.X - this.X > 0) direction = Direction.East;
        else if (otherTile.X - this.X < 0) direction = Direction.West;
        else if (otherTile.Y - this.Y > 0) direction = Direction.North;
        else if (otherTile.Y - this.Y < 0) direction = Direction.South;
        else return; // the two tiles must be the same

        this.RemoveWall(direction);
        otherTile.RemoveCorrespondingWall(direction);

    }

    protected void RemoveWall(Direction direction)
    {
        wallDirs[direction].gameObject.SetActive(false);
    }

    protected void RemoveCorrespondingWall(Direction direction)
    {
        correspondingWallDirs[direction].gameObject.SetActive(false);
    }

    public bool WallsBetweenTilesExist(TileScript otherTile)
    {
        Direction direction;

        Vector3 thisPos = this.transform.position;
        Vector3 otherPos = otherTile.transform.position;

        if      (otherTile.X - this.X > 0) direction = Direction.East;
        else if (otherTile.X - this.X < 0) direction = Direction.West;
        else if (otherTile.Y - this.Y > 0) direction = Direction.North;
        else if (otherTile.Y - this.Y < 0) direction = Direction.South;
        else return true; // the two tiles must be the same

        if (this.wallDirs[direction].gameObject.activeSelf
            && otherTile.correspondingWallDirs[direction].gameObject.activeSelf)
            return true;
        else
            return false;
    }

    public void ResetTile()
    {
        SetWallsFromCode("1111");
    }

    public abstract void Visit();

    public abstract void OnStack();

    public abstract void UnVisit();

    public abstract void MakeStartTile();

    public abstract void MakeEndTile();

    public abstract void MakeOpen();

    public abstract void MakeClosed();

    public abstract void SearchVisit();

    public override string ToString()
    {
        return "tile at (" + this.X + ", " + this.Y + ")";
    }

}
