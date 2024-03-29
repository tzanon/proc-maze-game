﻿using UnityEngine;

public class Edge {

    private Node _node1, _node2;
    private int _weight;
    

    public Node Node1
    {
        get { return _node1; }
        set
        {
            _node1 = value;
            CalculateWeight();
        }
    }
    public Node Node2
    {
        get { return _node2; }
        set
        {
            _node2 = value;
            CalculateWeight();
        }
    }
    public int Weight
    {
        get { return _weight; }
    }

    public Edge()
    {
        _weight = int.MaxValue;
    }

    public Edge(Node n1, Node n2)
    {
        _node1 = n1;
        _node2 = n2;
        CalculateWeight();
    }
	
    private void CalculateWeight()
    {
        if (Node1 == null || Node2 == null ||
            (Node1.X != Node2.X && Node1.Y != Node2.Y))
            _weight = int.MaxValue;

        else
        {
            if (Node1.X == Node2.X)
            {
                _weight = Mathf.Abs(Node1.Y - Node2.Y);
            }
            else
            {
                _weight = Mathf.Abs(Node1.X - Node2.X);
            }
        }
    }

    public Direction GetDirection()
    {
        if (Node1.X == Node2.X)
        {
            if (Node1.Y < Node2.Y) return Direction.North;
            else return Direction.South;
        }
        else
        {
            if (Node1.X < Node2.X) return Direction.East;
            else return Direction.West;
        }
    }
    
    public bool EquivalentTo(Edge other)
    {
        return (this.Node1.EquivalentTo(other.Node2) && this.Node2.EquivalentTo(other.Node1) ||
            this.Node1.EquivalentTo(other.Node1) && this.Node2.EquivalentTo(other.Node2));
    }

    public override string ToString()
    {
        return "Node1: " + Node1.ToString() + ", Node2: " + Node2.ToString() + ", Weight: " + Weight;
    }

}
