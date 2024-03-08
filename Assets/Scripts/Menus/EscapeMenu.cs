using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EscapeMenu : MonoBehaviour
{
    public GameObject escapeMenuUI;
    public GameObject defaultUI;

    public Button quitButton;
    public Button returnButton;

    public void ReturnToGame() {
        SC_FPSController.paused = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        defaultUI.SetActive(true);
        escapeMenuUI.SetActive(false);
    }

    public void QuitGame() {
        SceneManager.LoadScene("MainMenuScene");
    }

    // Start is called before the first frame update
    void Start()
    {
        if(defaultUI == null) {
            defaultUI = GameObject.Find("DefaultUI");
        }
        quitButton.onClick.AddListener(QuitGame);
        returnButton.onClick.AddListener(ReturnToGame);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0)) {
            if (EventSystem.current.IsPointerOverGameObject()) {
                // Get a reference to the selected UI game object
                GameObject selectedObject = EventSystem.current.currentSelectedGameObject;
                                Debug.Log(selectedObject.name);
                if(selectedObject == null) {
                    return;
                }
                else if(selectedObject.name.Equals("Return")) {
                    ReturnToGame();
                }
                else if(selectedObject.name.Equals("Main Menu")) {
                    //call some function that saves some data and returns player to main menu
                    QuitGame();
                }
            }
        }
    }
}
