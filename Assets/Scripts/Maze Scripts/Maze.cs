
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

/*
 * C# naming conventions
 * camelCase for member vars, parameters, local vars
 * PascalCase for function, property, event, class names
 * prefix interface names with I
 * don't prefix enums, classes, delegates with letters
 */

// a class to store information about a maze object
public class Maze : MonoBehaviour
{
    public TileScript tileTemplate;
    private TileScript[,] grid;
    [HideInInspector]
    public TileScript startTile, endTile;
    public SolverController solverTemplate;

    public Material startMaterial, endMaterial, searchMaterial, discoveredMaterial, nodeMaterial;

    public int Height
    {
        get
        {
            if (grid != null) return grid.GetLength(0);
            else return -1;
        }
    }
    public int Width
    {
        get
        {
            if (grid != null) return grid.GetLength(1);
            else return -1;
        }
    }
    public TileScript[,] Grid
    {
        get { return grid; }
        private set { grid = value; }
    }

    [HideInInspector]
    public const int minHeight = 6, minWidth = 6, maxHeight = 32, maxWidth = 32;

    public bool withDelay;

    #region graph variables

    public MeshRenderer nodeMarker, edgeMarker;
    private Dictionary<Vector2, MeshRenderer> nodeMarkers = new Dictionary<Vector2, MeshRenderer>();
    private HashSet<MeshRenderer> edgeMarkers = new HashSet<MeshRenderer>();

    private Graph graphRepresentation = new Graph();
    private HashSet<TileScript> graphTiles = new HashSet<TileScript>(); //tiles representing nodes in a graph
    private HashSet<TileScript> terminalTiles = new HashSet<TileScript>(); //tiles with exactly 3 walls
    private static Dictionary<Direction, Vector2> directionVectors = new Dictionary<Direction, Vector2>
    {
        { Direction.North, new Vector2(0, 1) },
        { Direction.East, new Vector2(1, 0) },
        { Direction.South, new Vector2(0, -1) },
        { Direction.West, new Vector2(-1, 0) }
    };

    private Coroutine searchRoutine;

    public Graph GraphRepresentation
    {
        get { return graphRepresentation; }
    }

    #endregion

    #region generation variables

    public Text stackCount;
    public Text setCount;

    private const float delay = 0.1f; // delay for "animation"
    private bool isGenerating;
    private Stack<TileScript> tileStack = new Stack<TileScript>();
    private HashSet<TileScript> unvisitedTiles = new HashSet<TileScript>();

    #endregion

    void Start()
    {
        // maze generation things
        UpdateStackCount();
        UpdateUnvisitedTiles();
        isGenerating = false;
        withDelay = true;
    }

    #region general utility methods

    public void MakeSmallMaze()
    {
        MakeBlankGrid(minHeight, minWidth);

        StartGeneration();
    }

    public void MakeMedMaze()
    {
        MakeBlankGrid(maxHeight / 2, maxWidth / 2);

        StartGeneration();
    }

    public void MakeLargeMaze()
    {
        MakeBlankGrid(maxHeight, maxWidth);

        StartGeneration();
    }

    // makes a new grid from scratch and generates a maze in it
    public void MakeRandomMaze()
    {
        int randHeight = UnityEngine.Random.Range(minHeight, maxHeight);
        int randWidth = UnityEngine.Random.Range(minWidth, maxWidth);

        MakeBlankGrid(randHeight, randWidth);

        StartGeneration();
    }

    // creates a blank grid in preparation for generation or editing
    public void MakeBlankGrid(int gridHeight, int gridWidth)
    {
        Debug.Log("making the codes...");
        string[,] gridCodes = new string[gridHeight, gridWidth];

        for (int i = 0; i < gridHeight; i++)
        {
            for (int j = 0; j < gridWidth; j++)
            {
                gridCodes[i, j] = "1111";
            }
        }

        MakeGridFromCodes(gridCodes);
        MarkAllTilesUnvisited();
        PrepareForGeneration();
    }

