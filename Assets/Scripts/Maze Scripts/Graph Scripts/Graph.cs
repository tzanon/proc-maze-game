
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Graph
{
    private Dictionary<Vector2, Node> nodes;
    private HashSet<Edge> edges;
    private Node startNode, endNode;
    private NodePath shortestPath;

    public Node StartNode
    {
        get { return startNode; }
    }
    public Node EndNode
    {
        get { return endNode; }
    }
    public HashSet<Node> Nodes
    {
        get { return new HashSet<Node>(nodes.Values); }
    }
    public HashSet<Edge> Edges
    {
        get { return edges; }
    }
    public NodePath ShortestPath
    {
        get
        {
            if (shortestPath == null || shortestPath.Count <= 0)
            {
                shortestPath = AStarSearch(startNode, endNode);
            }
            return shortestPath;
        }
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
        shortestPath = null;
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

    public bool HasNodeAtPoint(int x, int y)
    {
        return nodes.ContainsKey(new Vector2(x, y));
    }

    public bool HasNodeAtPoint(float x, float y)
    {
        return HasNodeAtPoint((int)x, (int)y);
    }

    public Node GetNodeAtPoint(int x, int y)
    {
        return nodes[new Vector2(x, y)];
    }

    public Node GetNodeAtPoint(float x, float y)
    {
        return GetNodeAtPoint((int)x, (int)y);
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
        return node1.DistanceBetween(node2);
    }

    // concerned about rounding...
    private int EuclideanDistance(Node node1, Node node2)
    {
        return Mathf.RoundToInt(Mathf.Sqrt(Mathf.Pow(node2.X - node1.X, 2) + Mathf.Pow(node2.Y - node1.Y, 2)));
    }

    // finds the shortest path between two given nodes
    public NodePath AStarSearch(Node start, Node end)
    {
        HashSet<Node> openNodes = new HashSet<Node>();
        HashSet<Node> closedNodes = new HashSet<Node>();

        Dictionary<Node, int> nodeScores = new Dictionary<Node, int>();
        Dictionary<Node, int> nodePathCosts = new Dictionary<Node, int>();
        Dictionary<Node, Node> cameFrom = new Dictionary<Node, Node>();

        openNodes.Add(start);
        nodePathCosts.Add(start, 0);
        nodeScores.Add(start, ManhattanDistance(start, end));
        cameFrom.Add(start, null);

        do
        {
            Node currentNode = new List<Node>(openNodes)[0]; // get node with lowest score
            foreach (Node node in openNodes)
                if (nodeScores[node] <= nodeScores[currentNode]) currentNode = node;

            if (currentNode == end) // reconstruct the path and return it
            {
                HashSet<Node> keys = new HashSet<Node>(cameFrom.Keys);
                List<Node> path = new List<Node>();
                path.Add(currentNode);

                while (keys.Contains(currentNode) && cameFrom[currentNode] != null)
                {
                    currentNode = cameFrom[currentNode];
                    path.Add(currentNode);
                }

                path.Reverse();
                LinkedList<Node> linkedPath = new LinkedList<Node>(path);
                return new NodePath(linkedPath);
            }

            openNodes.Remove(currentNode); // rmv from open, add to closed
            closedNodes.Add(currentNode);
            
            List<Node> neighbours = currentNode.Neighbours;
            foreach (Node neighbour in neighbours) // for each of its neighbours
            {
                if (closedNodes.Contains(neighbour)) continue; // ignore neighbour if in closed list

                int pathCost = nodePathCosts[currentNode] + currentNode.DistanceBetween(neighbour);//info.Distance;
                int score = pathCost + ManhattanDistance(neighbour, end);

                if (!openNodes.Contains(neighbour)) // if not in open, add and compute score
                {
                    openNodes.Add(neighbour);

                    nodePathCosts.Add(neighbour, pathCost);
                    nodeScores.Add(neighbour, score);

                    cameFrom.Add(neighbour, currentNode);
                }

                // if in open, update best-cost path
                if (openNodes.Contains(neighbour) && pathCost < nodePathCosts[neighbour])
                {
                    //if (pathCost >= nodePathCosts[info.Neighbour]) continue; // not a better path

                    nodePathCosts.Add(neighbour, pathCost);
                    nodeScores.Add(neighbour, score);

                    cameFrom.Add(neighbour, currentNode);
                }
            }
        }
        while (openNodes.Count > 0);

        return null;
    }

    // displays the coordinates and neighbours of all nodes in the graph
    public void DisplayInfo()
    {
        Debug.Log("graph with " + Nodes.Count + " nodes and " + Edges.Count + " edges:");
        Debug.Log("start is " + startNode.ToString());
        Debug.Log("start's neighbour is " + startNode.Neighbours[0].ToString());
        //Debug.Log("another neighbour is " + startNode.Neighbours[0].Neighbours[0].ToString());

        foreach (Node node in Nodes)
        {
            Debug.Log(node.ToString());
        }
    }

    public List<Node> DepthFirstSearch(Node start)
    {
        Stack<Node> nodeStack = new Stack<Node>();
        List<Node> discovered = new List<Node>();

        nodeStack.Push(start);

        while (nodeStack.Count > 0)
        {
            Node node = nodeStack.Pop();
            if (!discovered.Contains(node))
            {
                discovered.Add(node);
                //node.CorrespondingTile.SearchVisit();
                Debug.Log("Discovered node " + node.ToString());
                foreach (Node neighbour in node.Neighbours) nodeStack.Push(neighbour);
            }
        }
        return discovered;
    }

    public List<Node> BreadthFirstSearch(Node start)
    {
        Queue<Node> nodeQueue = new Queue<Node>();
        List<Node> discovered = new List<Node>();

        nodeQueue.Enqueue(start);

        while (nodeQueue.Count > 0)
        {
            Node node = nodeQueue.Dequeue();
            if (!discovered.Contains(node))
            {
                discovered.Add(node);
                //node.CorrespondingTile.SearchVisit();
                Debug.Log("Discovered node " + node.ToString());
                foreach (Node neighbour in node.Neighbours) nodeQueue.Enqueue(neighbour);
            }
        }
        return discovered;
    }

}
