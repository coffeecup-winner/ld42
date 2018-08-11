using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Figure : MonoBehaviour {

    public static GameObject GenerateRandomFigure(int w, int h) {
        var figure = Instantiate(Resources.Load<GameObject>("Prefabs/Figure"));
        figure.name = String.Format("Figure {0}x{1}", w, h);
        var pfBlock = Resources.Load<GameObject>("Prefabs/Block");
        // TODO: for now generate a 1x1 block
        var block = Instantiate(pfBlock);
        block.transform.SetParent(figure.transform);
        return figure;
    }

    void Start() {
    }

    void Update() {
    }
}
