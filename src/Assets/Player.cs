using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;

public class Player : MonoBehaviour {
    private static readonly float Speed = 3.0f;
    private static readonly float StrafeSpeed = 3.0f;
    private static readonly float RotationSpeed = 170.0f;

    void Start() {
        // var fileData = File.ReadAllBytes("Assets/Textures/bullet.png");
        // var texture = new Texture2D(2, 2);
        // texture.LoadImage(fileData);
        // var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(1.0f, 1.0f));

        // var bullet = new GameObject();
        // var renderer = bullet.AddComponent<SpriteRenderer>();
        // renderer.sprite = sprite;

        // bullet.transform.parent = transform;
    }

    void Update() {
        float y_translation = Input.GetAxis("Vertical") * Speed * Time.deltaTime;
        float x_translation = Input.GetAxis("Strafe") * StrafeSpeed * Time.deltaTime;
        float rotation = Input.GetAxis("Horizontal") * RotationSpeed * Time.deltaTime;

        transform.Translate(x_translation, y_translation, 0);
        transform.Rotate(0, 0, -rotation);
    }
}
