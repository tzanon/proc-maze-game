using System.Collections.Generic;
using UnityEngine;

public class Graph {

    private Dictionary<Vector2, Node> nodes;
    private HashSet<Edge> edges;
    private Node startNode, endNode;

    public HashSet<Node> Nodes
    {
        get { return new HashSet<Node>(nodes.Values); }
    }

    public HashSet<Edge> Edges
    {
        get { return edges; }
    }

    public Graph()
    {
        nodes = new Dictionary<Vector2, Node>();
        edges = new HashSet<Edge>();
    }

    public void Clear()
    {
        startNode = null;
        endNode = null;
        nodes.Clear();
        edges.Clear();
    }

	public void AddNode(Node node)
    {
        nodes.Add(new Vector2(node.X, node.Y), node);
    }

    public bool AddEdge(Edge edge)
    {
        foreach (Edge e in Edges)
        {
            if (edge.EquivalentTo(e)) return false;
        }
        edges.Add(edge);
        return true;
    }

    public Node GetNodeAtPoint(int x, int y)
    {
        return nodes[new Vector2(x, y)];
    }

    public void SetStartNodeAtPoint(int x, int y)
    {
        startNode = nodes[new Vector2(x, y)];
    }

    public void SetEndNodeAtPoint(int x, int y)
    {
        endNode = nodes[new Vector2(x, y)];
    }

    private int ManhattanDistance(Node node1, Node node2)
    {
        int horizDistance = Mathf.Abs(node1.X - node2.X);
        int vertDistance = Mathf.Abs(node1.Y - node2.Y);
        return horizDistance + vertDistance;
    }

    private float EuclideanDistance(Node node1, Node node2)
    {
        return Mathf.Sqrt(Mathf.Pow(node2.X - node1.X, 2) + Mathf.Pow(node2.Y - node1.Y, 2));
    }

    public LinkedList<Node> AStarSearch(Node start, Node end)
    {
        /*
         * look up computerphile video
         * A*
         * need 2 lists:
         * open list - nodes being considered for path
         * closed list - nodes not being considered again
         * start: add current pos to closed list, add all walkable neighbours (non null) to open list
         * give each node a score G + H
         *   G - cost from start to current node
         *   H - est. cost from curr node to destination (heuristic)
         * 
         */

        HashSet<Node> openNodes = new HashSet<Node>();
        HashSet<Node> closedNodes = new HashSet<Node>();
        closedNodes.Add(start);

        

        return null;
    }

}