    // "fills" the grid by instantiating each tile with properties specified
    public void MakeGridFromCodes(string[,] gridCodes)
    {
        DestroyGrid(); // previous grid's tiles will be removed
        Grid = new TileScript[gridCodes.GetLength(0), gridCodes.GetLength(1)];
        Debug.Log("new height is " + Height);
        Debug.Log("new width is " + Width);

        Vector3 position = tileTemplate.StartingPosition;
        Vector3 xIncr = tileTemplate.HorizontalSpawnIncrement;
        Vector3 yIncr = tileTemplate.VerticalSpawnIncrement;

        for (int i = 0; i < Height; i++)
        {
            for (int j = 0; j < Width; j++)
            {
                TileScript newTile = Grid[i, j] = Instantiate(tileTemplate, position, Quaternion.identity) as TileScript;
                newTile.X = j;
                newTile.Y = i;
                newTile.SetWallsFromCode(gridCodes[i, j]);

                position += xIncr;
            }
            position.x = tileTemplate.StartingPosition.x;
            position += yIncr;
        }
        // set start and end tile now
    }

    // destroys every tile in the grid
    public void DestroyGrid()
    {
        if (Grid == null) return;

        for (int i = 0; i < Grid.GetLength(0); i++)
        {
            for (int j = 0; j < Grid.GetLength(1); j++)
            {
                if (Grid[i, j] != null) Destroy(Grid[i, j].gameObject);
            }
        }

        // reset utiity data structures
        tileStack.Clear();
        unvisitedTiles.Clear();

        // update GUI display
        UpdateStackCount();
        UpdateUnvisitedTiles();
    }

    // determines if the given coordinates are within the grid's bounds
    public bool InBounds(int x, int y)
    {
        return (
            x >= 0 &&
            x < Grid.GetLength(1) &&
            y >= 0 &&
            y < Grid.GetLength(0)
            );
    }

    // checks if the given tile is in this grid
    // DEBUGGING: try this with tile.gameObject and with just tile
    public bool GridContains(TileScript tile)
    {
        if (tile == null || !GridExists()) return false;

        foreach (TileScript gridTile in Grid)
            if (gridTile.gameObject == tile.gameObject) return true;

        return false;
    }

    // the grid exists only if all of its tiles have been instantiated
    // DEBUGGING: try this with tile.gameObject and with just tile
    public bool GridExists()
    {
        if (Grid == null) return false;

        foreach (TileScript tile in Grid)
            if (tile.gameObject == null) return false;

        return true;
    }

    // maze is playable only if its start and end tiles are defined
    public bool Playable()
    {
        return (!isGenerating && GridContains(startTile) && GridContains(endTile));
    }

    // spawns a bot to solve the maze
    public void SolveMaze()
    {
        SolverController solver = Instantiate(solverTemplate, startTile.transform.position, Quaternion.identity);
        solver.InitializeBot(graphRepresentation);
        solver.TraversePath();
    }

    public void DisplayCoordinates()
    {
        foreach (TileScript tile in Grid)
        {
            Debug.Log("(" + tile.X + ", " + tile.Y + ")");
        }
    }

    #endregion

    #region maze generation methods

    // debugging GUI update functions
    private void UpdateStackCount()
    {
        if (stackCount != null)
            stackCount.text = "Tiles on stack: " + tileStack.Count;
    }
    private void UpdateUnvisitedTiles()
    {
        if (setCount != null)
            setCount.text = "Unvisited tiles: " + unvisitedTiles.Count;
    }

    // reset the grid to a blank state
    public void ResetGrid()
    {
        if (isGenerating || !GridExists()) return;

        // reset the grid's walls
        for (int i = 0; i < Grid.GetLength(0); i++)
        {
            for (int j = 0; j < Grid.GetLength(1); j++)
            {
                Grid[i, j].ResetTile();
            }
        }

        PrepareForGeneration();
    }

