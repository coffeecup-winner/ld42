using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotPacMan : MonoBehaviour
{
    float m_speed;

    void Start() {
        m_speed = 60.0f;
    }
    
    void Update() {
        transform.Rotate(m_speed * Vector3.back * Time.deltaTime);
    }
}
