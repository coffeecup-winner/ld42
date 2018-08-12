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

        if (draggedBlock)

        // true for one frame only
        if (Input.GetMouseButtonUp(0)) {
            if (draggedBlock) {
                var t = draggedBlock.parent;
                t.position = new Vector3(Mathf.Round(t.position.x), Mathf.Round(t.position.y));
                draggedBlock = null;
            }
        }

        if (draggedBlock) {
            Vector2 currentPos = (Vector2)draggedBlock.position;
            Vector2 snappedPos = new Vector2(Mathf.Round(currentPos.x), Mathf.Round(currentPos.y));
            Vector2 posToTarget = (Vector2)mouseWorld - currentPos;
            Vector2 gridToTarget = (Vector2)mouseWorld - snappedPos;
            Vector2 absDelta = new Vector2(Mathf.Abs(gridToTarget.x), Mathf.Abs(gridToTarget.y));
            Vector3 outputMove = Vector3.zero;

            if (absDelta.x >= 1.0f || absDelta.y >= 1.0f) {
                // long drag: move along the major axis, snap to the minor one
                float major = Mathf.Max(absDelta.x, absDelta.y);
                float minor = Mathf.Min(absDelta.x, absDelta.y);
                float snappiness = Mathf.Clamp01(major / 100 * Mathf.Max(0.1f, minor));

                if (absDelta.y >= absDelta.x) {
                    // snap to x, move along y
                    outputMove.x = snappiness * (Mathf.Round(currentPos.x) - currentPos.x);
                    outputMove.y = Mathf.Clamp(posToTarget.y, -1.0f, 1.0f);
                    Debug.Log(string.Format("x snap gridToTarget=({0:0.00}, {1:0.00})", gridToTarget.x, gridToTarget.y));
                }
                else {
                    // move along x, snap to y
                    outputMove.x = Mathf.Clamp(posToTarget.x, -1.0f, 1.0f);
                    outputMove.y = snappiness * (Mathf.Round(currentPos.y) - currentPos.y);
                }
            }
            else {
                // short drag: allow free movement within the vicinity of the nearest gridpoint
                float d = 0.50f;  // [0, 1] default 0.5, higher is looser (how far away the circle is from the gridpoint)
                float r = 0.45f;  // [0, 1] default 0.5, higher is tigher (circle radius)
                var circleCenter = new Vector2(gridToTarget.x >= 0 ? d : (-d), gridToTarget.y >= 0 ? d : (-d));
                float t = RaycastVectorCircle(gridToTarget, circleCenter, r);
                outputMove = (snappedPos + t * gridToTarget) - currentPos;

                // if (posToTarget.sqrMagnitude > 0.0001f) {
                //     Debug.Log(string.Format(
                //         "short pos=({0:0.00}, {1:0.00}) drag=({2:0.00}, {3:0.00}), gridToTarget=({4:0.00} {5:0.00}), out=({6:0.00} {7:0.00}), t={8:0.00}",
                //         currentPos.x, currentPos.y, posToTarget.x, posToTarget.y, gridToTarget.x, gridToTarget.y, outputMove.x, outputMove.y, t
                //     ));
                // }
            }

            // move the figure, not the block
            draggedBlock.parent.position += outputMove;
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

    public static float RaycastVectorCircle(Vector2 v, Vector2 center, float r) {
        // WARNING: this doesn't work in the general case
        // some assumptions:
        //   v is anchored at (0, 0)
        //   v is bounded by center - (r, r) and center + (r, r)
        // returns a float close to
        //   0 if (0, 0) is within the circle
        //   1 if v doesn't intersect the circle at all
        //   t such that v*t is the intersection point
        float a = 0;
        float b = 1;
        float rr = r * r;
        for (int i = 0; i < 12; ++i) {
            float s = 0.5f * (a + b);
            if ((v * s - center).sqrMagnitude < rr)
                b = s;
            else
                a = s;
        }
        return a;
    }
}
