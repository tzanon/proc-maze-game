
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Graph
{

    private Dictionary<Vector2, Node> nodes;
    private HashSet<Edge> edges;
    private Node startNode, endNode;
    private const float delay = 0.2f;

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

    public int DistanceBetweenNodes(Node node1, Node node2)
    {
        return ManhattanDistance(node1, node2);
    }

    private int ManhattanDistance(Node node1, Node node2)
    {
        int horizDistance = Mathf.Abs(node1.X - node2.X);
        int vertDistance = Mathf.Abs(node1.Y - node2.Y);
        return horizDistance + vertDistance;
    }

    // concerned about rounding...
    private int EuclideanDistance(Node node1, Node node2)
    {
        return Mathf.RoundToInt(Mathf.Sqrt(Mathf.Pow(node2.X - node1.X, 2) + Mathf.Pow(node2.Y - node1.Y, 2)));
    }

    public LinkedList<Node> AStarSearch(Node start, Node end)
    {
        /*
         * look up computerphile video
         * A*
         * need 2 lists:
         * open list - nodes being considered for path
         * closed list - nodes not being considered again
         * give each node a score G + H
         *   G - cost from start to current node
         *   H - est. cost from curr node to destination (heuristic)
         * 
         */

        Debug.Log("graph before searching");
        DisplayInfo();

        HashSet<Node> openNodes = new HashSet<Node>();
        HashSet<Node> closedNodes = new HashSet<Node>();

        Dictionary<Node, int> nodeScores = new Dictionary<Node, int>();
        Dictionary<Node, int> nodePathCosts = new Dictionary<Node, int>();
        Dictionary<Node, Node> cameFrom = new Dictionary<Node, Node>();

        

        openNodes.Add(start);
        nodePathCosts.Add(start, 0);
        nodeScores.Add(start, ManhattanDistance(start, end));
        cameFrom.Add(start, null);
        start.CorrespondingTile.MakeOpen();
        Debug.Log("opened start node " + start.ToString());

        do
        {
            Node currentNode = new List<Node>(openNodes)[0]; // 1. get node with lowest score
            foreach (Node node in openNodes)
                if (nodeScores[node] <= nodeScores[currentNode]) currentNode = node;

            if (currentNode == end)
            {
                // reconstruct the path and return it
                HashSet<Node> keys = new HashSet<Node>(cameFrom.Keys);
                LinkedList<Node> path = new LinkedList<Node>();
                path.AddLast(currentNode);

                while (keys.Contains(currentNode) && currentNode != null)
                {
                    currentNode = cameFrom[currentNode];
                    path.AddLast(currentNode);
                }
                return path;
            }

            openNodes.Remove(currentNode); // 2. rmv from open, add to closed
            closedNodes.Add(currentNode);
            currentNode.CorrespondingTile.MakeClosed();
            Debug.Log("closed node " + currentNode.ToString());

            
            List<Node> neighbours = currentNode.Neighbours;
            foreach (Node neighbour in neighbours) // 3. for each of its neighbours
            {
                if (closedNodes.Contains(neighbour)) continue; // A. ignore neighbour if in closed list

                int pathCost = nodePathCosts[currentNode] + DistanceBetweenNodes(currentNode, neighbour);//info.Distance;
                int score = pathCost + ManhattanDistance(neighbour, end);

                if (!openNodes.Contains(neighbour)) // B. if not in open, add and compute score
                {
                    openNodes.Add(neighbour);
                    neighbour.CorrespondingTile.MakeOpen();
                    Debug.Log("opened node " + neighbour.ToString());

                    nodePathCosts.Add(neighbour, pathCost);
                    nodeScores.Add(neighbour, score);

                    cameFrom.Add(neighbour, currentNode);
                }

                // C. if in open, update best-cost path
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

        Debug.Log("-----------------------------------");
        Debug.Log("search ended");

        return null;
    }

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

    public void DepthFirstSearch(Node startNode)
    {
        Stack<Node> nodeStack = new Stack<Node>();
        Dictionary<Node, bool> discovered = new Dictionary<Node, bool>();

        nodeStack.Push(startNode);

        while (nodeStack.Count > 0)
        {
            Node node = nodeStack.Pop();
            if (!discovered.ContainsKey(node))
            {
                discovered.Add(node, true);
                node.CorrespondingTile.SearchVisit();
                Debug.Log("Discovered node " + node.ToString());
                foreach (Node neighbour in node.Neighbours) nodeStack.Push(neighbour);
            }
        }
    }

}
