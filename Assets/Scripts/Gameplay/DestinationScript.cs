using UnityEngine;

public class DestinationScript : MonoBehaviour
{

    [HideInInspector]
    public Maze Level;

    [HideInInspector]
    public GameRunner Runner;

    void OnTriggerEnter(Collider coll)
    {
        if (coll.tag == "Player")
        {
            Runner.EndGame();
        }
    }

}
