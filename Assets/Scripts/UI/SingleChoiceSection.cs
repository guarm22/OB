using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SingleChoiceSection : MonoBehaviour
{
    //fill out list in the inspector
    public List<Button> choices;
    [HideInInspector]
    public Button currentChoice;
    public Image underline;

    public void SetChoice(Button choice) {
        currentChoice = choice;
        foreach (Button b in choices) {
            if (b.GetComponentInChildren<TMPro.TMP_Text>().text == choice.GetComponentInChildren<TMPro.TMP_Text>().text) {
                b.GetComponentInChildren<TMPro.TMP_Text>().color = Color.white;
                underline.transform.position = new Vector3(choice.transform.position.x, choice.transform.position.y - 50, choice.transform.position.z);
            }
            else {
                //set color of button text
                b.GetComponentInChildren<TMPro.TMP_Text>().color = new Color(172/255f, 187/255f, 207/255f, 1);
            }
        }
    }

    //Set this selections choice based on the string passed in
    //First argument is the value you want to change it to
    //Second value is the name of the GameObject you want to change
    public void SetChoice(String choice) {
        //make sure the choice matches one of the possible options
        foreach (Button b in choices) {
            if (b.GetComponentInChildren<TMPro.TMP_Text>().text == choice) {
                b.GetComponentInChildren<TMPro.TMP_Text>().color = Color.white;
                SetChoice(b);
            }
            else {
                //set color of button text
                b.GetComponentInChildren<TMPro.TMP_Text>().color = new Color(172/255f, 187/255f, 207/255f, 1);
            }
        }
    }

    public String GetCurrentChoice() {
        return currentChoice.GetComponentInChildren<TMPro.TMP_Text>().text;
    }

    void Start() {
        //add listeners for each button in the list
        foreach (Button b in choices) {
            b.onClick.AddListener(delegate { SetChoice(b); });
        }
    }

    // Update is called once per frame
    void Update() {
        
    }
}
