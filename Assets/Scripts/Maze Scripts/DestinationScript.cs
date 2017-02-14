using UnityEngine;

public class DestinationScript : MonoBehaviour {

    [HideInInspector]
    public Maze Level;
    [HideInInspector]
    public GameRunner3D Runner;

	// Use this for initialization
	void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider coll)
    {
        if (coll.tag == "Player")
        {
            Runner.EndGame();
        }
    }



}
