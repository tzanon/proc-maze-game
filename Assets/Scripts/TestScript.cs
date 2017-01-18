using UnityEngine;
using UnityEngine.UI;
using System;

public class TestScript : MonoBehaviour {

    public Maze MazeTemplate;
    public GameRunner RunnerTemplate;
    public Text StackSizeText;
    public Text UnvisitedTilesText;
    public Text WinText;
    public InputField HeightSetter;
    public InputField WidthSetter;

    private Maze level;
    private GameRunner runner;

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
        if (runner != null) return;
        level.StartGeneration();
    }

    public void Reset()
    {
        if (runner != null) return;
        level.ResetGrid();
    }

    public void ToggleAnim()
    {
        level.withDelay = !level.withDelay;
    }
	
    public void Destroy()
    {
        if (runner != null) return;
        level.DestroyGrid();
    }

    public void UpdateGridDimensions()
    {
        if (runner != null) return;
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

    public void PlayLevel()
    {
        if (runner != null || !level.Playable()) return;

        runner = Instantiate(RunnerTemplate) as GameRunner;
        runner.Level = level;
        runner.WinText = WinText;
    }

    public void StopPlaying()
    {
        if (runner == null) return;
        runner.EndGame();
    }

}
