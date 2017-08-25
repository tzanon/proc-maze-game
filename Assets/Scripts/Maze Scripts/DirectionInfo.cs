using System.Collections.Generic;
using UnityEngine;

public static class DirectionInfo
{

    public static readonly Dictionary<Direction, Direction> oppositeDirections = new Dictionary<Direction, Direction>
    {
        { Direction.North, Direction.South },
        { Direction.East, Direction.West },
        { Direction.South, Direction.North },
        { Direction.West, Direction.East }
    };

    public static readonly Dictionary<Direction, Quaternion> directionQuaternions = new Dictionary<Direction, Quaternion>()
    {
        { Direction.North, Quaternion.Euler(0, 0, 0) },
        { Direction.East, Quaternion.Euler(0, 90, 0) },
        { Direction.South, Quaternion.Euler(0, 180, 0) },
        { Direction.West, Quaternion.Euler(0, 270, 0) }
    };

    public static readonly Dictionary<Direction, Vector3> directionRotations = new Dictionary<Direction, Vector3>()
    {
        { Direction.North, new Vector3(0, 0, 0) },
        { Direction.East, new Vector3(0, 90, 0) },
        { Direction.South, new Vector3(0, 180, 0) },
        { Direction.West, new Vector3(0, 270, 0) }
    };

    public static readonly Dictionary<Direction, int> directionAngles = new Dictionary<Direction, int>()
    {
        { Direction.North, 0 },
        { Direction.East, 90 },
        { Direction.South, 180 },
        { Direction.West, 270 }
    };

    public static readonly Dictionary<Direction, Vector3> directionVectors = new Dictionary<Direction, Vector3>()
    {
        { Direction.North, Vector3.up },
        { Direction.East, Vector3.right },
        { Direction.South, Vector3.down },
        { Direction.West, Vector3.left }
    };

}
