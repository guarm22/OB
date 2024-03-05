using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Popup : MonoBehaviour
{

    public static Popup Instance;

    public TMP_Text PopupText;
    public Button CloseButton;
    public bool isPopupOpen = false;

    public void OpenPopup(string message) {
        SC_FPSController.paused = true;
        isPopupOpen = true;
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        PopupText.SetText(message);
        this.gameObject.SetActive(true);
    }

    public void ClosePopup() {
        SC_FPSController.paused = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        this.gameObject.SetActive(false);
        isPopupOpen = false;
    }
    // Start is called before the first frame update
    void Start() {
        if (Instance != null) {
            Debug.LogError("There is more than one instance!");
            return;
        }
        Instance = this;
        PopupText = this.transform.GetChild(0).GetChild(1).GetComponent<TMP_Text>();
        CloseButton = this.transform.GetChild(0).GetChild(2).GetComponent<Button>();
        this.gameObject.SetActive(false);
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
                else if(selectedObject.name.Equals("ClosePopup")) {
                    ClosePopup();
                }
            }
        }
    }
}
