using UnityEngine;
using System.Collections;
using System;

public class GameRunner3D : GameRunner {

    private const float _playerHeight = 1f;

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
        _player.transform.position = Level.startTile.transform.position + new Vector3(0, _playerHeight, 0);
    }

    public override void EndGame()
    {
        Player.GetComponent<Camera>().enabled = false;
        base.EndGame();
    }

}
