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

public enum MoveDirection {
    Left,
    Up,
    Right,
    Down,
}

public class CollisionData {
    public CollisionData(bool[,] field, Vector2 leftOfSawBlade, Vector2 rightOfSawBlade) {
        Field = field;
        LeftOfSawBlade = leftOfSawBlade;
        RightOfSawBlade = rightOfSawBlade;
    }
    public bool[,] Field { get; private set; }
    public Vector2 LeftOfSawBlade { get; private set; }
    public Vector2 RightOfSawBlade { get; private set; }
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

    public class ShapeDesc {
        public int templateId;
        public BlockType blockType;

        public ShapeDesc(int i, BlockType bt) {
            templateId = i;
            blockType = bt;
        }
    }

    // figures pipe
    private static readonly List<Vector2> visiblePipePositions = new List<Vector2>();
    private static readonly Queue<GameObject> visibleFigures = new Queue<GameObject>();
    // private static readonly Queue<GameObject> allFigures = new Queue<GameObject>();
    private static readonly Queue<ShapeDesc> remainingShapes = new Queue<ShapeDesc>();

    // resources
    public static int maxFuel { get; private set; }
    private static int fuelField;
    public static int fuel {
        get {
            return fuelField;
        }
        set {
            int ok = value;
            if (ok < 0)
                ok = 0;
            if (ok > maxFuel)
                ok = maxFuel;
            UiStuff.setFuel(ok);
            fuelField = ok;
        }
    }

    public static int maxResearch { get; private set; }
    private static int researchField;
    public static int research {
        get {
            return researchField;
        }
        set {
            int ok = value;
            if (ok < 0)
                ok = 0;

            if (ok >= maxResearch) {
                if (ok > maxResearch)
                    Debug.LogWarning(string.Format("research went above max ({0} > {1}) ", ok, maxResearch));
                ok = 0;
                Instance.GrantUpgrade();
            }

            UiStuff.setResearch(ok);
            researchField = ok;
        }
    }

    public static Color TypeToColor(BlockType type) {
        return type == BlockType.Green ? Game.Instance.green
            : type == BlockType.Blue ? Game.Instance.blue
            : Game.Instance.red;
    }

    public static int cuttingCost(BlockType type) {
        if (type == BlockType.Green)
            return 0;
        if (type == BlockType.Blue)
            return sawCostUpgraded ? 0 : 1;
        if (type == BlockType.Red)
            return sawCostUpgraded ? 1 : 2;
        return 0;
    }

    public static int rotationCost { get; set; }
    public static int transmutationCost { get; private set; }

    public static bool sawCostUpgraded { get; set; }
    public static bool rotatorCostUpgraded { get; set; }
    public static int rotatorSize { get; set; }
    public static int transmuterSize { get; set; }

