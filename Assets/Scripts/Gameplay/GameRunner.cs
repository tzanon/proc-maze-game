using UnityEngine;
using System.Collections;

public class GameRunner : MonoBehaviour
{

    private Maze _level;

    public Maze Level
    {
        get { return _level; }
        set
        {
            if (!value.Playable())
                Debug.LogError("This level is not playable.", value);
            else
                _level = value;
        }
    }

	// Use this for initialization
	void Start()
    {
	    
	}
	
	// Handle player input
	void Update()
    {
	    
	}

    

}
