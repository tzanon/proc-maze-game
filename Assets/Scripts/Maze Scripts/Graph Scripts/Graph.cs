using System.Collections.Generic;
using UnityEngine;

public class Graph {

    public enum Directions { North, East, South, West, Origin };

    private Dictionary<Vector2, Node> _nodes;
    private HashSet<Edge> _edges;

    public Graph()
    {
        _nodes = new Dictionary<Vector2, Node>();
        _edges = new HashSet<Edge>();
        
    }

    public void Clear()
    {
        _nodes.Clear();
        _edges.Clear();
    }

	public void AddNode(Node node)
    {
        _nodes.Add(new Vector2(node.X, node.Y), node);
    }

    public Node GetNodeAtPoint(int x, int y)
    {
        return _nodes[new Vector2(x, y)];
    }

}
