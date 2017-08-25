using System.Collections;
using System.Collections.Generic;
using UnityEngine;

delegate void MovementAction(Node node);

public abstract class BotController : EntityController
{
    // make a separate class for following a path???
    protected NodePath botPath;
    //private LinkedListNode<Node> nextInPath;

    private MovementAction currentAction;

    public NodePath BotPath
    {
        get { return botPath; }
        set { botPath = value; }
    }

    protected override void Start ()
    {
        movementFactor = 2f;
        rotationFactor = 90f;
    }

    protected override void Update()
    {
        //if (transform.position == )
    }

    public void InitializeBot(Graph graph)
    {
        MazeGraph = graph;
        BotPath = graph.ShortestPath;
    }

    protected virtual void PathEndAction()
    {
        botPath.Reset();
    }

    protected void CalculatePath(Node destNode)
    {
        botPath = mazeGraph.AStarSearch(currentNode, destNode);
    }

    public void TraversePath()
    {

        StartCoroutine(JumpPath());

        /*
        if (this.transform.position != nextInPath.Value.WorldLocation)
        {
            Vector3.MoveTowards(transform.position, nextInPath.Value.WorldLocation, movementFactor * Time.deltaTime);
        }
        */
    }

    protected IEnumerator JumpPath()
    {
        while (botPath != null && botPath.HasNext())
        {
            transform.position = botPath.Next.WorldLocation;
            yield return new WaitForSeconds(0.5f);
        }

        PathEndAction();
    }

    protected void MoveToNode(Node node)
    {
        Direction nodeDir = currentNode.DirectionBetween(node);
    }

    protected void RotateToFaceNode(Node node)
    {
        Direction nodeDir = currentNode.DirectionBetween(node);

        if (this.transform.rotation.eulerAngles.y - DirectionInfo.directionRotations[nodeDir].y < 180)
        {

        }
        else if (this.transform.rotation.eulerAngles.y - DirectionInfo.directionRotations[nodeDir].y >= 180)
        {

        }
    }

}
