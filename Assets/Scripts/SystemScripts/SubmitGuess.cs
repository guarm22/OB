using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SubmitGuess : MonoBehaviour
{

    public Button SubmitButton;
    // Start is called before the first frame update
    void Start()
    {
        SubmitButton.onClick.AddListener(SubmitAnswer);
    }

    void SubmitAnswer() {
        if(TypeSelection.CurrentlySelected.Count == 0 || RoomSelection.CurrentlySelected == null) {
            return;
        }

        List<string> type = TypeSelection.CurrentlySelected;
        string room = RoomSelection.CurrentlySelected;
        GameSystem.MakeSelection(type,room);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
