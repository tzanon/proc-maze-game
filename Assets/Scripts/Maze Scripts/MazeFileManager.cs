using UnityEngine;
using System;
using System.Collections;
using System.IO;
using System.Linq;

public class MazeFileManager : MonoBehaviour {

    Maze mazeScript;

	// Use this for initialization
	void Start () {
	
	}

    public void SaveMaze(Maze maze, string filename)
    {
        TileScript[,] grid = maze.Grid;

        int gridHeight = grid.GetLength(0);
        int gridWidth = grid.GetLength(1);

        string filePath = @"MazeFiles\" + filename + ".txt";
        StreamWriter mazeFile = new StreamWriter(filePath);

        mazeFile.WriteLine(gridHeight);
        mazeFile.WriteLine(gridWidth);

        mazeFile.WriteLine();

        for (int i = 0; i < gridHeight; i++)
        {
            for (int j = 0; j < gridWidth; j++)
            {
                string wallCode = grid[i, j].GetWallCode();
                mazeFile.WriteLine(wallCode);
            }
        }

        mazeFile.Close();

        Debug.Log("Saving file...");
    }

    public void DisplayMazeFiles()
    {
        /*
        string[] mazeFiles = Directory.GetFiles(@"MazeFiles\", ".txt")
            .Select(Path.GetFileName)
            .ToArray();
        */
    }

    // creates a grid from information read from a file
    public Maze LoadMaze(string fname)
    {
        string filePath = @"MazeFiles\" + fname + ".txt"; // this will need to be modified

        StreamReader mazeFile;
        Maze maze = Instantiate(mazeScript) as Maze;

        try
        {
            using (StreamReader mazeFileReader = new StreamReader(filePath))
            {
                Debug.Log("Loading file...");
                mazeFile = new StreamReader(filePath);

                // initialize a grid with the stored height and width
                int gridHeight = Int32.Parse(mazeFile.ReadLine());
                int gridWidth = Int32.Parse(mazeFile.ReadLine());
                maze.InitializeMaze(gridHeight, gridWidth);

                // set each tile's walls
                string[,] gridCodes = new string[gridHeight, gridWidth];
                for (int i = 0; i < gridHeight; i++)
                {
                    for (int j = 0; j < gridWidth; j++)
                    {
                        gridCodes[i, j] = mazeFile.ReadLine();
                    }
                }
                maze.CreateGridFromFileInfo(gridCodes);

                // set the start and end tile
            }
        }
        catch (IOException e)
        {
            Console.WriteLine("File not found.");
            Console.WriteLine(e.Message);
        }
        return maze;
    }


}
