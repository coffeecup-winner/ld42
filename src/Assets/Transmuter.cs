using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Transmuter : MonoBehaviour, IMovable {
    private int areaSize = 0;

    private GameObject activeArea1x1;
    private GameObject activeArea2x2;
    private GameObject activeArea3x3;

    void Start() {
        activeArea1x1 = transform.Find("Active Area (size = 1)").gameObject;
        activeArea2x2 = transform.Find("Active Area (size = 2)").gameObject;
        activeArea3x3 = transform.Find("Active Area (size = 3)").gameObject;
        activeArea1x1.SetActive(false);
        activeArea2x2.SetActive(false);
        activeArea3x3.SetActive(false);
    }

    public void UpgradeSize(int newSize) {
        areaSize = newSize;
        if (areaSize >= 1)
            activeArea1x1.SetActive(true);
        if (areaSize >= 2)
            activeArea2x2.SetActive(true);
        if (areaSize >= 3)
            activeArea3x3.SetActive(true);
    }

    void OnMouseDown() {
        if (areaSize < 1)
            return;
        var clickPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (Math.Abs(clickPos.y - transform.position.y) <= 0.5f) {
            if (Math.Abs(clickPos.x - (transform.position.x + 1)) <= 0.5f) {
                // continue
            } else {
                return;
            }
        } else {
            return;
        }

        var leftBottom = new Vector2(transform.position.x, transform.position.y + 1);
        if (areaSize == 1) {
            leftBottom.x += 1;
        }
        var groups = GameObject
            .FindGameObjectsWithTag("Block")
            .Where(b =>
                   b.transform.position.x >= leftBottom.x &&
                   b.transform.position.x <= leftBottom.x + areaSize - 1 &&
                   b.transform.position.y >= leftBottom.y &&
                   b.transform.position.y <= leftBottom.y + areaSize - 1)
            .GroupBy(b => b.transform.parent)
            .ToDictionary(g => g.Key.gameObject, g => g);

        if (!groups.All(g => g.Key.transform.childCount == g.Value.Count())) {
            Debug.Log("Not all figures are completely in the transmuter active area");
            return;
        }

        if (!groups.Any()) {
            Debug.Log("Nothing to transmute");
            return;
        }

        if (Game.fuel < Game.transmutationCost) {
            UiStuff.Instance.flashOutOfFuel();
            return;
        }
        Game.fuel -= Game.transmutationCost;

        foreach (var group in groups) {
            var figure = group.Key.GetComponent<Figure>();
            switch (figure.Type) {
                case BlockType.Red:
                    figure.Type = BlockType.Blue;
                    break;
                case BlockType.Blue:
                    figure.Type = BlockType.Green;
                    break;
                default:
                    break;
            }

            foreach (var renderer in group.Key.transform.GetComponentsInChildren<SpriteRenderer>()) {
                renderer.color = Game.TypeToColor(figure.Type);
            }
            foreach (var block in group.Key.transform.GetComponentsInChildren<Block>()) {
                block.ResetColors();
            }
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
