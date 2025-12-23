using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using TMPro;

public class EndGameScreen : MonoBehaviour
{
    public TMP_Text score;
    public TMP_Text GameOver;
    public Button ReturnToMenuButton;
    public Button RetryButton;
    public void ReturnToMenu() {
        PlayerDataManager.Instance.SavePlayerData();
        SceneManager.LoadScene("MainMenuScene");
    }

    public void Retry() {
        PlayerDataManager.Instance.SavePlayerData();
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

    // Start is called before the first frame update
    void Start() {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        float finalScore = GameSystem.Instance.CalculateScore();

        score.text = "Score: " + finalScore;

        if(GameSystem.Instance.Won == true) {
            GameOver.text = "Puncture Stabilized!";
        }
        else if(GameSystem.Instance.GameOver == true) {
            GameOver.text = "Game Over";
        }
        if(GameSystem.Instance.endReason == "puncture") {
            GameOver.text = "Taken by the Puncture...";
        }
        RetryButton.onClick.AddListener(Retry);
        ReturnToMenuButton.onClick.AddListener(ReturnToMenu);
    }
}
