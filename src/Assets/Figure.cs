using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Figure : MonoBehaviour {

    private struct Link {
        public Link(int v0, int v1) {
            if (v0 >= v1) {
                throw new Exception("ERROR: Invalid link");
            }
            V0 = v0;
            V1 = v1;
        }
        public int V0;
        public int V1;
    }

    public static GameObject GenerateRandomFigure(int w, int h) {
        var figure = Instantiate(Resources.Load<GameObject>("Prefabs/Figure"));
        figure.name = String.Format("Figure {0}x{1}", w, h);

        var figureScript = figure.GetComponent<Figure>();
        figureScript.Width = w;
        figureScript.Height = h;
        figureScript.BlocksCount = 1;
        figureScript.blocks = new int[w, h];

        var pfBlock = Resources.Load<GameObject>("Prefabs/Block");
        // TODO: for now generate a 2x2 block
        int id = 1;
        for (int x = 0; x < 2; x++) {
            for (int y = 0; y < 2; y++) {
                var block = Instantiate(pfBlock, new Vector3(x, y, 0.0f), Quaternion.identity);
                block.transform.SetParent(figure.transform);

                figureScript.blocks[x, y] = id++;
                if (x > 0) {
                    int prevId = figureScript.blocks[x - 1, y];
                    if (prevId > 0) {
                        figureScript.links.Add(new Link(prevId, id - 1));
                    }
                }
                if (y > 0) {
                    int prevId = figureScript.blocks[x, y - 1];
                    if (prevId > 0) {
                        figureScript.links.Add(new Link(prevId, id - 1));
                    }
                }
            }
        }

        return figure;
    }

    private int[,] blocks;
    private List<Link> links = new List<Link>();

    public int Width { get; private set; }
    public int Height { get; private set; }
    public int BlocksCount { get; private set; }

    public bool IsFilled(int x, int y) {
        return blocks[x, y] > 0;
    }

    void Start() {
    }

    void Update() {
    }
}
