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
            var t = draggedBlock.parent;
            t.position = new Vector3(Mathf.Round(t.position.x), Mathf.Round(t.position.y));
            draggedBlock = null;
        }

        if (draggedBlock) {
            var currentPos = (Vector2)draggedBlock.position;
            var snappedPos = new Vector2(Mathf.Round(currentPos.x), Mathf.Round(currentPos.y));
            var delta = (Vector2)mouseWorld - currentPos;
            var copy = new Vector2(delta.x, delta.y);

            // Debug.Log(string.Format("target = ({0:0.00}, {1:0.00})", mouseWorld.x, mouseWorld.y));

            // the smaller this coefficient (0 - 0.5), the more figures get stuck when dragging
            // when it's large, they're slippery and easily snap to grid (in the minor direction)
            bool xSnap = Mathf.Abs(currentPos.x - snappedPos.x) < 0.4f;
            bool ySnap = Mathf.Abs(currentPos.y - snappedPos.y) < 0.4f;
            if (!xSnap && !ySnap) {
                // this should never happen, but snap to whichever axis is the closest
                Debug.LogWarning("UiStuff.UpdateDragging: block snapped to neither axis!", draggedBlock);
                xSnap = Mathf.Abs(currentPos.x - snappedPos.x) <= Mathf.Abs(currentPos.y - snappedPos.y);
                ySnap = !xSnap;
            }
            else if (xSnap && ySnap) {
                // allow free movement in the close vicinity of the grid points
                if (((currentPos + delta) - snappedPos).sqrMagnitude <= 0.1 * 0.1) {
                    // snapping on either axis in this case would lead to oscillating between two points
                    xSnap = ySnap = false;  // todo: figure out how to not oscillate
                }
                else {  // if leaving the cozy grid point, then snap based on the direction of the drag
                    xSnap = Mathf.Abs(delta.y) >= Mathf.Abs(delta.x);
                    ySnap = !xSnap;
                }
            }

            if (xSnap) {  // snap to x, move along y
                delta.x = Mathf.Round(currentPos.x) - currentPos.x;
                delta.y = Mathf.Clamp(delta.y, -1.0f, 1.0f);
                // Debug.Log(string.Format("xSnap ({0:0.00}, {1:0.00}) => ({2:0.00} {3:0.00})", copy.x, copy.y, delta.x, delta.y));
            }

            if (ySnap) {  // move along x, snap to y
                delta.x = Mathf.Clamp(delta.x, -1.0f, 1.0f);
                delta.y = Mathf.Round(currentPos.y) - currentPos.y;
                // Debug.Log(string.Format("ySnap ({0:0.00}, {1:0.00}) => ({2:0.00} {3:0.00})", copy.x, copy.y, delta.x, delta.y));
            }

            // move the figure, not the block
            draggedBlock.parent.position += (Vector3)delta;
            // Debug.Log(string.Format("pos = ({0:0.00}, {1:0.00})", draggedBlock.parent.position.x, draggedBlock.parent.position.y));
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
