using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Figure : MonoBehaviour, IMovable {
    private static int IdGen = 0;

    public static GameObject Create(bool[,] template, BlockType type) {
        var figure = Instantiate(Resources.Load<GameObject>("Prefabs/Figure"));

        var figureScript = figure.GetComponent<Figure>();
        figureScript.id = IdGen++;
        figureScript.Type = type;
        figureScript.Width = template.GetLength(0);
        figureScript.Height = template.GetLength(1);
        figureScript.blocks = new int[figureScript.Width, figureScript.Height];
        figure.name = String.Format("Figure #{0} {1}x{2}", figureScript.id, figureScript.Width, figureScript.Height);

        figureScript.links[0] = new HashSet<int>();
        int id = 1;
        for (int x = 0; x < figureScript.Width; x++) {
            for (int y = 0; y < figureScript.Height; y++) {
                if (!template[x, y]) {
                    continue;
                }
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
        figureScript.maxId = id - 1;
        for (id = 1; id <= figureScript.maxId; id++) {
            foreach (int v in figureScript.links[id]) {
                figureScript.links[v].Add(id);
            }
        }

        var pfBlock = Resources.Load<GameObject>("Prefabs/Block");
        for (int x = 0; x < figureScript.Width; x++) {
            for (int y = 0; y < figureScript.Height; y++) {
                if (figureScript.blocks[x, y] > 0) {
                    var block = Instantiate(pfBlock);
                    block.name = string.Format("Block " + figureScript.blocks[x, y]);
                    block.transform.SetParent(figure.transform);
                    block.transform.localPosition = new Vector3(x, y, 0.0f);
                    var renderer = block.GetComponent<SpriteRenderer>();
                    renderer.sprite = figureScript.GetSprite(x, y);
                    renderer.color = Game.TypeToColor(type);
                    figureScript.visualBlocks.Add(figureScript.blocks[x, y], block);
                }
            }
        }

        return figure;
    }

    private int id;
    private int maxId; // TODO: rewrite ID usage to not need this
    private int[,] blocks;
    private Dictionary<int, HashSet<int>> links = new Dictionary<int, HashSet<int>>();
    private Dictionary<int, GameObject> visualBlocks = new Dictionary<int, GameObject>();

    public BlockType Type { get; private set; }
    public int Width { get; private set; }
    public int Height { get; private set; }

    public bool IsFilled(int x, int y) {
        return blocks[x, y] > 0;
    }

    public bool CutRightOf(GameObject blockLeft) {
        // TODO: Create a block => id map if slow
        for (int id = 1; id <= maxId; id++) {
            if (visualBlocks.ContainsKey(id)) {
                if (visualBlocks[id] == blockLeft) {
                    // TODO: Create an id => (x, y) map if slow
                    for (int x = 0; x < Width; x++) {
                        for (int y = 0; y < Height; y++) {
                            if (blocks[x, y] == id) {
                                return Cut(x, x + 1, y);
                            }
                        }
                    }
                }
            }
        }
        return false;
    }

    public void Rotate3x3(Vector2 rotatorAreaBottomLeft, bool rotateCW) {
        const int RotatorSize = 3;

        var rotatedBlocks = new int[Height, Width];
        for (int x = 0; x < Width; x++) {
            for (int y = 0; y < Height; y++) {
                if (rotateCW) {
                    rotatedBlocks[y, Width - x - 1] = blocks[x, y];
                } else {
                    rotatedBlocks[Height - y - 1, x] = blocks[x, y];
                }
            }
        }
        int oldWidth = Width;
        int oldHeight = Height;

        Width = oldHeight;
        Height = oldWidth;
        blocks = rotatedBlocks;

        for (int x = 0; x < Width; x++) {
            for (int y = 0; y < Height; y++) {
                int id = blocks[x, y];
                if (id > 0) {
                    visualBlocks[id].transform.localPosition = new Vector2(x, y);
                    visualBlocks[id].GetComponent<SpriteRenderer>().sprite = GetSprite(x, y);
                }
            }
        }

        Vector2 oldBottomLeft = (Vector2)transform.localPosition - rotatorAreaBottomLeft;
        Vector2 newBottomLeft;
        if (rotateCW) {
            var bottomRight = oldBottomLeft + new Vector2(Height - 1, 0);
            newBottomLeft = new Vector2(bottomRight.y, RotatorSize - bottomRight.x - 1);
        } else {
            var topLeft = oldBottomLeft + new Vector2(0, Width - 1);
            newBottomLeft = new Vector2(RotatorSize - topLeft.y - 1, topLeft.x);
        }
        transform.localPosition = rotatorAreaBottomLeft + newBottomLeft;
    }

    bool Cut(int x0, int x1, int y) {
        int v0 = blocks[x0, y];
        int v1 = blocks[x1, y];

        if (!links[v0].Contains(v1)) {
            return false;
        }

        links[v0].Remove(v1);
        links[v1].Remove(v0);

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
            return true;
        }

        Debug.Log(string.Format("({0},{1}) <-> ({2},{3}): the figure is now split", x0, y, x1, y));

        stack = new Stack<int>();
        var figure2ids = new HashSet<int>();
        stack.Push(v1);
        figure2ids.Add(v1);
        while (stack.Count > 0) {
            int id = stack.Pop();

            foreach (int v in links[id]) {
                if (figure2ids.Add(v)) {
                    stack.Push(v);
                }
            }
        }

        var newFigure = ShallowClone();
        SplitTo(visited);
        newFigure.GetComponent<Figure>().SplitTo(figure2ids);
        newFigure.transform.SetParent(transform.parent);

        return true;
    }

    // This method assumes the parameter is a separate graph component
    void SplitTo(HashSet<int> ids) {
        int minX = Width;
        int maxX = -1;
        int minY = Height;
        int maxY = -1;

        for (int x = 0; x < Width; x++) {
            for (int y = 0; y < Height; y++) {
                if (ids.Contains(blocks[x, y])) {
                    minX = (int)Math.Min(minX, x);
                    maxX = (int)Math.Max(maxX, x);
                    minY = (int)Math.Min(minY, y);
                    maxY = (int)Math.Max(maxY, y);
                }
            }
        }

        int oldWidth = Width;
        int oldHeight = Height;
        Width = maxX - minX + 1;
        Height = maxY - minY + 1;
        gameObject.name = string.Format("Figure #{0} {1}x{2}", id, Width, Height);

        var newBlocks = new int[Width, Height];
        for (int x = minX; x <= maxX; x++) {
            for (int y = minY; y <= maxY; y++) {
                if (ids.Contains(blocks[x, y])) {
                    newBlocks[x - minX, y - minY] = blocks[x, y];
                }
            }
        }
        blocks = newBlocks;

        transform.position += new Vector3(minX, minY, 0.0f);
        for (int id = 1; id <= maxId; id++) {
            if (ids.Contains(id)) {
                visualBlocks[id].transform.SetParent(transform);
            } else {
                links.Remove(id);
                visualBlocks.Remove(id);
            }
        }
    }

    GameObject ShallowClone() {
        var clone = Instantiate(Resources.Load<GameObject>("Prefabs/Figure"));
        clone.transform.position = transform.position;

        var cloneScript = clone.GetComponent<Figure>();
        cloneScript.id = IdGen++;
        cloneScript.maxId = maxId;
        cloneScript.Type = Type;
        cloneScript.Width = Width;
        cloneScript.Height = Height;
        cloneScript.blocks = new int[Width, Height];
        clone.name = string.Format("Figure #{0} {1}x{2}", cloneScript.id, cloneScript.Width, cloneScript.Height);

        Array.Copy(blocks, cloneScript.blocks, Width * Height);
        foreach (var pair in links) {
            cloneScript.links[pair.Key] = new HashSet<int>(pair.Value);
        }
        foreach (var pair in visualBlocks) {
            cloneScript.visualBlocks[pair.Key] = pair.Value;
        }

        return clone;
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
        return Resources.Load<Sprite>(sprite);
    }

    public IEnumerable<Vector2> EnumerateAllFilledBlocks() {
        for (int x = 0; x < Width; x++) {
            for (int y = 0; y < Height; y++) {
                if (blocks[x, y] > 0) {
                    yield return new Vector2(x, y);
                }
            }
        }
    }

    public void GetAllowedMoves(out bool left, out bool top, out bool right, out bool bottom) {
        var collisionField = Game.Instance.GetCollisionField(new HashSet<GameObject> { gameObject });

        var selfPos = new Vector2((float)Math.Round(transform.position.x), (float)Math.Round(transform.position.y));
        var positions = EnumerateAllFilledBlocks().Select(p => p + selfPos);

        Func<Vector2, MoveDirection, bool> isAllowed = (p, d) =>
            Game.Instance.IsMoveAllowed(collisionField, (int)p.x, (int)p.y, d);
        left = positions.All(p => isAllowed(p, MoveDirection.Left));
        top = positions.All(p => isAllowed(p, MoveDirection.Up));
        right = positions.All(p => isAllowed(p, MoveDirection.Right));
        bottom = positions.All(p => isAllowed(p, MoveDirection.Down));
    }
}
