using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Figure : MonoBehaviour {

    public static GameObject GenerateRandomFigure(int w, int h) {
        var figure = Instantiate(Resources.Load<GameObject>("Prefabs/Figure"));
        figure.name = String.Format("Figure {0}x{1}", w, h);

        var figureScript = figure.GetComponent<Figure>();
        figureScript.Width = w;
        figureScript.Height = h;
        figureScript.BlocksCount = 1;
        figureScript.blocks = new int[w, h];

        // TODO: for now generate a 2x2 block
        figureScript.links[0] = new HashSet<int>();
        int id = 1;
        for (int x = 0; x < 2; x++) {
            for (int y = 0; y < 2; y++) {
                figureScript.blocks[x, y] = id;
                figureScript.links[id] = new HashSet<int>();
                if (x > 0) {
                    int prevId = figureScript.blocks[x - 1, y];
                    if (prevId > 0) {
                        figureScript.links[prevId].Add(id);
                    }
                }
                if (y > 0) {
                    int prevId = figureScript.blocks[x, y - 1];
                    if (prevId > 0) {
                        figureScript.links[prevId].Add(id);
                    }
                }
                ++id;
            }
        }

        var pfBlock = Resources.Load<GameObject>("Prefabs/Block");
        for (int x = 0; x < w; x++) {
            for (int y = 0; y < h; y++) {
                if (figureScript.blocks[x, y] > 0) {
                    var block = Instantiate(pfBlock);
                    block.transform.SetParent(figure.transform);
                    block.transform.localPosition = new Vector3(x, y, 0.0f);
                    block.GetComponent<SpriteRenderer>().sprite = figureScript.GetSprite(x, y);
                    figureScript.visualBlocks.Add(figureScript.blocks[x, y], block);
                }
            }
        }

        return figure;
    }

    private int[,] blocks;
    private Dictionary<int, HashSet<int>> links = new Dictionary<int, HashSet<int>>();
    private Dictionary<int, GameObject> visualBlocks = new Dictionary<int, GameObject>();

    public int Width { get; private set; }
    public int Height { get; private set; }
    public int BlocksCount { get; private set; }

    public bool IsFilled(int x, int y) {
        return blocks[x, y] > 0;
    }

    public void Cut(int x0, int x1, int y) {
        int v0 = blocks[x0, y];
        int v1 = blocks[x1, y];

        if (v0 > v1) {
            int temp = v1;
            v1 = v0;
            v0 = temp;
        }

        if (!links[v0].Contains(v1)) {
            Debug.Log(string.Format("({0},{1}) <-> ({2},{3}): no link to cut", x0, y, x1, y));
            return;
        }

        links[v0].Remove(v1);

        var stack = new Stack<int>();
        var visited = new HashSet<int>();
        stack.Push(v0);
        visited.Add(v0);
        while (stack.Count > 0) {
            int id = stack.Pop();

            foreach (int v in links[id]) {
                if (visited.Add(v)) {
                    stack.Push(v);
                }
            }
        }

        visualBlocks[v0].GetComponent<SpriteRenderer>().sprite = GetSprite(x0, y);
        visualBlocks[v1].GetComponent<SpriteRenderer>().sprite = GetSprite(x1, y);

        if (visited.Contains(v1)) {
            Debug.Log(string.Format("({0},{1}) <-> ({2},{3}): cut the link", x0, y, x1, y));
            return;
        }

        Debug.Log(string.Format("({0},{1}) <-> ({2},{3}): the figure is now split", x0, y, x1, y));
        // TODO: add figure splitting
    }

    void Start() {
    }

    void Update() {
        if (Input.GetAxis("Vertical") < 0) {
            Cut(0, 1, 0);
        }
        if (Input.GetAxis("Vertical") > 0) {
            Cut(0, 1, 1);
        }
    }

    // Visuals

    Sprite GetSprite(int x, int y) {
        int id = blocks[x, y];
        int leftId = x > 0 ? blocks[x - 1, y] : 0;
        int topId = y < Height - 1 ? blocks[x, y + 1] : 0;
        int rightId = x < Width - 1 ? blocks[x + 1, y] : 0;
        int bottomId = y > 0 ? blocks[x, y - 1] : 0;
        int leftBorder = links[leftId].Contains(id) ? 0 : 1;
        int topBorder = links[id].Contains(topId) ? 0 : 1;
        int rightBorder = links[id].Contains(rightId) ? 0 : 1;
        int bottomBorder = links[bottomId].Contains(id) ? 0 : 1;

        string sprite = string.Format("Textures/block_{0}{1}{2}{3}", leftBorder, topBorder, rightBorder, bottomBorder);
        Debug.Log(string.Format("[{0},{1}]: {2}", x, y, sprite));
        return Resources.Load<Sprite>(sprite);
    }
}
