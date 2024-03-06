using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SubmitGuess : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void SubmitAnswer() {
        if(TypeSelection.CurrentlySelected.Count == 0 || RoomSelection.CurrentlySelected == null) {
            return;
        }

        List<string> type = TypeSelection.CurrentlySelected;
        string room = RoomSelection.CurrentlySelected;
        GameSystem.MakeSelection(type,room);

        TypeSelection.CurrentlySelected.Clear();
        TypeSelection.Instance.ResetToggles();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0)) {
            if (EventSystem.current.IsPointerOverGameObject()) {
                // Get a reference to the selected UI game object
                GameObject selectedObject = EventSystem.current.currentSelectedGameObject;
                
                if(selectedObject == null || !selectedObject.name.Equals("AnswerSubmit")) {
                    return;
                }
                SubmitAnswer();
            }
        }
    }
}
