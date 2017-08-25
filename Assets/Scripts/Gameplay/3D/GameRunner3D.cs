using System;
using UnityEngine;

public class GameRunner3D : GameRunner {

    [HideInInspector]
    public PlayInitializer3D initializer;
    private const float actorHeight = 1f;

    protected override void Start ()
    {
        base.Start();
        Player.GetComponent<Camera>().enabled = true;
    }

    protected override void PlacePlayer()
    {
        Player.transform.position = Level.startTile.transform.position + new Vector3(0, actorHeight, 0);
    }

    protected override void PlaceDestintation()
    {
        Destination = Instantiate(destTemplate, EndTile.transform.position + new Vector3(0, 1, 0), Quaternion.identity) as DestinationScript;
        Destination.transform.parent = EndTile.transform;
        Destination.Level = this.Level;
        Destination.Runner = this;
    }

    protected override void PlacePowerups()
    {
        
    }

    protected override void PlaceEnemies()
    {
        
    }

    public override void EndGame()
    {
        Player.GetComponent<Camera>().enabled = false;
        Destroy(Destination.gameObject);
        base.EndGame();
        initializer.StopPlaying();
    }

}
