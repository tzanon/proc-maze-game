using UnityEngine;
using System.Collections.Generic;
using System;

public class TileScript2D : TileScript
{
    private SpriteRenderer _spriteRenderer;

    public Sprite StartingSprite;
    public Sprite VisitedSprite;
    public Sprite StackSprite;
    public Sprite BeginSprite;
    public Sprite EndSprite;

    private float _wallPos;

    public override Vector3 StartingPosition
    {
        get
        {
            return new Vector3(-8.25f, -4f, 0);
        }
    }
    public override Vector3 HorizontalSpawnIncrement
    {
        get
        {
            return new Vector3(0.535f, 0, 0);
        }
    }
    public override Vector3 VerticalSpawnIncrement
    {
        get
        {
            return new Vector3(0, 0.535f, 0);
        }
    }

    protected override void Awake ()
    {
        base.Awake();
        _wallPos = 0.265f;
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _spriteRenderer.sprite = StartingSprite;

        CreateWalls();
    }

    protected override void CreateWalls()
    {
        wallDirs.Clear();
        correspondingWallDirs.Clear();

        WallScript upWall = walls[(int)Direction.North];
        WallScript rightWall = walls[(int)Direction.East];
        WallScript downWall = walls[(int)Direction.South];
        WallScript leftWall = walls[(int)Direction.West];

        InitWall(Direction.North,    new Vector3(        0,  _wallPos,    0),           Vector3.zero);
        InitWall(Direction.East, new Vector3( _wallPos,         0,    0),  new Vector3(0, 0, 90));
        InitWall(Direction.South,  new Vector3(        0, -_wallPos,    0),           Vector3.zero);
        InitWall(Direction.West,  new Vector3(-_wallPos,         0,    0),  new Vector3(0, 0, 90));

    }

    public override void Visit()
    {
        _spriteRenderer.sprite = VisitedSprite;
    }

    public override void OnStack()
    {
        _spriteRenderer.sprite = StackSprite;
    }

    public override void UnVisit()
    {
        _spriteRenderer.sprite = StartingSprite;
    }

    public override void MakeStartTile()
    {
        _spriteRenderer.sprite = BeginSprite;
    }

    public override void MakeEndTile()
    {
        _spriteRenderer.sprite = EndSprite;
    }

    public override void MakeOpen()
    {
        
    }

    public override void MakeClosed()
    {
        
    }

    public override void SearchVisit()
    {

    }

}
