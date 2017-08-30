using UnityEngine;
using System.Collections.Generic;

public class TileScript : MonoBehaviour
{

    public const string verticalCorridor = "0101", horizontalCorridor = "1010";

    public static readonly HashSet<string> corridorCodes = new HashSet<string>()
    {
        { "1010" },
        { "0101" }
    };

    [HideInInspector]
    public int X, Y;

    public WallScript wallTemplate;

    protected WallScript[] walls = new WallScript[4];
    protected Dictionary<Direction, WallScript> wallDirs = new Dictionary<Direction, WallScript>();
    protected Dictionary<Direction, WallScript> correspondingWallDirs = new Dictionary<Direction, WallScript>();
    
    public static readonly Dictionary<Direction, Direction> correspondingDirs = new Dictionary<Direction, Direction>
    {
        { Direction.North, Direction.South },
        { Direction.East, Direction.West },
        { Direction.South, Direction.North },
        { Direction.West, Direction.East }
    };

    public Vector3 StartingPosition
    {
        get
        {
            return Vector3.zero;
        }
    }
    public Vector3 HorizontalSpawnIncrement
    {
        get
        {
            return new Vector3(1.8f, 0, 0);
        }
    }
    public Vector3 VerticalSpawnIncrement
    {
        get
        {
            return new Vector3(0, 0, 1.8f);
        }
    }

    #region 3D vars/properties

    private MeshRenderer meshRenderer;

    public Material StartingMaterial;
    public Material VisitedMaterial;
    public Material StackMaterial;
    public Material BeginMaterial;
    public Material EndMaterial;
    public Material OpenMaterial;
    public Material ClosedMaterial;
    public Material SearchMaterial;

    private const float xWallPos = 0.45f, yWallPos = 9.5f;

    #endregion

    protected virtual void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.material = StartingMaterial;

        CreateWalls();
    }

    protected virtual void InitWall(Direction dir, Vector3 offset, Vector3 rotation)
    {
        int d = (int)dir;

        walls[d] = Instantiate(wallTemplate, this.transform, false) as WallScript;
        walls[d].transform.localPosition = offset;
        walls[d].transform.localRotation = Quaternion.Euler(rotation);

        wallDirs.Add(dir, walls[d]);
        correspondingWallDirs.Add(correspondingDirs[dir], walls[d]);
    }

    protected virtual void CreateWalls()
    {
        wallDirs.Clear();
        correspondingWallDirs.Clear();

        /*
        WallScript upWall = walls[(int)Direction.North];
        WallScript rightWall = walls[(int)Direction.East];
        WallScript downWall = walls[(int)Direction.South];
        WallScript leftWall = walls[(int)Direction.West];
        */

        InitWall(Direction.North, new Vector3(0, yWallPos, xWallPos), new Vector3(90, 0, 0));
        InitWall(Direction.East, new Vector3(xWallPos, yWallPos, 0), new Vector3(90, 90, 0));
        InitWall(Direction.South, new Vector3(0, yWallPos, -xWallPos), new Vector3(90, 0, 0));
        InitWall(Direction.West, new Vector3(-xWallPos, yWallPos, 0), new Vector3(90, 90, 0));
    }

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

    public void Visit()
    {
        meshRenderer.material = VisitedMaterial;
    }

    public void OnStack()
    {
        meshRenderer.material = StackMaterial;
    }

    public void UnVisit()
    {
        meshRenderer.material = StartingMaterial;
    }

    public void MakeStartTile()
    {
        meshRenderer.material = BeginMaterial;
    }

    public void MakeEndTile()
    {
        meshRenderer.material = EndMaterial;
    }

    public void MakeOpen()
    {
        meshRenderer.material = OpenMaterial;
    }

    public void MakeClosed()
    {
        meshRenderer.material = ClosedMaterial;
    }

    public void SearchVisit()
    {
        meshRenderer.material = SearchMaterial;
    }

    public override string ToString()
    {
        return "tile at (" + this.X + ", " + this.Y + ")";
    }

}
