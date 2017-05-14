using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public enum Directions { North, East, South, West, Origin };
    private TileScript _correspondingTile;
    public Dictionary<Directions, Node> Neighbours;
    public Dictionary<Directions, Edge> IncidentEdges;

    private Transform tf;

    public int X
    {
        get { return _correspondingTile.X; }
    }
    public int Y
    {
        get { return _correspondingTile.Y; }
    }
    public TileScript CorrespondingTile
    {
        get { return _correspondingTile; }
    }

    public Node(TileScript tile)
    {
        _correspondingTile = tile;
    }
	
	public void AddNeighbour()
    {

    }

}
