using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Game : MonoBehaviour {
    public static Game Instance { get; private set; }

    public static Prefabs Prefabs { get; private set; }

    void Start() {
        Instance = this;
        Prefabs = GameObject.Find("Prefabs").GetComponent<Prefabs>();

        Debug.Log(Prefabs == null
                  ? "Failed to find a Prefabs child"
                  : "Initialized Prefabs container");
    }

    void Update() {

    }
}
