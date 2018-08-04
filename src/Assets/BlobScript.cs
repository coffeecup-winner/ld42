using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;

public class BlobScript : MonoBehaviour {
    void Start () {
        var fileData = File.ReadAllBytes("Assets/Textures/bullet.png");
        var texture = new Texture2D(2, 2);
        texture.LoadImage(fileData);
        var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(1.0f, 1.0f));

        var bullet = new GameObject();
        var renderer = bullet.AddComponent<SpriteRenderer>();
        renderer.sprite = sprite;

        bullet.transform.parent = transform;
    }

    void Update () {
        transform.position =
            Vector3.up * 3.0f * (float)Math.Sin(Time.time) +
            Vector3.right * 3.0f * (float)Math.Cos(Time.time);
    }
}
