using System.Collections;
using System.Collections.Generic;
using DG.Tweening.Core.Easing;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DefaultUI : MonoBehaviour
{
    public Text label;
    public TMP_Text flashlight;
    // Start is called before the first frame update
    void Start() {
        
    }

    void SetText() {
        bool beforeTimer = Time.time - GameSystem.LastGuess >= GameSystem.Instance.GuessLockout/2;
        bool afterTimer = Time.time - GameSystem.LastGuess <= GameSystem.Instance.GuessLockout;

        if(!GameSystem.Guessed) {
            label.text = "TAB";
        }
        else if(Time.time - GameSystem.LastGuess < GameSystem.Instance.GuessLockout/2 && GameSystem.Guessed) {
            label.text = "Verifying...";
        }
        else if(beforeTimer && GameSystem.Instance.wasLastGuessCorrect && afterTimer) {
            label.text = "CORRECT";
        }
        else if(beforeTimer && !GameSystem.Instance.wasLastGuessCorrect && afterTimer) {
            label.text = "WRONG";
        }

        if(Flashlight.Instance.beam.enabled) {
            flashlight.text = "Flashlight: ON";
        }
        else {
            flashlight.text = "Flashlight: OFF";
        }
    }

    // Update is called once per frame
    void Update() {
        if(PlayerUI.paused || GameSystem.Instance.GameOver) {
            return;
        }
        SetText();
    }
}
