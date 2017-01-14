using UnityEngine;

public class PlayInitializer : MonoBehaviour {

    public Maze MazeTemplate;

    private PlayLevelManager manager;
    private Maze level;
    private GameRunner runner;
    private string filename;

	// Use this for initialization
	void Start()
    {
        TestRun();
	}


    private void TestRun()
    {
        level = Instantiate(MazeTemplate) as Maze;
        level.MakeRandomMaze();

        // make and activate game runner
        //runner = Instantiate(runner) as GameRunner;
        //runner.Level = level;

    }


    private void NormalRun()
    {
        //manager = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<PlayLevelManager>();
        level = Instantiate(level) as Maze;
        filename = null;

        if (filename == null) // make random level
        {
            level.MakeRandomMaze();
        }
        else // load level with the given name
        {
            level.LoadMaze(manager.filename);
        }

        // activate game runner
        runner = Instantiate(runner) as GameRunner;
        runner.Level = level;
    }

}