    void Awake() {
        Instance = this;
        levelWidth = 3 + levelWidthBeforeGreen + levelWidthGreenToRed + levelWidthRedToBlue + levelWidthAfterBlue;
        levelHeight = levelPlayableHeight;

        rotationCost = 1;
        transmutationCost = 1;
        maxFuel = 100;
        fuel = 10;
        maxResearch = 5;
        research = 0;

        sawCostUpgraded = false;
        rotatorCostUpgraded = false;
        rotatorSize = 2;
        transmuterSize = 0;

        // 0-10: 9 green, 10 blue, 5 red
        remainingShapes.Enqueue(new ShapeDesc(0, BlockType.Green));
        remainingShapes.Enqueue(new ShapeDesc(0, BlockType.Blue));
        remainingShapes.Enqueue(new ShapeDesc(0, BlockType.Red));
        remainingShapes.Enqueue(new ShapeDesc(1, BlockType.Green));
        remainingShapes.Enqueue(new ShapeDesc(2, BlockType.Blue));
        remainingShapes.Enqueue(new ShapeDesc(3, BlockType.Green));
        remainingShapes.Enqueue(new ShapeDesc(8, BlockType.Blue));
        remainingShapes.Enqueue(new ShapeDesc(15, BlockType.Red));
        remainingShapes.Enqueue(new ShapeDesc(6, BlockType.Green));
        remainingShapes.Enqueue(new ShapeDesc(13, BlockType.Blue));
        // 10-20: 11 green, 9 blue, 22 red
        remainingShapes.Enqueue(new ShapeDesc(10, BlockType.Red));
        remainingShapes.Enqueue(new ShapeDesc(11, BlockType.Red));
        remainingShapes.Enqueue(new ShapeDesc(7, BlockType.Green));
        remainingShapes.Enqueue(new ShapeDesc(16, BlockType.Blue));
        remainingShapes.Enqueue(new ShapeDesc(18, BlockType.Red));
        remainingShapes.Enqueue(new ShapeDesc(22, BlockType.Green));
        remainingShapes.Enqueue(new ShapeDesc(31, BlockType.Red));
        remainingShapes.Enqueue(new ShapeDesc(5, BlockType.Green));
        remainingShapes.Enqueue(new ShapeDesc(12, BlockType.Blue));
        remainingShapes.Enqueue(new ShapeDesc(30, BlockType.Red));
        // 20-30: 4 green, 17 blue, 21 red,
        remainingShapes.Enqueue(new ShapeDesc(26, BlockType.Blue));
        remainingShapes.Enqueue(new ShapeDesc(19, BlockType.Blue));
        remainingShapes.Enqueue(new ShapeDesc(23, BlockType.Red));
        remainingShapes.Enqueue(new ShapeDesc(9, BlockType.Green));
        remainingShapes.Enqueue(new ShapeDesc(20, BlockType.Red));
        remainingShapes.Enqueue(new ShapeDesc(14, BlockType.Red));
        remainingShapes.Enqueue(new ShapeDesc(25, BlockType.Blue));
        remainingShapes.Enqueue(new ShapeDesc(4, BlockType.Blue));
        remainingShapes.Enqueue(new ShapeDesc(17, BlockType.Red));
        remainingShapes.Enqueue(new ShapeDesc(24, BlockType.Red));
        // 30-34: whatever
        remainingShapes.Enqueue(new ShapeDesc(29, BlockType.Green));
        remainingShapes.Enqueue(new ShapeDesc(21, BlockType.Blue));
        remainingShapes.Enqueue(new ShapeDesc(28, BlockType.Red));
        remainingShapes.Enqueue(new ShapeDesc(27, BlockType.Red));
        Debug.Log("TemplatesCount = " + FigureFactory.TemplatesCount);
    }

    void Start() {
        figures = GameObject.Find("Figures").transform;

        generateLevel();

        // for (int i = 0; i < FigureFactory.TemplatesCount; i++) {
        //     var blockType = BlockType.Green;
        //     if (i % 2 == 1) {
        //         blockType = i % 4 == 1 ? BlockType.Blue : BlockType.Red;
        //     }
        //     allFigures.Enqueue(Figure.Create(FigureFactory.GetTemplate(i), blockType));
        // }
        for (int i = 0; i < visiblePipePositions.Count && remainingShapes.Count > 0; i++) {
            // var figure = allFigures.Dequeue();
            // var figure = Figure.Create(FigureFactory.GetTemplate(i), BlockType.Green);
            var desc = remainingShapes.Dequeue();
            var figure = Figure.Create(FigureFactory.GetTemplate(desc.templateId), desc.blockType);
            figure.transform.SetParent(figures);
            figure.transform.localPosition = visiblePipePositions[i];
            visibleFigures.Enqueue(figure);
        }

        float yMin = -3.5f;
        float yMax = levelHeight + levelHoleSize + 1 + 0.5f;
        Camera.main.transform.position = new Vector3(levelWidth * 0.5f, 0.5f * (yMin + yMax), -10.0f);
        Camera.main.orthographicSize = 0.5f * (yMax - yMin);
    }

    // void nextFigure()