    // sets the data structures used in the generation process
    public void PrepareForGeneration()
    {
        HideGraph();
        ShowMaze();

        // reset utiity data structures
        tileStack.Clear();
        unvisitedTiles.Clear();
        graphTiles.Clear();
        terminalTiles.Clear();
        graphRepresentation.Clear();

        // mark all tiles as unvisited
        MarkAllTilesUnvisited();

        // update GUI display
        UpdateStackCount();
        UpdateUnvisitedTiles();
    }

    // marks all tiles in the grid as unvisited
    private void MarkAllTilesUnvisited()
    {
        for (int i = 0; i < Grid.GetLength(0); i++)
        {
            for (int j = 0; j < Grid.GetLength(1); j++)
            {
                TileScript tile = Grid[i, j];
                tile.UnVisit();
                unvisitedTiles.Add(tile);
            }
        }
    }

    // starts the generation process
    public void StartGeneration()
    {
        if (!isGenerating) StartCoroutine(GenerateMaze());
    }

    // generates a maze using randomized depth-first search with backtracking
    // ***DO NOT MODIFY THE DFS***
    private IEnumerator GenerateMaze()
    {
        /*
         Generate Maze with Backtracking (from Wikipedia):
            1. Make the initial cell the current cell and mark it as visited
            2. While there are unvisited cells
                1. If the current cell has any neighbours which have not been visited
                    1. Choose randomly one of the unvisited neighbours
                    2. Push the current cell to the stack
                    3. Remove the wall between the current cell and the chosen cell
                    4. Make the chosen cell the current cell and mark it as visited
                2. Else if stack is not empty
                    1. Pop a cell from the stack
                    2. Make it the current cell
                3. Else
                    1. Pick a random unvisited cell, make it the current cell and mark it as visited
         */

        PrepareForGeneration();

        isGenerating = true;

        int initY = 0;
        int initX = UnityEngine.Random.Range(0, Grid.GetLength(1));

        TileScript currentTile = Grid[initY, initX];
        unvisitedTiles.Remove(currentTile);
        UpdateUnvisitedTiles();
        currentTile.Visit();

        while (unvisitedTiles.Count > 0)
        {
            if (withDelay) yield return new WaitForSeconds(delay);

            List<TileScript> unvisitedNeighbours = GetUnvisitedNeighbours(currentTile);

            if (unvisitedNeighbours.Count > 0)
            {
                int neighbourNum = UnityEngine.Random.Range(0, unvisitedNeighbours.Count);

                TileScript neighbourTile = unvisitedNeighbours[neighbourNum];
                tileStack.Push(currentTile);
                UpdateStackCount();
                currentTile.OnStack();

                currentTile.RemoveWallsBetweenTiles(neighbourTile);

                currentTile = neighbourTile;
                unvisitedTiles.Remove(currentTile);
                UpdateUnvisitedTiles();
                currentTile.Visit();
            }
            else if (tileStack.Count > 0)
            {
                currentTile.Visit();

                currentTile = tileStack.Pop();
                UpdateStackCount();
            }
            else
            {
                TileScript[] unvisited = new TileScript[unvisitedTiles.Count];
                unvisitedTiles.CopyTo(unvisited);
                int rand = UnityEngine.Random.Range(0, unvisited.GetLength(0));

                currentTile = unvisited[rand];
                unvisitedTiles.Remove(currentTile);
                UpdateUnvisitedTiles();
            }

        }

        while (tileStack.Count > 0)
        {
            if (withDelay) yield return new WaitForSeconds(delay);
            currentTile.Visit();

            currentTile = tileStack.Pop();
            UpdateStackCount();
        }

        if (withDelay) yield return new WaitForSeconds(delay);
        currentTile.Visit();

        startTile = Grid[initY, initX];
        startTile.MakeStartTile();

        int endY = -1;
        int endX = -1;

        /*
        // select some random tile at the last row for end tile
        endY = Grid.GetLength(0) - 1;
        endX = UnityEngine.Random.Range(0, Grid.GetLength(1));
        */

        // preferably, select a tile in the 1st or 2nd last rows with 3 walls for end tile
        for (int i = Grid.GetLength(0) - 2; i < Grid.GetLength(0); i++)
        {
            for (int j = 0; j < Grid.GetLength(1); j++)
            {
                if (Grid[i,j].GetNumActiveWalls() == 3)
                {
                    endY = i;
                    endX = j;
                }
            }
        }

        // if not, any node
        if (endY < 0 || endX < 0)
        {
            for (int i = Grid.GetLength(0) - 2; i < Grid.GetLength(0); i++)
            {
                for (int j = 0; j < Grid.GetLength(1); j++)
                {
                    if (Grid[i, j].GetNumActiveWalls() != 2)
                    {
                        endY = i;
                        endX = j;
                    }
                }
            }
        }
        
        endTile = Grid[endY, endX];
        endTile.MakeEndTile();

        FindNodes();
        MakeGraph();
        
        isGenerating = false;
    }

