using UnityEngine;
using UnityEngine.UI;
using System;

public class TestScript : MonoBehaviour {

    public Maze MazeTemplate;
    public Text StackSizeText;
    public Text UnvisitedTilesText;
    public InputField HeightSetter;
    public InputField WidthSetter;

    Maze level;

	// Use this for initialization
	void Start () {
        level = Instantiate(MazeTemplate) as Maze;
        level.MakeBlankGrid(10, 10);
        level.stackCount = StackSizeText;
        level.setCount = UnvisitedTilesText;

        level.PrepareForGeneration();
    }
	
    public void Generate()
    {
        level.StartGeneration();
    }

    public void Reset()
    {
        level.ResetGrid();
    }

    public void ToggleAnim()
    {
        level.withDelay = !level.withDelay;
    }
	
    public void Destroy()
    {
        level.DestroyGrid();
    }

    public void UpdateGridDimensions()
    {
        int newHeight;
        int newWidth;

        bool isHeightNumeric = int.TryParse(HeightSetter.text, out newHeight);
        bool isWidthNumeric = int.TryParse(WidthSetter.text, out newWidth);

        if (!isHeightNumeric) newHeight = Maze.minHeight;
        else if (newHeight < Maze.minHeight) newHeight = Maze.minHeight;
        else if (newHeight > Maze.maxHeight) newHeight = Maze.maxHeight;

        if (!isWidthNumeric) newWidth = Maze.minWidth;
        else if (newWidth < Maze.minWidth) newWidth = Maze.minWidth;
        else if (newWidth > Maze.maxWidth) newWidth = Maze.maxWidth;

        if (newHeight == level.Height) Debug.Log("No change in height");
        if (newWidth == level.Width) Debug.Log("No change in width");

        level.MakeBlankGrid(newHeight, newWidth);
    }

}
