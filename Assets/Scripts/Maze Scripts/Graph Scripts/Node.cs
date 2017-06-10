using System.Collections.Generic;
using UnityEngine;

public class Node
{

    public class NeighbourInfo
    {
        private Node neighbour;
        private int distance;

        public Node Neighbour
        {
            get { return neighbour; }
        }
        public int Distance
        {
            get { return distance; }
        }

        public NeighbourInfo()
        {
            neighbour = null;
            distance = int.MaxValue;
        }

        public NeighbourInfo(Node neighbour, int distance)
        {
            this.neighbour = neighbour;
            this.distance = distance;
        }

        public void SetNeighbour(Node neighbour, int distance)
        {
            this.neighbour = neighbour;
            this.distance = distance;
        }

    }

    private TileScript correspondingTile;
    private Dictionary<Directions, NeighbourInfo> neighbourInfos = new Dictionary<Directions, NeighbourInfo>();

    public int X
    {
        get { return correspondingTile.X; }
    }
    public int Y
    {
        get { return correspondingTile.Y; }
    }
    public TileScript CorrespondingTile
    {
        get { return correspondingTile; }
    }
    public Vector3 WorldLocation
    {
        get { return correspondingTile.transform.position; }
    }

    public Node(TileScript tile)
    {
        correspondingTile = tile;
    }

	public void AddNeighbour(Node neighbour, Directions direction, int distance)
    {
        neighbourInfos.Add(direction, new NeighbourInfo(neighbour, distance));
    }

    public bool HasNeighbourWithDirection(Directions direction)
    {
        return neighbourInfos.ContainsKey(direction);
    }

    public Node NeighbourWithDirection(Directions direction)
    {
        if (HasNeighbourWithDirection(direction)) return neighbourInfos[direction].Neighbour;
        else return null;
    }

    public List<NeighbourInfo> NeighbourInfos()
    {
        List<NeighbourInfo> neighbours = new List<NeighbourInfo>();
        foreach (NeighbourInfo info in neighbourInfos.Values)
            neighbours.Add(info);
        return neighbours;
    }

    public bool EquivalentTo(Node other)
    {
        return (this.X == other.X && this.Y == other.Y);
    }

    public override string ToString()
    {
        return "(" + X + ", " + Y + ") at " + WorldLocation;
    }

}
