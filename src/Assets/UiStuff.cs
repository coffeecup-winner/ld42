using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiStuff : MonoBehaviour
{
    Transform figureContainer;
    Transform draggedThing;

    void Awake() {
        draggedThing = null;
    }

    void Start() {
        figureContainer = GameObject.Find("Figures").transform;
    }
    
    void Update() {
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0;

        // true for one frame only
        if (Input.GetMouseButtonUp(0)) {
            draggedThing = null;
        }

        if (draggedThing) {
            draggedThing.position = mouseWorld - (Vector3)(0.5f * Vector2.one);
            return;
        }

        // true for one frame only
        if (Input.GetMouseButtonDown(0)) {
            Collider2D x = Physics2D.OverlapPoint(mouseWorld);
            if (x) {
                draggedThing = x.transform;
                x.GetComponent<SpriteRenderer>().color = Color.red;
            }
        }

    }
}
