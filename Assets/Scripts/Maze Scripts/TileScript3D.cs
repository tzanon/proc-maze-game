using UnityEngine;
using System.Collections;

public class TileScript3D : TileScript {

    private MeshRenderer _meshRenderer;

    public Material StartingMaterial;
    public Material VisitedMaterial;
    public Material StackMaterial;
    public Material BeginMaterial;
    public Material EndMaterial;

    private const float _xWallPos = 0.45f, _yWallPos = 9.5f;

    public override Vector3 StartingPosition
    {
        get
        {
            return Vector3.zero;
        }
    }
    public override Vector3 HorizontalSpawnIncrement
    {
        get
        {
            return new Vector3(1.8f, 0, 0);
        }
    }
    public override Vector3 VerticalSpawnIncrement
    {
        get
        {
            return new Vector3(0, 0, 1.8f);
        }
    }

    protected override void Awake()
    {
        base.Awake();
        _meshRenderer = GetComponent<MeshRenderer>();
        _meshRenderer.material = StartingMaterial;

        CreateWalls();
    }

    protected override void CreateWalls()
    {
        _wallDirs.Clear();
        _correspondingWallDirs.Clear();

        WallScript upWall = _walls[(int)Directions.Up];
        WallScript rightWall = _walls[(int)Directions.Right];
        WallScript downWall = _walls[(int)Directions.Down];
        WallScript leftWall = _walls[(int)Directions.Left];

        InitWall(Directions.Up,    new Vector3( _xWallPos,  _yWallPos,          0), new Vector3(90, 90, 0));
        InitWall(Directions.Right, new Vector3(         0,  _yWallPos, -_xWallPos), new Vector3(90, 0, 0));
        InitWall(Directions.Down,  new Vector3(-_xWallPos,  _yWallPos,          0), new Vector3(90, 90, 0));
        InitWall(Directions.Left,  new Vector3(         0,  _yWallPos,  _xWallPos), new Vector3(90, 0, 0));

    }

    public override void Visit()
    {
        
    }

    public override void OnStack()
    {

    }

    public override void UnVisit()
    {

    }

    public override void MakeStartTile()
    {

    }

    public override void MakeEndTile()
    {

    }

}
