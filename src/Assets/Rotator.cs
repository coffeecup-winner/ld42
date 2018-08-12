using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Rotator : MonoBehaviour, IMovable {
    void OnMouseDown() {
        var clickPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        bool rotateCW;

        if (Math.Abs(clickPos.y - transform.position.y) <= 0.5f) {
            if (Math.Abs(clickPos.x - transform.position.x) <= 0.5f) {
                rotateCW = false;
            } else if (Math.Abs(clickPos.x - (transform.position.x + 2)) <= 0.5f) {
                rotateCW = true;
            } else {
                return;
            }
        } else {
            return;
        }

        Debug.Log("Rotate " + (rotateCW ? "CW" : "CCW"));

        var groups = GameObject
            .FindGameObjectsWithTag("Block")
            .Where(b =>
                   b.transform.position.x > (transform.position.x - 0.1f) &&
                   b.transform.position.x < (transform.position.x + 2.1f) &&
                   b.transform.position.y > (transform.position.y + 0.9f) &&
                   b.transform.position.y < (transform.position.y + 3.1f))
            .GroupBy(b => b.transform.parent)
            .ToDictionary(g => g.Key.gameObject, g => g);

        if (!groups.All(g => g.Key.transform.childCount == g.Value.Count())) {
            Debug.Log("Not all figures are completely in the rotator active area");
            return;
        }

        foreach (var group in groups) {
            group.Key.GetComponent<Figure>().Rotate3x3(transform.position.x, transform.position.y + 1.0f);
        }
    }

    public IEnumerable<Vector2> EnumerateAllFilledBlocks() {
        yield return new Vector2(0, 0);
        yield return new Vector2(1, 0);
        yield return new Vector2(2, 0);
    }

    public void GetAllowedMoves(out bool left, out bool top, out bool right, out bool bottom) {
        throw new NotImplementedException();
    }
}