    // gets all of the unvisited neighbour tiles of the given tile
    public List<TileScript> GetUnvisitedNeighbours(TileScript currentTile)
    {
        List<TileScript> unvisitedNeighbours = new List<TileScript>();
        int x = currentTile.X;
        int y = currentTile.Y;

        if (InBounds(x + 1, y) && unvisitedTiles.Contains(Grid[y, x + 1])) unvisitedNeighbours.Add(Grid[y, x + 1]);
        if (InBounds(x - 1, y) && unvisitedTiles.Contains(Grid[y, x - 1])) unvisitedNeighbours.Add(Grid[y, x - 1]);
        if (InBounds(x, y + 1) && unvisitedTiles.Contains(Grid[y + 1, x])) unvisitedNeighbours.Add(Grid[y + 1, x]);
        if (InBounds(x, y - 1) && unvisitedTiles.Contains(Grid[y - 1, x])) unvisitedNeighbours.Add(Grid[y - 1, x]);

        return unvisitedNeighbours;
    }

    #endregion

    #region graph-related methods

    public void HideMaze()
    {
        foreach (TileScript tile in Grid)
        {
            tile.gameObject.SetActive(false);
        }
    }

    public void ShowMaze()
    {
        HideGraph();

        foreach (TileScript tile in Grid)
        {
            tile.gameObject.SetActive(true);
        }
    }

    public void HideGraph()
    {
        if (searchRoutine != null) StopCoroutine(searchRoutine);

        if (nodeMarkers == null) return;
        foreach (MeshRenderer marker in nodeMarkers.Values)
        {
            Destroy(marker.gameObject);
        }
        nodeMarkers.Clear();

        if (edgeMarkers == null) return;
        foreach (MeshRenderer marker in edgeMarkers)
        {
            Destroy(marker.gameObject);
        }
        edgeMarkers.Clear();
    }

    public void ShowGraph()
    {
        HideMaze();

        Node[] graphNodes = new Node[graphRepresentation.Nodes.Count];
        graphRepresentation.Nodes.CopyTo(graphNodes);

        for (int i = 0; i < graphNodes.Length; i++)
        {
            MeshRenderer marker = Instantiate(nodeMarker, graphNodes[i].WorldLocation, Quaternion.identity);
            nodeMarkers.Add(new Vector2(graphNodes[i].X, graphNodes[i].Y), marker);
        }
        nodeMarkers[new Vector2(startTile.X, startTile.Y)].material = startMaterial;
        nodeMarkers[new Vector2(endTile.X, endTile.Y)].material = endMaterial;

        Edge[] graphEdges = new Edge[graphRepresentation.Edges.Count];
        graphRepresentation.Edges.CopyTo(graphEdges);

        for (int i = 0; i < graphEdges.Length; i++)
        {
            if (graphEdges[i].GetDirection() == Direction.North)
            {
                DrawEdge(graphEdges[i], Vector3.forward, Quaternion.identity);
            }
            else if (graphEdges[i].GetDirection() == Direction.South)
            {
                DrawEdge(graphEdges[i], Vector3.back, Quaternion.identity);
            }
            else if (graphEdges[i].GetDirection() == Direction.East)
            {
                DrawEdge(graphEdges[i], Vector3.right, Quaternion.Euler(0, 90, 0));
            }
            else
            {
                DrawEdge(graphEdges[i], Vector3.left, Quaternion.Euler(0, 90, 0));
            }
        }
    }

