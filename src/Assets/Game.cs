using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Game : MonoBehaviour {
    public static Game Instance { get; private set; }

    private Transform figures;

    void Start() {
        Instance = this;
        figures = GameObject.Find("Figures").transform;

        var figure = Figure.GenerateRandomFigure(2, 2);
        figure.transform.SetParent(figures);
    }

    void Update() {
    }

    public void OnBlockMouseDown(GameObject block) {
    }
}
