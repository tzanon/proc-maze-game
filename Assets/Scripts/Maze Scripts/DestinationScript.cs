using UnityEngine;
using System.Collections;

public class DestinationScript : MonoBehaviour {

    public Maze Level;
    public GameRunner3D Runner;
    public TileScript Tile;


	// Use this for initialization
	void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider coll)
    {
        Runner.EndGame();
    }



}
