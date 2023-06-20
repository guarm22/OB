using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DefaultUI : MonoBehaviour
{

    public Text label;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void SetText() {
        if(Time.time - GameSystem.LastGuess < 10 &&  GameSystem.Guessed) {
            label.text = "Verifying...";
        }

        else if(Time.time - GameSystem.LastGuess >= 10 && GameSystem.CorrectGuess && GameSystem.Guessed) {
            label.text = "CORRECT";
        }
        else if(Time.time - GameSystem.LastGuess >= 10 && !GameSystem.CorrectGuess && GameSystem.Guessed) {
            label.text = "WRONG";
        }
        else {
            label.text = "TAB";
        }
    }

    // Update is called once per frame
    void Update()
    {
        SetText();
    }
}
