using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Game : MonoBehaviour {
    public static Game Instance { get; private set; }

    public const int IslandRadius = 10;

    private Transform groundLayer;

    void Start() {
        Instance = this;
        groundLayer = new GameObject("GroundLayer").transform;
        groundLayer.SetParent(transform);

        GenerateGround();
    }

    void GenerateGround() {
        var pfGround = Resources.Load<GameObject>("Prefabs/Ground");
        for (int x = -IslandRadius; x <= IslandRadius; x++) {
            for (int y = -IslandRadius; y <= IslandRadius; y++) {
                if (Math.Sqrt(x * x + y * y) < IslandRadius + 1) {
                    var ground = Instantiate(pfGround, new Vector3(x, y, 0f), Quaternion.identity);
                    ground.transform.SetParent(groundLayer);
                }
            }
        }
    }

    void Update() {

    }
}
