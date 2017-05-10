using UnityEngine;
using System.Collections.Generic;

public abstract class TileScript : MonoBehaviour {

    [HideInInspector]
    public int X, Y;

    public enum Directions { Up, Right, Down, Left, Centre };

    public WallScript WallTemplate;

    protected WallScript[] _walls = new WallScript[4];
    protected Dictionary<Directions, WallScript> _wallDirs = new Dictionary<Directions, WallScript>();
    protected Dictionary<Directions, WallScript> _correspondingWallDirs = new Dictionary<Directions, WallScript>();
    protected Dictionary<Directions, Directions> _correspondingDirs = new Dictionary<Directions, Directions>();

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
        _correspondingDirs.Add(Directions.Up, Directions.Down);
        _correspondingDirs.Add(Directions.Right, Directions.Left);
        _correspondingDirs.Add(Directions.Down, Directions.Up);
        _correspondingDirs.Add(Directions.Left, Directions.Right);
    }

    protected virtual void InitWall(Directions dir, Vector3 offset, Vector3 rotation)
    {
        int d = (int)dir;

        _walls[d] = Instantiate(WallTemplate, this.transform, false) as WallScript;
        _walls[d].transform.localPosition = offset;
        _walls[d].transform.localRotation = Quaternion.Euler(rotation);

        _wallDirs.Add(dir, _walls[d]);
        _correspondingWallDirs.Add(_correspondingDirs[dir], _walls[d]);
    }

    protected abstract void CreateWalls();

    public string GetWallCode()
    {
        string code = "";

        for (int i = 0; i < _walls.Length; i++)
        {
            if (_walls[i].gameObject.activeSelf) code += '1';
            else code += '0';
        }

        return code;
    }

    public int GetNumActiveWalls()
    {
        int num = 0;

        for (int i = 0; i < _walls.Length; i++)
            if (_walls[i].gameObject.activeSelf) num++;

        return num;
    }

    public int GetNumExistingWalls()
    {
        int num = 0;

        for (int i = 0; i < _walls.Length; i++)
            if (_walls[i].gameObject != null) num++;

        return num;
    }

    public void SetWallsFromCode(string code)
    {
        for (int i = 0; i < _walls.Length; i++)
        {
            if (code[i] == '1') _walls[i].gameObject.SetActive(true);
            else _walls[i].gameObject.SetActive(false);
        }
    }

    public void RemoveWallsBetweenTiles(TileScript otherTile)
    {
        Directions direction;

        Vector3 thisPos = this.transform.position;
        Vector3 otherPos = otherTile.transform.position;

        if      (otherTile.X - this.X > 0) direction = Directions.Right;
        else if (otherTile.X - this.X < 0) direction = Directions.Left;
        else if (otherTile.Y - this.Y > 0) direction = Directions.Up;
        else if (otherTile.Y - this.Y < 0) direction = Directions.Down;
        else return; // the two tiles must be the same

        this.RemoveWall(direction);
        otherTile.RemoveCorrespondingWall(direction);

    }

    protected void RemoveWall(Directions direction)
    {
        _wallDirs[direction].gameObject.SetActive(false);
    }

    protected void RemoveCorrespondingWall(Directions direction)
    {
        _correspondingWallDirs[direction].gameObject.SetActive(false);
    }

    public bool WallsBetweenTilesExist(TileScript otherTile)
    {
        Directions direction;

        Vector3 thisPos = this.transform.position;
        Vector3 otherPos = otherTile.transform.position;

        if      (otherTile.X - this.X > 0) direction = Directions.Right;
        else if (otherTile.X - this.X < 0) direction = Directions.Left;
        else if (otherTile.Y - this.Y > 0) direction = Directions.Up;
        else if (otherTile.Y - this.Y < 0) direction = Directions.Down;
        else return true; // the two tiles must be the same

        if (this._wallDirs[direction].gameObject.activeSelf
            && otherTile._correspondingWallDirs[direction].gameObject.activeSelf)
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

}
