using UnityEngine;
using System.Collections.Generic;

public class TileScript2D : TileScript
{
    private SpriteRenderer _spriteRenderer;

    public Sprite StartingSprite;
    public Sprite VisitedSprite;
    public Sprite StackSprite;
    public Sprite BeginSprite;
    public Sprite EndSprite;

    private float _wallPos;

    /*
    public enum Directions { Up, Right, Down, Left, Centre };

    public WallScript WallTemplate;
    private float _wallPos = 0.265f;

    private WallScript[] _walls = new WallScript[4];
    private Dictionary<Directions, WallScript> _wallDirs = new Dictionary<Directions, WallScript>();
    private Dictionary<Directions, WallScript> _correspondingWallDirs = new Dictionary<Directions, WallScript>();
    private Dictionary<Directions, Directions> _correspondingDirs = new Dictionary<Directions, Directions>();
    */

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
        _wallDirs.Clear();
        _correspondingWallDirs.Clear();

        WallScript upWall = _walls[(int)Directions.Up];
        WallScript rightWall = _walls[(int)Directions.Right];
        WallScript downWall = _walls[(int)Directions.Down];
        WallScript leftWall = _walls[(int)Directions.Left];

        InitWall(Directions.Up,    new Vector3(        0,  _wallPos,    0),           Vector3.zero);
        InitWall(Directions.Right, new Vector3( _wallPos,         0,    0),  new Vector3(0, 0, 90));
        InitWall(Directions.Down,  new Vector3(        0, -_wallPos,    0),           Vector3.zero);
        InitWall(Directions.Left,  new Vector3(-_wallPos,         0,    0),  new Vector3(0, 0, 90));

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

}