    private void DrawEdge(Edge edge, Vector3 increment, Quaternion rotation)
    {
        Vector3 spawnLocation = edge.Node1.WorldLocation + increment;
        for (int j = 0; j < edge.Weight; j++)
        {
            edgeMarkers.Add(Instantiate(edgeMarker, spawnLocation, rotation));
            spawnLocation += 1.8f * increment;
        }
    }

    private void FindNodes()
    {
        graphTiles.Clear();
        terminalTiles.Clear();
        graphRepresentation.Clear();

        foreach (TileScript tile in Grid)
        {
            if (!TileScript.CorridorCodes.Contains(tile.GetWallCode()))
            {
                SetTileAsNode(tile);

                Node node = new Node(tile);
                graphRepresentation.AddNode(node);
                if (tile.GetNumActiveWalls() == 3) terminalTiles.Add(tile);
            }
        }

        graphRepresentation.SetStartNodeAtPoint(startTile.X, startTile.Y);
        graphRepresentation.SetEndNodeAtPoint(endTile.X, endTile.Y);
    }

    private void SetTileAsNode(TileScript tile)
    {
        graphTiles.Add(tile);
        tile.tag = "NodeTile";
        BoxCollider playerDetector = tile.gameObject.AddComponent<BoxCollider>();
        playerDetector.isTrigger = true;
        playerDetector.center = new Vector3(0, 4, 0);
        playerDetector.size = new Vector3(1, 8, 1);
    }

    private void MakeGraph()
    {
        foreach (Node node in graphRepresentation.Nodes)
        {
            FindNodeNeighbours(node);
        }
        //Debug.Log("edges made: " + createdEdges + ", added: " + addedEdges);
        //graphRepresentation.DisplayInfo();
    }

    private void FindNodeNeighbours(Node node)
    {

        foreach (Direction dir in Enum.GetValues(typeof(Direction)))
        {
            if (node.CorrespondingTile.WallActive(dir) || node.HasNeighbourWithDirection(dir)) continue;

            Vector2 incrVector = directionVectors[dir];
            Vector2 neighbourCoordinates = new Vector2(node.X, node.Y);

            do
            {
                neighbourCoordinates += incrVector;
            } while (!graphRepresentation.HasNodeAtPoint(neighbourCoordinates.x, neighbourCoordinates.y));

            Node neighbour = graphRepresentation.GetNodeAtPoint(neighbourCoordinates.x, neighbourCoordinates.y);
            node.AddNeighbour(neighbour, dir);
            neighbour.AddNeighbour(node, TileScript.CorrespondingDirs[dir]);

            Edge edge = new Edge(node, neighbour);
            graphRepresentation.AddEdge(edge);
        }
    }

    public void SolveGraph()
    {
        NodePath path = graphRepresentation.ShortestPath;
        searchRoutine = StartCoroutine(AnimSearch(path.ListRepresentation));
    }

    public void DFS()
    {
        List<Node> searchSequence = graphRepresentation.DepthFirstSearch(graphRepresentation.StartNode);
        searchRoutine = StartCoroutine(AnimSearch(searchSequence));
    }

    public void BFS()
    {
        List<Node> searchSequence = graphRepresentation.BreadthFirstSearch(graphRepresentation.StartNode);
        searchRoutine = StartCoroutine(AnimSearch(searchSequence));
    }

