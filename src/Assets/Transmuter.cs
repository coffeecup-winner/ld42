using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Transmuter : MonoBehaviour, IMovable {
    private int areaSize = 1;

    void Upgrade() {
        // TODO: make checks against research

        if (areaSize < 3) {
            areaSize++;
        } else {
            Debug.Log("Nothing to upgrade");
            return;
        }

        // Hard-coded against prefab
        var activeColor = transform.GetChild(3).GetChild(0).GetComponent<SpriteRenderer>().color;
        var nextSizeChildren = transform.GetChild(areaSize == 2 ? 4 : 5);
        for (int i = 0; i < nextSizeChildren.childCount; i++) {
            nextSizeChildren.GetChild(i).GetComponent<SpriteRenderer>().color = activeColor;
        }
        if (areaSize == 3) {
            transform.GetChild(2).GetComponent<SpriteRenderer>().enabled = false;
        }
    }

    void OnMouseDown() {
        var clickPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (Math.Abs(clickPos.y - transform.position.y) <= 0.5f) {
            if (Math.Abs(clickPos.x - (transform.position.x + 1)) <= 0.5f) {
                // continue
            } else if (Math.Abs(clickPos.x - (transform.position.x + 2)) <= 0.5f) {
                Upgrade();
                return;
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
