using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodePath
{
    private LinkedList<Node> path;
    private LinkedListNode<Node> target;

    public int Count
    {
        get { return path.Count; }
    }

    public List<Node> ListRepresentation
    {
        get { return new List<Node>(path); }
    }
    
    public Node Next
    {
        get
        {
            Node nextNode = target.Value;
            target = target.Next;
            return nextNode;
        }
    }

    public NodePath(LinkedList<Node> nodeSequence)
    {
        path = nodeSequence;
        target = path.First;
    }

    public NodePath(List<Node> nodeSequence)
    {
        path = new LinkedList<Node>(nodeSequence);
        target = path.First;
    }

    public bool HasNext()
    {
        return target != null;
    }

    public void Reset()
    {
        target = path.First;
    }

}