    private IEnumerator AnimSearch(List<Node> searchSequence)
    {

        float searchDelay = Mathf.Log(searchSequence.Count, 10) / searchSequence.Count * 10f;
        HashSet<Node> used = new HashSet<Node>();
        HashSet<Node> discovered = new HashSet<Node>();

        foreach (Node node in searchSequence)
        {
            nodeMarkers[new Vector2(node.X, node.Y)].material = searchMaterial;
            used.Add(node);

            yield return new WaitForSeconds(searchDelay);

            foreach (Node neighbour in node.Neighbours)
            {
                if (!used.Contains(neighbour))
                {
                    nodeMarkers[new Vector2(neighbour.X, neighbour.Y)].material = discoveredMaterial;
                    discovered.Add(neighbour);
                }
            }

            yield return new WaitForSeconds(searchDelay);
        }

        yield return new WaitForSeconds(10 * searchDelay);

        foreach (Node node in discovered)
        {
            nodeMarkers[new Vector2(node.X, node.Y)].material = nodeMaterial;
        }
        nodeMarkers[new Vector2(startTile.X, startTile.Y)].material = startMaterial;
        nodeMarkers[new Vector2(endTile.X, endTile.Y)].material = endMaterial;
    }

    #endregion

    #region file IO methods

    // saves the current maze's info to a text file
    public void SaveMaze(string filename)
    {
        if (startTile == null || endTile == null || isGenerating || !GridExists()) return;

        string filePath = @"Assets\MazeFiles\" + filename + ".txt";
        StreamWriter mazeFile = new StreamWriter(filePath);

        Debug.Log("Saving file...");

        int gridHeight = Grid.GetLength(0);
        int gridWidth = Grid.GetLength(1);

        Vector2 startPos = new Vector2(startTile.X, startTile.Y);
        Vector2 endPos = new Vector2(endTile.X, endTile.Y);

        mazeFile.WriteLine(gridHeight);
        mazeFile.WriteLine(gridWidth);

        for (int i = 0; i < gridHeight; i++)
        {
            for (int j = 0; j < gridWidth; j++)
            {
                string wallCode = grid[i, j].GetWallCode();
                mazeFile.WriteLine(wallCode);
            }
        }

        mazeFile.WriteLine(startPos.x);
        mazeFile.WriteLine(startPos.y);
        mazeFile.WriteLine(endPos.x);
        mazeFile.WriteLine(endPos.y);

        mazeFile.Close();

        Debug.Log("File saved");
    }

    // gets list of all files for mazes
    public string[] GetMazeFiles()
    {
        return Directory.GetFiles(@"Assets\MazeFiles\", "*.txt");
    }

    // creates a grid from information read from a file
    public void LoadMaze(string fname)
    {
        if (isGenerating) return;

        Debug.Log("loading file " + fname);

        string filePath = @"Assets\MazeFiles\" + fname;
        StreamReader mazeFile;

        try
        {
            using (StreamReader mazeFileReader = new StreamReader(filePath))
            {
                HideGraph();

                Debug.Log("Loading file...");
                mazeFile = new StreamReader(filePath);

                // initialize a grid with the stored height and width
                int gridHeight = Int32.Parse(mazeFile.ReadLine());
                int gridWidth = Int32.Parse(mazeFile.ReadLine());

                // set each tile's walls
                string[,] gridCodes = new string[gridHeight, gridWidth];
                for (int i = 0; i < gridHeight; i++)
                {
                    for (int j = 0; j < gridWidth; j++)
                    {
                        gridCodes[i, j] = mazeFile.ReadLine();
                    }
                }

                int startX = Int32.Parse(mazeFile.ReadLine());
                int startY = Int32.Parse(mazeFile.ReadLine());
                int endX = Int32.Parse(mazeFile.ReadLine());
                int endY = Int32.Parse(mazeFile.ReadLine());

                this.MakeGridFromCodes(gridCodes);

                foreach (TileScript tile in Grid)
                {
                    tile.Visit();
                }

                startTile = Grid[startY, startX];
                startTile.MakeStartTile();

                endTile = Grid[endY, endX];
                endTile.MakeEndTile();

                FindNodes();
                MakeGraph();
            }
        }
        catch (IOException e)
        {
            Console.WriteLine("File not found.");
            Console.WriteLine(e.Message);
        }
    }

    #endregion

}
