using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DefaultUI : MonoBehaviour
{

    public Text label;

    public GameObject screen;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void SetText() {
        bool beforeTimer = Time.time - GameSystem.LastGuess >= GameSystem.Instance.GuessLockout-GameSystem.Instance.reportTextTimer;
        bool afterTimer = Time.time - GameSystem.LastGuess <= GameSystem.Instance.GuessLockout;

        if(!GameSystem.Guessed) {
            label.text = "TAB";
        }
        else if(Time.time - GameSystem.LastGuess < GameSystem.Instance.GuessLockout-5 &&  GameSystem.Guessed) {
            label.text = "Verifying...";
        }

        else if(beforeTimer && GameSystem.CorrectGuess && afterTimer) {
            label.text = "CORRECT";
        }
        else if(beforeTimer && !GameSystem.CorrectGuess && afterTimer) {
            label.text = "WRONG";
        }
    }

    // Update is called once per frame
    void Update()
    {
        SetText();
    }
}
