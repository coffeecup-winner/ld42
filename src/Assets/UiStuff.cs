using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiStuff : MonoBehaviour
{
    Transform figureContainer;
    Transform draggedBlock;

    void Awake() {
        draggedBlock = null;
    }

    void Start() {
        figureContainer = GameObject.Find("Figures").transform;
    }
    
    void Update() {
        UpdateDragging();
    }

    void UpdateDragging() {
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0;

        // true for one frame only
        if (Input.GetMouseButtonUp(0)) {
            draggedBlock = null;
        }

        if (draggedBlock) {
            var figure = draggedBlock.parent;
            figure.position = mouseWorld - draggedBlock.localPosition;
            return;
        }

        // true for one frame only
        if (Input.GetMouseButtonDown(0)) {
            Collider2D collider = Physics2D.OverlapPoint(mouseWorld);
            if (!collider) {
                return;
            }
            var block = collider.GetComponent<Block>();
            if (block) {
                draggedBlock = block.transform;
                return;
            }
            // todo: drag buildings
        }
    }
}
