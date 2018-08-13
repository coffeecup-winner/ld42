using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TutorialSequence : MonoBehaviour {
    public Vector2 greenPos;
    public Vector2 redPos;
    public Vector2 bluePos;
    public Vector2 sawPos;

    public Vector2 basePos;
    private int state = 0;
    private Text tutorial;

    void Start() {
        basePos = greenPos;
        tutorial = GameObject.Find("TutorialText").GetComponent<Text>();
    }

    void Update() {
        switch (state) {
            case 0:
                if (Game.fuel != 10) {
                    tutorial.text = "Good! Now, move take the blue square and move it to the blue output.";
                    basePos = bluePos;
                    state = 1;
                }
            break;
            case 1:
                if (Game.research != 0) {
                    tutorial.text = "Guess what to do with the red block!";
                    basePos = redPos;
                    state = 2;
                }
            break;
            case 2:
                if (Game.fuel != 12) {
                    tutorial.text = "Last one: drag the green shape onto the saw to cut it in two.";
                    basePos = sawPos;
                    state = 3;
                }
            break;
            case 3:
                if (Game.fuel != 11) {
                    tutorial.text = "Last one: drag the green shape onto the saw to cut it in two.";
                    basePos = sawPos;
                    state = 4;
                }
            break;
            case 4:
                if (Game.fuel != 10) {
                    state = 5;
                }
            break;
            default:
                Destroy(tutorial.gameObject);
                Destroy(gameObject);
            break;
        }
        transform.position = (Vector3)(basePos + new Vector2(0, 0.5f * Mathf.Sin(2 * Time.time)));
    }
}
