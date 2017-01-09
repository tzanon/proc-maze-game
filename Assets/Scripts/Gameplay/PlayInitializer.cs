using UnityEngine;

public class PlayInitializer : MonoBehaviour {

    /* 
     * this script is run when the play scene starts
     * it will load the level and then run the game logic script
    */

    private PlayLevelManager manager;

	// Use this for initialization
	void Start () {
        manager = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<PlayLevelManager>();
        if (manager.filename == null)
        {
            // make random level
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
