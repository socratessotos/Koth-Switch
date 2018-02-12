using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour {

    public Text instructionText;

    float[] timers = new float[4];
    public float quitTime = 2f;

    int biggestTimerIndex = 0;

    int minTextSize;
    int maxTextSize;

    void Start() {
        ResetTimers();

        minTextSize = instructionText.fontSize;
        maxTextSize = minTextSize * 3;
    }

    void FixedUpdate() {

        for (int i = 0, l = timers.Length; i < l; i++) {
            
            if (GameController.instance.playerInputs[i].snapshot.aButton.pressed && GameController.instance.playerInputs[i].snapshot.bButton.pressed) {
                
                timers[i] += Time.fixedDeltaTime;

                if (timers[i] > timers[biggestTimerIndex])
                    biggestTimerIndex = i;

                if (timers[i] >= quitTime) 
                    Quit();

            } else {
                timers[i] = 0;
            }

        }

        AdjustTextSize();
    }

    void Quit() {
        ResetTimers();
        GameController.instance.ReturnToMainMenu();
    }

    void ResetTimers() {
        for (int i = 0; i < timers.Length; i++) {
            timers[i] = 0;
        }
    }

    void AdjustTextSize() {

        int textSize = minTextSize + (int) (maxTextSize * (timers[biggestTimerIndex] / quitTime));

        instructionText.fontSize = textSize;
    }

    void OnDisable() {
        instructionText.fontSize = minTextSize;
    }

}
