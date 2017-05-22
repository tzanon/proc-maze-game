using System.Collections.Generic;
using UnityEngine;

public class Node
{

    private class NeighbourInfo
    {
        public Node _neighbour;
        private int _distance;

        public Node Neighbour
        {
            get { return _neighbour; }
        }
        public int Distance
        {
            get { return _distance; }
        }

        public NeighbourInfo()
        {
            _neighbour = null;
            _distance = int.MaxValue;
        }

        public void SetNeighbour(Node neighbour, int distance)
        {
            _neighbour = neighbour;
            _distance = distance;
        }

    }

    private TileScript _correspondingTile;
    private Dictionary<Directions, NeighbourInfo> NeighbourInfos = new Dictionary<Directions, NeighbourInfo>();

    private Vector3 _worldLocation;

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
    public Vector3 WorldLocation
    {
        get { return _correspondingTile.transform.position; }
    }

    public Node(TileScript tile)
    {
        _correspondingTile = tile;
        NeighbourInfos.Add(Directions.North, new NeighbourInfo());
        NeighbourInfos.Add(Directions.East, new NeighbourInfo());
        NeighbourInfos.Add(Directions.South, new NeighbourInfo());
        NeighbourInfos.Add(Directions.West, new NeighbourInfo());
    }

	public void AddNeighbour(Node neighbour, Directions direction, int distance)
    {
        NeighbourInfos[direction].SetNeighbour(neighbour, distance);
        
    }

    public bool HasNeighbour(Directions direction)
    {
        return NeighbourInfos[direction]._neighbour != null;
    }

    public Node GetNeighbour(Directions direction)
    {
        return NeighbourInfos[direction].Neighbour;
    }

    public override string ToString()
    {
        return "";
    }

}
