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
    public Button ReturnToMenuButton;
    public Button RetryButton;
    public void ReturnToMenu() {
        SceneManager.LoadScene("MainMenuScene");
    }

    public void Retry() {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

    // Start is called before the first frame update
    void Start() {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;

        ADetected.text = "Divergences Found: " + GameSystem.Instance.AnomaliesSuccesfullyReportedThisGame;
        AMissed.text = "Divergences Missed: " + (GameSystem.Instance.AnomaliesSuccesfullyReportedThisGame - GameSystem.Instance.TotalAnomalies);

        if(GameSystem.Instance.Won == true) {
            GameOver.text = "You Won!";
        }
        RetryButton.onClick.AddListener(Retry);
        ReturnToMenuButton.onClick.AddListener(ReturnToMenu);

    }

    // Update is called once per frame
    void Update() {
    }
}
