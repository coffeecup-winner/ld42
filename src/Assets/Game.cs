using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Game : MonoBehaviour {
    private Transform cloverContainer;

    public GameObject Clover;

    void Start() {
        cloverContainer = GameObject.Find("CloverContainer").transform;
    }
	
    void Update() {
        if (cloverContainer.childCount < 3) {
            var clover = Instantiate(Clover,
                new Vector3(Random.Range(-15.0f, 15.0f), Random.Range(-5.0f, 5.0f), 0),
                Quaternion.identity);
            
            clover.transform.parent = cloverContainer;
        }
    }
}
