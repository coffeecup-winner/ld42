using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum BlockType {
    Green,
    Blue,
    Red,
}

public class Game : MonoBehaviour {
    public static Game Instance { get; private set; }

    private Transform figures;

    // playable means within walls
    public int levelPlayableHeight;
    // total playable width = these four + 3 (1 for each color output)
    public int levelWidthBeforeGreen;
    public int levelWidthGreenToRed;
    public int levelWidthRedToBlue;
    public int levelWidthAfterBlue;
    // top left corner
    public int levelHoleSize;

    // Colors
    public Color green;
    public Color blue;
    public Color red;

    // only valid after Awake()
    public static int levelWidth { get; private set; }
    public static int levelHeight { get; private set; }

    public static Color TypeToColor(BlockType type) {
        return type == BlockType.Green ? Game.Instance.green
            : type == BlockType.Blue ? Game.Instance.blue
            : Game.Instance.red;
    }

    void Awake() {
        levelWidth = 3 + levelWidthBeforeGreen + levelWidthGreenToRed + levelWidthRedToBlue + levelWidthAfterBlue;
        levelHeight = levelPlayableHeight;
    }

    void Start() {
        Instance = this;
        figures = GameObject.Find("Figures").transform;

        var figure = Figure.Create(FigureFactory.GetTemplate(), BlockType.Green);
        figure.transform.SetParent(figures);
        figure.transform.localPosition = Vector3.zero;

        generateLevel();

        float yMin = -3.5f;
        float yMax = levelHeight + levelHoleSize + 1 + 0.5f;
        Camera.main.transform.position = new Vector3(levelWidth * 0.5f, 0.5f * (yMin + yMax), -10.0f);
        Camera.main.orthographicSize = 0.5f * (yMax - yMin);
    }

    void Update() {
    }

    public void OnBlockMouseDown(GameObject block) {
    }

    private void generateLevel() {
        var level = GameObject.Find("Level").transform;
        var tools = GameObject.Find("Tools").transform;
        var pfWall = Resources.Load<GameObject>("Prefabs/Wall");
        var pfOutput = Resources.Load<GameObject>("Prefabs/Output");
        var pfSaw = Resources.Load<GameObject>("Prefabs/Saw");
        var pfRotator = Resources.Load<GameObject>("Prefabs/Rotator");

        int emptyX1 = levelWidthBeforeGreen;
        int emptyX2 = emptyX1 + 1 + levelWidthGreenToRed;
        int emptyX3 = emptyX2 + 1 + levelWidthRedToBlue;

        var wallPositions = new List<Vector2>();
        var outputPositions = new List<Vector2>();

        for (int x = -1; x <= levelWidth; x += 1) {
            // the topmost layer is full
            wallPositions.Add(new Vector2(x, levelHeight + levelHoleSize + 1));

            // the top/middle layer has a gap
            if (x < 0 || x >= levelHoleSize)
                wallPositions.Add(new Vector2(x, levelHeight));

            // the bottom layer needs gaps for color outputs
            if (x != emptyX1 && x != emptyX2 && x != emptyX3)
                wallPositions.Add(new Vector2(x, -1));
            else
                outputPositions.Add(new Vector2(x, -1));

            // the very bottom layer is full
            wallPositions.Add(new Vector2(x, -3));
        }

        for (int y = 0; y <= levelHeight + levelHoleSize; ++y) {
            wallPositions.Add(new Vector2(-1, y));
            if (y < levelHeight)
                wallPositions.Add(new Vector2(levelWidth, y));
        }

        // extra regular walls
        wallPositions.Add(new Vector2(-1, -2));
        wallPositions.Add(new Vector2(emptyX3 - 1, -2));
        wallPositions.Add(new Vector2(levelWidth, -2));

        foreach (Vector2 pos in wallPositions) {
            var wall = Instantiate(pfWall);
            wall.transform.SetParent(level);
            wall.transform.localPosition = (Vector3)pos;
        }

        var types = new[] { BlockType.Green, BlockType.Red, BlockType.Blue };
        for (int i = 0; i < 3; i++) {
            var output = Instantiate(pfOutput);
            output.name = string.Format("Output ({0})", types[i]);
            output.GetComponent<Output>().Type = types[i];
            output.GetComponent<SpriteRenderer>().color = TypeToColor(types[i]);
            output.transform.SetParent(tools);
            output.transform.localPosition = (Vector3)outputPositions[i];
        }

        var saw = Instantiate(pfSaw);
        saw.name = "Saw";
        saw.transform.SetParent(tools);
        saw.transform.localPosition = new Vector3(4.0f, 2.0f, 0.0f);

        var rotator = Instantiate(pfRotator);
        rotator.name = "Rotator";
        rotator.transform.SetParent(tools);
        rotator.transform.localPosition = new Vector3(8.0f, 2.0f, 0.0f);
    }

    public bool[,] GetCollisionField(HashSet<GameObject> exclude) {
        var field = new bool[levelWidth, levelHeight];

        var activeObjects = GameObject.FindGameObjectsWithTag("Figure")
            .Concat(GameObject.FindGameObjectsWithTag("Tool"))
            .Where(o => !exclude.Contains(o));
        foreach (var obj in activeObjects) {
            int ox = (int)Math.Round(obj.transform.position.x);
            int oy = (int)Math.Round(obj.transform.position.y);
            foreach (var pos in obj.GetComponent<IMovable>().EnumerateAllFilledBlocks()) {
                field[ox + (int)pos.x, oy + (int)pos.y] = true;
            }
        }

        return field;
    }

    public bool IsPositionAllowed(bool[,] collisionField, int x, int y) {
        int output1 = levelWidthBeforeGreen;
        int output2 = output1 + 1 + levelWidthGreenToRed;
        int output3 = output2 + 1 + levelWidthRedToBlue;

        return (y == -1 && (x == output1 || x == output2 || x == output3)) ||
            (x >= 0 && x < levelWidth && y >= 0 && y < levelHeight && !collisionField[x, y]);
    }

    public bool TryOutput(BlockType type) {
        // TODO
        return true;
    }
}
