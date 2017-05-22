using System.Collections.Generic;
using UnityEngine;

public class Graph {

    private Dictionary<Vector2, Node> _nodes;
    private HashSet<Edge> _edges;

    public HashSet<Node> Nodes
    {
        get { return new HashSet<Node>(_nodes.Values); }
    }

    public HashSet<Edge> Edges
    {
        get { return _edges; }
    }

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

    public void AddEdge(Edge edge)
    {
        foreach (Edge e in Edges)
        {
            if (edge.EquivalentTo(e)) return;
        }
        _edges.Add(edge);
    }

    public Node GetNodeAtPoint(int x, int y)
    {
        return _nodes[new Vector2(x, y)];
    }

}