    void Update() {
        bool hasBlocksInInputArea = GameObject.FindGameObjectsWithTag("Block")
            .Any(o => o.transform.parent.parent == figures && // ~= is it visible
                 o.transform.position.x >= 0 && o.transform.position.x < levelHoleSize &&
                 o.transform.position.y >= levelHeight && o.transform.position.y < levelHeight + levelHoleSize + 1);

        if (!hasBlocksInInputArea && visibleFigures.Count > 0) {
            visibleFigures.Dequeue(); // remove the one that was dragged out
            foreach (var figure in visibleFigures) {
                figure.transform.localPosition += (Vector3)new Vector2(-levelHoleSize, 0);
            }
            // if (allFigures.Count > 0) {
            //     var newFigure = allFigures.Dequeue();
            //     newFigure.transform.SetParent(figures);
            //     newFigure.transform.localPosition = visiblePipePositions.Last();
            //     visibleFigures.Enqueue(newFigure);
            // } else {
            //     // TODO: PUT YOU WIN MESSAGE HERE
            // }
            if (remainingShapes.Count > 0) {
                var desc = remainingShapes.Dequeue();
                var newFigure = Figure.Create(FigureFactory.GetTemplate(desc.templateId), desc.blockType);
                newFigure.transform.SetParent(figures);
                newFigure.transform.localPosition = visiblePipePositions.Last();
                visibleFigures.Enqueue(newFigure);
            } else {
                // TODO: PUT YOU WIN MESSAGE HERE
            }
        }
    }

    public void OnBlockMouseDown(GameObject block) {
    }

