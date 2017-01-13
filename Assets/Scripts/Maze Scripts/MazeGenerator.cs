using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class MazeGenerator : MonoBehaviour
{

    private Maze _maze;

    public Maze Maze
    {
        get { return _maze; }
        private set { _maze = value; }
    }

    void Start()
    {
        
    }

    

    public void Test()
    {
        /*
        DestroyGrid();
        gridHeight = 5;
        gridWidth = 5;
        grid = new TileScript[gridHeight, gridWidth];
        CreateGrid();
        */
    }

}
