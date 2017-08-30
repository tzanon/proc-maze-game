
using UnityEngine;

public abstract class EntityController : MonoBehaviour
{

    protected float movementFactor;
    protected float rotationFactor;

    public Graph mazeGraph;

    public Node currentNode;

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
