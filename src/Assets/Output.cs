using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Output : MonoBehaviour {
    public BlockType Type { get; set; }

    void Update() {
        GameObject placedBlock = null;
        foreach (var block in GameObject.FindGameObjectsWithTag("Block")) {
            if (block.transform.position.y == transform.position.y &&
                block.transform.position.x == transform.position.x) {
                placedBlock = block;
            }
        }

        if (placedBlock == null || placedBlock.transform.parent.childCount != 1 ||
            placedBlock.transform.parent.gameObject.GetComponent<Figure>().Type != Type) {
            return;
        }

        if (Game.Instance.TryOutput(Type)) {
            GameObject.Destroy(placedBlock.transform.parent.gameObject);
        }
    }
}
