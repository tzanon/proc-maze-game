using UnityEngine;
using System.Collections;

public class GameRunner : MonoBehaviour
{

    private Maze _level;
    private TileScript[,] grid;

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
    public GameObject Player;

	// Use this for initialization
	void Start()
    {
	    
	}
	
	// Handle player input
	void Update()
    {
	    
	}

    private void SetLevel(Maze maze)
    {
        Level = maze;
        grid = maze.Grid;
    }

    

}
