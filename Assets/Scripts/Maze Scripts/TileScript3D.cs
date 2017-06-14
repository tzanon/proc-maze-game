using UnityEngine;
using System.Collections;
using System;

public class TileScript3D : TileScript {

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
        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.material = StartingMaterial;

        CreateWalls();
    }

    protected override void CreateWalls()
    {
        _wallDirs.Clear();
        _correspondingWallDirs.Clear();

        WallScript upWall = _walls[(int)Directions.North];
        WallScript rightWall = _walls[(int)Directions.East];
        WallScript downWall = _walls[(int)Directions.South];
        WallScript leftWall = _walls[(int)Directions.West];

        InitWall(Directions.North,    new Vector3(         0,  yWallPos,  xWallPos), new Vector3(90, 0, 0));
        InitWall(Directions.East, new Vector3( xWallPos,  yWallPos,          0), new Vector3(90, 90, 0));
        InitWall(Directions.South,  new Vector3(         0,  yWallPos, -xWallPos), new Vector3(90, 0, 0));
        InitWall(Directions.West,  new Vector3(-xWallPos,  yWallPos,          0), new Vector3(90, 90, 0));

    }

    public override void Visit()
    {
        meshRenderer.material = VisitedMaterial;
    }

    public override void OnStack()
    {
        meshRenderer.material = StackMaterial;
    }

    public override void UnVisit()
    {
        meshRenderer.material = StartingMaterial;
    }

    public override void MakeStartTile()
    {
        meshRenderer.material = BeginMaterial;
    }

    public override void MakeEndTile()
    {
        meshRenderer.material = EndMaterial;
    }

    public override void MakeOpen()
    {
        meshRenderer.material = OpenMaterial;
    }

    public override void MakeClosed()
    {
        meshRenderer.material = ClosedMaterial;
    }

    public override void SearchVisit()
    {
        meshRenderer.material = SearchMaterial;
    }

}
