using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class EndGameScreen : MonoBehaviour
{
    public Text ADetected;
    public Text AMissed;
    public Text GameOver;


    public void ReturnToMenu() {
        SceneManager.LoadScene(0);
    }

    public void Retry() {
        SceneManager.LoadScene(2);
    }

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;

        ADetected.text = "Divergences Found: " + GameSystem.Instance.AnomaliesSuccesfullyReportedThisGame;
        AMissed.text = "Divergences Missed: " + (GameSystem.Instance.TotalAnomalies - GameSystem.Instance.AnomaliesSuccesfullyReportedThisGame);

        if(GameSystem.Instance.Won == true) {
            GameOver.text = "You Won!";
        }
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
                else if(selectedObject.name.Equals("Return to menu")) {
                    ReturnToMenu();
                }
                else if(selectedObject.name.Equals("Retry")) {
                    //call some function that saves some data and returns player to main menu
                    Retry();
                }
            }
        }
    }
}
