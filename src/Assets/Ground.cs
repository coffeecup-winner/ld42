using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Ground : MonoBehaviour {
    private Color normalColor;
    private Color mouseOverColor;

    void Start() {
        normalColor = GetComponent<SpriteRenderer>().color;
        mouseOverColor = new Color(normalColor.r * 1.1f, normalColor.g * 1.1f, normalColor.b * 1.1f);
    }

    void Update() {
    }

    void OnMouseEnter() {
        GetComponent<SpriteRenderer>().color = mouseOverColor;
    }

    void OnMouseExit() {
        GetComponent<SpriteRenderer>().color = normalColor;
    }
}
