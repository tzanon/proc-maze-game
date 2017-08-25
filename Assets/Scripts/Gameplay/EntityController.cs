
using UnityEngine;

public abstract class EntityController : MonoBehaviour
{

    protected float movementFactor;
    protected float rotationFactor;

    protected Graph mazeGraph;

    protected Node currentNode;

    public Node CurrentNode
    {
        get { return currentNode; }
        set { currentNode = value; }
    }

    public Graph MazeGraph
    {
        get { return mazeGraph; }
        set { mazeGraph = value; }
    }

	protected virtual void Start ()
    {
        
	}

    protected virtual void Update ()
    {
		
	}

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.tag == "NodeTile")
        {
            TileScript tile = other.GetComponent<TileScript>();
            currentNode = mazeGraph.GetNodeAtPoint(tile.X, tile.Y);
            //Debug.Log("Current node is: " + currentNode.ToString());
        }
    }

    protected void UpdateCurrentNode(Node newCurrentNode)
    {
        currentNode = newCurrentNode;
    }

    protected void MoveForward()
    {
        transform.position += transform.forward * movementFactor * Time.deltaTime;
    }

    protected void MoveBackward()
    {
        transform.position -= transform.forward * movementFactor * Time.deltaTime;
    }

    protected void RotateRight()
    {
        transform.Rotate(0, rotationFactor * Time.deltaTime, 0);
    }

    protected void RotateLeft()
    {
        transform.Rotate(0, -rotationFactor * Time.deltaTime, 0);
    }

}
