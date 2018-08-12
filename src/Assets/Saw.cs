using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Saw : MonoBehaviour, IMovable {
    void Update() {
        GameObject blockLeft = null;
        GameObject blockRight = null;
        foreach (var block in GameObject.FindGameObjectsWithTag("Block")) {
            if (Math.Abs(block.transform.position.y - transform.position.y) < 1.1f) {
                if (Math.Abs(block.transform.position.x - transform.position.x) < 0.1f) {
                    blockLeft = block;
                } else if (Math.Abs(block.transform.position.x - transform.position.x) < 1.1f) {
                    blockRight = block;
                }
            }
        }

        if (blockLeft == null || blockRight == null || blockLeft.transform.parent != blockRight.transform.parent) {
            return;
        }

        blockRight.transform.parent.GetComponent<Figure>().CutRightOf(blockLeft);
    }

    public IEnumerable<Vector2> EnumerateAllFilledBlocks() {
        yield return new Vector2(0, 0);
        yield return new Vector2(1, 0);
    }

    public void GetAllowedMoves(out bool left, out bool top, out bool right, out bool bottom) {
        throw new NotImplementedException();
    }
}
