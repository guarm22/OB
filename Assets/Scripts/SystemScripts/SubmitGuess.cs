using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SubmitGuess : MonoBehaviour {
    public Button SubmitButton;
    void Start() {
        SubmitButton.onClick.AddListener(SubmitAnswer);
    }
    void SubmitAnswer() {
        List<string> type = TypeSelection.CurrentlySelected;
        string room = RoomSelection.CurrentlySelected;
        DivergenceControl.Instance.MakeSelection(type,room);
    }
}
