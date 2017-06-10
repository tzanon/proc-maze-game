using System.Collections.Generic;
using UnityEngine;

public class Graph {

    private Dictionary<Vector2, Node> nodes;
    private HashSet<Edge> edges;
    private Node startNode, endNode;

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
            Node currentNode = new List<Node>(openNodes)[0]; // 1. get node with lowest score
            foreach (Node node in openNodes)
                if (nodeScores[node] <= nodeScores[currentNode]) currentNode = node;

            if (currentNode == end)
            {
                // reconstruct the path and return it
                HashSet<Node> keys = new HashSet<Node>(cameFrom.Keys);
                LinkedList<Node> path = new LinkedList<Node>();
                path.AddLast(currentNode);

                while (keys.Contains(currentNode))
                {
                    currentNode = cameFrom[currentNode];
                    path.AddLast(currentNode);
                }
                return path;
            }

            openNodes.Remove(currentNode); // 2. rmv from open, add to closed
            closedNodes.Add(currentNode);

            List<Node.NeighbourInfo> infos = currentNode.NeighbourInfos();
            foreach (Node.NeighbourInfo info in infos) // 3. for each of its neighbours
            {
                if (closedNodes.Contains(info.Neighbour)) continue; // A. ignore neighbour if in closed list

                int pathCost = nodePathCosts[currentNode] + info.Distance;
                int score = pathCost + ManhattanDistance(info.Neighbour, end);

                if (!openNodes.Contains(info.Neighbour)) // B. if not in open, add and compute score
                {
                    openNodes.Add(info.Neighbour);

                    nodePathCosts.Add(info.Neighbour, pathCost);
                    nodeScores.Add(info.Neighbour, score);

                    cameFrom.Add(info.Neighbour, currentNode);
                }

                // C. if in open, update best-cost path
                if (openNodes.Contains(info.Neighbour) && pathCost < nodePathCosts[info.Neighbour])
                {
                    //if (pathCost >= nodePathCosts[info.Neighbour]) continue; // not a better path

                    nodePathCosts.Add(info.Neighbour, pathCost);
                    nodeScores.Add(info.Neighbour, score);

                    cameFrom.Add(info.Neighbour, currentNode);
                }
            }
            

        }
        while (openNodes.Count > 0);


        return null;
    }

}
