using UnityEngine;
using System.Collections;
using System;

public class GameRunner3D : GameRunner {

    [HideInInspector]
    public PlayInitializer3D initializer;

    private const float _actorHeight = 1f;

    // Use this for initialization
    protected override void Start ()
    {
        base.Start();
        Player.GetComponent<Camera>().enabled = true;
    }

    // Update is called once per frame
    void Update ()
    {
	    
	}

    protected override void PlacePlayer()
    {
        Player.transform.position = Level.startTile.transform.position + new Vector3(0, _actorHeight, 0);
    }

    protected override void PlaceDestintation()
    {
        Destination = Instantiate(DestTemplate, EndTile.transform.position + new Vector3(0, 1, 0), Quaternion.identity) as DestinationScript;
        Destination.transform.parent = EndTile.transform;
        Destination.Level = this.Level;
        Destination.Runner = this;
    }

    public override void EndGame()
    {
        Player.GetComponent<Camera>().enabled = false;
        Destroy(Destination.gameObject);
        base.EndGame();
        initializer.StopPlaying();
    }

}
