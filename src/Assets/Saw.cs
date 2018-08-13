using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Saw : MonoBehaviour, IMovable {
    public Vector2 LeftOfSawBlade { get { return (Vector2)transform.position + new Vector2(0, 1); } }

    void Update() {
        GameObject blockLeft = null;
        GameObject blockRight = null;
        foreach (var block in GameObject.FindGameObjectsWithTag("Block")) {
            if (block.transform.position.y == transform.position.y + 1) {
                if (block.transform.position.x == transform.position.x) {
                    blockLeft = block;
                } else if (block.transform.position.x == transform.position.x + 1) {
                    blockRight = block;
                }
            }
        }

        if (blockLeft == null || blockRight == null || blockLeft.transform.parent != blockRight.transform.parent) {
            return;
        }

        var figure = blockRight.transform.parent.GetComponent<Figure>();
        int cost = Game.cuttingCost(figure.Type);
        if (Game.fuel >= cost) {
            if (figure.CutRightOf(blockLeft)) {
                Game.fuel -= cost;
            }
        }
    }

    public IEnumerable<Vector2> EnumerateAllFilledBlocks() {
        yield return new Vector2(0, 0);
        yield return new Vector2(1, 0);
    }

    public void GetAllowedMoves(out bool left, out bool top, out bool right, out bool bottom) {
        throw new NotImplementedException();
    }
}
