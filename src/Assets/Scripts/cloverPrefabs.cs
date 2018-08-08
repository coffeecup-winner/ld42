using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cloverPrefabs : MonoBehaviour {

    // Use this for initialization
    void Start () {
    
    }
    
    // Update is called once per frame
    void Update () {
        var coef = Math.Abs((float)Math.Sin(Time.time));
        transform.localScale = new Vector3(1,1,1) * coef + new Vector3(1, 1, 1);
    }
}
