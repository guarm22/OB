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
        Debug.Log("RoomSelection.CurrentlySelected: " + RoomSelection.CurrentlySelected);
        if(TypeSelection.CurrentlySelected.Count == 0 || RoomSelection.CurrentlySelected == null) {
            return;
        }
        List<string> type = TypeSelection.CurrentlySelected;
        string room = RoomSelection.CurrentlySelected;
        GameSystem.Instance.MakeSelection(type,room);
    }
}
