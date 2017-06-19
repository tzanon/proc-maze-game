﻿using System.Collections.Generic;
using UnityEngine;

public class Node
{

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

    // calculates distance this node and the other
    public int DistanceBetween(Node other)
    {
        int horizDistance = Mathf.Abs(this.X - other.X);
        int vertDistance = Mathf.Abs(this.Y - other.Y);
        return horizDistance + vertDistance;
    }

    // calculates direction from this node to the other
    public Directions DirectionBetween(Node other)
    {
        if (other.X - this.X > 0) return Directions.East;
        if (other.X - this.X < 0) return Directions.West;
        if (other.Y - this.Y > 0) return Directions.North;
        else return Directions.South;
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
