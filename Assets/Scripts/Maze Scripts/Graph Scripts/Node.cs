using System.Collections.Generic;
using UnityEngine;

public class Node
{
    /*
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
    */

    private TileScript correspondingTile;
    private Dictionary<Directions, Node> neighbours = new Dictionary<Directions, Node>();

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
    public List<Node> Neighbours
    {
        get { return new List<Node>(neighbours.Values); }
    }

    public Node(TileScript tile)
    {
        correspondingTile = tile;
    }

	public void AddNeighbour(Node neighbour, Directions direction)
    {
        neighbours.Add(direction, neighbour);
    }

    public bool HasNeighbourWithDirection(Directions direction)
    {
        return neighbours.ContainsKey(direction);
    }

    public Node NeighbourWithDirection(Directions direction)
    {
        if (HasNeighbourWithDirection(direction)) return neighbours[direction];
        else return null;
    }

    public int NumberNeighbours()
    {
        return Neighbours.Count;
    }

    public bool EquivalentTo(Node other)
    {
        return (this.X == other.X && this.Y == other.Y);
    }

    public override string ToString()
    {
        string nodeString = "(" + X + ", " + Y + ") with " + " neighbours: ";

        foreach (Node neighbour in Neighbours)
        {
            nodeString += "(" + neighbour.X + ", " + neighbour.Y + ") ";
        }

        return nodeString;
    }

}
