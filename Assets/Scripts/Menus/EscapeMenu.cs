using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EscapeMenu : MonoBehaviour
{
    public GameObject escapeMenuUI;
    public GameObject defaultUI;

    public void ReturnToGame() {
        SC_FPSController.paused = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        escapeMenuUI.SetActive(false);
        defaultUI.SetActive(true);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0)) {
            if (EventSystem.current.IsPointerOverGameObject()) {
                // Get a reference to the selected UI game object
                GameObject selectedObject = EventSystem.current.currentSelectedGameObject;
                if(selectedObject == null) {
                    return;
                }
                else if(selectedObject.name.Equals("Return")) {
                    ReturnToGame();
                }
                else if(selectedObject.name.Equals("Main Menu")) {
                    //call some function that saves some data and returns player to main menu
                    Debug.Log("going home");
                }
            }
        }
    }
}
