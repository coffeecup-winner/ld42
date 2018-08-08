using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotPacMan : MonoBehaviour
{
    float m_speed;
    Player m_player;

    void Start() {
        m_speed = 60.0f;
        m_player = GameObject.Find("Player").GetComponent<Player>();
    }
    
    void Update() {
        Vector3 targetDir = m_player.transform.position - transform.position;
        float toward = Vector3.Angle(targetDir, Vector3.right);
        transform.Rotate(toward * Vector3.forward);
    }
}
