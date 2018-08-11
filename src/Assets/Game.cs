using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Game : MonoBehaviour {
    public static Game Instance { get; private set; }

    private Transform blocksLayer;

    void Start() {
        Instance = this;
        blocksLayer = new GameObject("BlocksLayer").transform;
        blocksLayer.SetParent(transform);

        GenerateRandomBlock(1, 1);
    }

    void GenerateRandomBlock(int w, int h) {
        var pfBlock = Resources.Load<GameObject>("Prefabs/Block");
        // TODO: for now generate a 1x1 block
        var block = Instantiate(pfBlock);
        block.transform.SetParent(blocksLayer);
    }

    void Update() {
    }

    public void OnBlockMouseDown(GameObject block) {
    }
}
