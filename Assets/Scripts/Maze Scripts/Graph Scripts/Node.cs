using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public int X, Y;
    public enum Directions { Up, Right, Down, Left, Centre };
    public Dictionary<Directions, Node> Neighbours;

    private Transform tf;

    void Start ()
    {
		
	}
	
	
}
