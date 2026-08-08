using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SingleChoiceSection : MonoBehaviour
{
    //fill out list in the inspector
    public List<Button> choices;
    public Button currentChoice;
    public Image underline;

    private Color defaultColor = new Color(172/255f, 187/255f, 207/255f, 1);
    private Color notInteractableColor = new Color(172/255f, 187/255f, 207/255f, 0.5f);

    public void SetChoice(Button choice) {
        currentChoice = choice;
        float y = Display.main.systemHeight/42;
        foreach (Button b in choices) {
            if (b.GetComponentInChildren<TMPro.TMP_Text>().text == choice.GetComponentInChildren<TMPro.TMP_Text>().text) {
                b.GetComponentInChildren<TMPro.TMP_Text>().color = Color.white;
                MoveUnderline(choice.gameObject);
            }
            else {
                //set color of button text
                b.GetComponentInChildren<TMPro.TMP_Text>().color = new Color(172/255f, 187/255f, 207/255f, 1);
            }
        }
    }

    private void MoveUnderline(GameObject parent ) {
        float y = Display.main.systemHeight/42;
        underline.transform.DOKill();
        underline.transform.DOMove(new Vector3(parent.transform.position.x, 
        parent.transform.position.y - y, parent.transform.position.z), .25f);

        parent.GetComponentInChildren<TMP_Text>().ForceMeshUpdate();
        float newX = parent.GetComponentInChildren<TMP_Text>().GetRenderedValues(true).x;
        underline.rectTransform.DOSizeDelta(new Vector2(newX, underline.rectTransform.sizeDelta.y), .25f);
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

    public void SetInteractable(bool interactable) {
        foreach (Button b in choices) {
            b.interactable = interactable;
            b.GetComponentInChildren<TMPro.TMP_Text>().color = interactable ? defaultColor : notInteractableColor;
        }
        underline.color = interactable ? defaultColor : notInteractableColor;
    }
}
