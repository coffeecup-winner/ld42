using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Game : MonoBehaviour {
    public static Game Instance { get; private set; }

    private Transform figures;

    // playable means within walls
    public int levelPlayableHeight = 6;
    // total playable width = these four + 3 (1 for each color output)
    public int levelWidthBeforeGreen = 2;
    public int levelWidthGreenToBlue = 3;
    public int levelWidthBlueToRed = 3;
    public int levelWidthAfterRed = 2;
    // top left corner
    public int levelHoleSize = 5;

    // only valid after Awake()
    public static int levelWidth { get; private set; }
    public static int levelHeight { get; private set; }

    void Awake() {
        levelWidth = 3 + levelWidthBeforeGreen + levelWidthGreenToBlue + levelWidthBlueToRed + levelWidthAfterRed;
        levelHeight = levelPlayableHeight;
    }

    void Start() {
        Instance = this;
        figures = GameObject.Find("Figures").transform;

        var figure = Figure.GenerateRandomFigure(2, 2);
        figure.transform.SetParent(figures);
        figure.transform.localPosition = Vector3.zero;

        generateLevel();

        float yMin = -2.5f;
        float yMax = levelHeight + levelHoleSize + 0.5f;
        Camera.main.transform.position = new Vector3(levelWidth * 0.5f, 0.5f * (yMin + yMax), -10.0f);
        Camera.main.orthographicSize = 0.5f * (yMax - yMin);
    }

    void Update() {
    }

    public void OnBlockMouseDown(GameObject block) {
    }

    private void generateLevel() {
        var level = GameObject.Find("Level").transform;
        var pfWall = Resources.Load<GameObject>("Prefabs/Wall");

        int emptyX1 = levelWidthBeforeGreen;
        int emptyX2 = emptyX1 + 1 + levelWidthGreenToBlue;
        int emptyX3 = emptyX2 + 1 + levelWidthBlueToRed;

        var wallPositions = new List<Vector2>();
        for (int x = -1; x <= levelWidth; x += 1) {
            wallPositions.Add(new Vector2(x, -2));
            // the bottom layer needs gaps for color outputs
            if (x != emptyX1 && x != emptyX2 && x != emptyX3)
                wallPositions.Add(new Vector2(x, -1));
            // the top layer has a gap
            if (x < 0 || x > levelHoleSize)
                wallPositions.Add(new Vector2(x, levelHeight));
        }

        for (int y = 0; y < levelHeight; ++y) {
            wallPositions.Add(new Vector2(-1, y));
            wallPositions.Add(new Vector2(levelWidth, y));
        }

        foreach (Vector2 pos in wallPositions) {
            var wall = Instantiate(pfWall);
            wall.transform.SetParent(level);
            wall.transform.localPosition = (Vector3)pos;
        }
    }
}