    private void generateLevel() {
        var level = GameObject.Find("Level").transform;
        var tools = GameObject.Find("Tools").transform;
        var pfWall = Resources.Load<GameObject>("Prefabs/Wall");
        var pfPipeEnd = Resources.Load<GameObject>("Prefabs/PipeEnd");
        var pfPipe = Resources.Load<GameObject>("Prefabs/Pipe");
        var pfInputArea = Resources.Load<GameObject>("Prefabs/InputArea");
        var pfDownArrow = Resources.Load<GameObject>("Prefabs/DownArrow");
        var pfFloor = Resources.Load<GameObject>("Prefabs/Floor");
        var pfOutput = Resources.Load<GameObject>("Prefabs/Output");
        var pfSaw = Resources.Load<GameObject>("Prefabs/Saw");
        var pfRotator = Resources.Load<GameObject>("Prefabs/Rotator");
        var pfTransmuter = Resources.Load<GameObject>("Prefabs/Transmuter");

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

        int inputAreaX = 0;
        for (int inputIdx = 0; inputIdx < levelWidth / levelHoleSize; inputIdx++) {
            inputAreaX = inputIdx * levelHoleSize;
            float modifier = inputIdx % 2 == 0 ? 1.1f : 1.0f;
            for (int x = 0; x < levelHoleSize; x++) {
                for (int y = 0; y < levelHoleSize; y++) {
                    var inputArea = Instantiate(pfInputArea);
                    inputArea.transform.SetParent(level);
                    inputArea.transform.localPosition = new Vector2(inputAreaX + x, levelHeight + y + 1);
                    inputArea.GetComponent<SpriteRenderer>().color *= modifier;
                }
            }
            visiblePipePositions.Add(new Vector2(inputAreaX, levelHeight + 1));
        }

        for (int x = inputAreaX + levelHoleSize; x < levelWidth + 1; x++) {
            for (int y = levelHeight + 1; y < levelHeight + levelHoleSize + 1; y++) {
                var pipe = Instantiate(x == (inputAreaX + levelHoleSize) ? pfPipeEnd : pfPipe);
                pipe.transform.SetParent(level);
                pipe.transform.localPosition = new Vector2(x, y);
            }
        }

        for (int x = 0; x < levelHoleSize; x++) {
            var arrow = Instantiate(pfDownArrow);
            arrow.transform.SetParent(level);
            arrow.transform.localPosition = new Vector2(x, levelHeight);
        }

        for (int x = 0; x < levelWidth; x++) {
            for (int y = 0; y < levelHeight; y++) {
                var floor = Instantiate(pfFloor);
                floor.transform.SetParent(level);
                floor.transform.localPosition = new Vector2(x, y);
            }
        }

        var types = new[] { BlockType.Green, BlockType.Red, BlockType.Blue };
        for (int i = 0; i < 3; i++) {
            var floor = Instantiate(pfFloor);
            floor.transform.SetParent(level);
            floor.transform.localPosition = (Vector3)outputPositions[i];

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
        saw.transform.localPosition = new Vector3(5.0f, 1.0f, 0.0f);

        var rotator = Instantiate(pfRotator);
        rotator.name = "Rotator";
        rotator.transform.SetParent(tools);
        rotator.transform.localPosition = new Vector3(10.0f, 1.0f, 0.0f);

        var transmuter = Instantiate(pfTransmuter);
        transmuter.name = "Transmuter";
        transmuter.transform.SetParent(tools);
        transmuter.transform.localPosition = new Vector3(16.0f, 1.0f, 0.0f);
    }

    public CollisionData GetCollisionData(HashSet<GameObject> exclude) {
        var field = new bool[levelWidth, levelHeight + levelHoleSize + 1];

        var activeObjects = GameObject.FindGameObjectsWithTag("Figure")
            .Where(f => f.transform.parent == figures)
            .Concat(GameObject.FindGameObjectsWithTag("Tool"))
            .Where(o => !exclude.Contains(o));

        Vector2 leftOfSawBlade = new Vector2(0, 0); // to make compiler happy
        foreach (var obj in activeObjects) {
            int ox = (int)Math.Round(obj.transform.position.x);
            int oy = (int)Math.Round(obj.transform.position.y);
            foreach (var pos in obj.GetComponent<IMovable>().EnumerateAllFilledBlocks()) {
                field[ox + (int)pos.x, oy + (int)pos.y] = true;
            }
            var saw = obj.GetComponent<Saw>();
            if (saw != null) {
                leftOfSawBlade = saw.LeftOfSawBlade;
            }
        }

        return new CollisionData(field, leftOfSawBlade, leftOfSawBlade + new Vector2(1, 0));
    }

    public bool IsMoveAllowed(CollisionData collisionData, int x, int y, MoveDirection direction) {
        int output1 = levelWidthBeforeGreen;
        int output2 = output1 + 1 + levelWidthGreenToRed;
        int output3 = output2 + 1 + levelWidthRedToBlue;

        switch (direction) {
            case MoveDirection.Left: x -= 1; break;
            case MoveDirection.Up: y += 1; break;
            case MoveDirection.Right: x += 1; break;
            case MoveDirection.Down: y -= 1; break;
            default: throw new InvalidOperationException();
        }

        bool isInInputArea = y >= levelHeight && x < levelHoleSize && direction == MoveDirection.Down;
        bool isInOutputArea = y == -1 && (x == output1 || x == output2 || x == output3);
        bool isClippingThroughBlocks = x < 0 || x >= levelWidth || y < 0 || y >= levelHeight || collisionData.Field[x, y];
        bool isClippingThroughSawBlade = y == (int)collisionData.LeftOfSawBlade.y &&
            (direction == MoveDirection.Left && x == (int)collisionData.LeftOfSawBlade.x ||
             direction == MoveDirection.Right && x == (int)collisionData.RightOfSawBlade.x);
        return isInInputArea || isInOutputArea || !(isClippingThroughBlocks || isClippingThroughSawBlade);
    }

    public bool TryOutput(BlockType type) {
        switch (type) {
            case BlockType.Green:
                fuel += 2;
                return true;
            case BlockType.Blue:
                // block if the player has a pending upgrade
                if (research < maxResearch) {
                    research += 1;
                    return true;
                }
                return false;
            case BlockType.Red:
                if (fuel == 0) {
                    return false;
                }
                fuel -= 1;
                return true;
            default: throw new InvalidOperationException();
        }
    }

    void GrantUpgrade() {
        UiStuff.Instance.EnableUpgradeButton();
    }
}
