using System.Collections;
using UnityEngine;

delegate void MovementAction(Node node);

public abstract class BotController : EntityController
{
    protected NodePath botPath;

    private MovementAction currentAction;

    public NodePath BotPath
    {
        get { return botPath; }
        set { botPath = value; }
    }

    protected virtual void Start()
    {
        movementFactor = 2f;
        rotationFactor = 90f;
    }

    public void InitializeBot(Graph graph)
    {
        mazeGraph = graph;
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
