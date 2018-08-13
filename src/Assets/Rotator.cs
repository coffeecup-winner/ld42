using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Rotator : MonoBehaviour, IMovable {
    private int areaSize = 2;
    private GameObject activeArea3x3;

    void Awake() {
        activeArea3x3 = transform.Find("Active Area (size = 3)").gameObject;
        activeArea3x3.SetActive(false);
    }

    public void UpgradeSize() {
        if (areaSize == 2) {
            areaSize = 3;
            activeArea3x3.SetActive(true);
        } else {
            Debug.Log("Nothing to upgrade");
            return;
        }
    }

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

        var groups = GameObject
            .FindGameObjectsWithTag("Block")
            .Where(b =>
                   b.transform.position.x >= transform.position.x &&
                   b.transform.position.x <= transform.position.x + areaSize - 1 &&
                   b.transform.position.y >= transform.position.y + 1 &&
                   b.transform.position.y <= transform.position.y + areaSize)
            .GroupBy(b => b.transform.parent)
            .ToDictionary(g => g.Key.gameObject, g => g);

        if (!groups.All(g => g.Key.transform.childCount == g.Value.Count())) {
            Debug.Log("Not all figures are completely in the rotator active area");
            return;
        }

        if (!groups.Any()) {
            Debug.Log("Nothing to rotate");
            return;
        }

        if (Game.fuel < Game.rotationCost) {
            UiStuff.Instance.flashOutOfFuel();
            return;
        }
        Game.fuel -= Game.rotationCost;

        var rotatorAreaBottomLeft = (Vector2)transform.position + new Vector2(0, 1.0f);
        foreach (var group in groups) {
            group.Key.GetComponent<Figure>().Rotate(rotatorAreaBottomLeft, areaSize, rotateCW);
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
