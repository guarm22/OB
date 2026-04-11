using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using TMPro;
using DG.Tweening.Plugins.Options;

public class EndGameScreen : MonoBehaviour
{
    public TMP_Text score;
    public TMP_Text GameOver;
    public Button ReturnToMenuButton;
    public Button RetryButton;

    public TMP_Text timerText;

    public GameObject defaultUI;

    public FillBar divReportedBar;
    public TMP_Text divReportedText;

    public void ReturnToMenu() {
        PlayerDataManager.Instance.SavePlayerData();
        PlayerUI.paused = false;
        SceneManager.LoadScene("MainMenuScene");
    }

    public void Retry() {
        PlayerDataManager.Instance.SavePlayerData();
        PlayerUI.paused = false;
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

    // Start is called before the first frame update
    void Start() {
        PostProcessingControl.Instance.EndGameScreenPostProcessing();
        defaultUI.SetActive(false);
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        float finalScore = GameSystem.Instance.CalculateScore();

        if(DivergenceControl.Instance.DivergencesSpawned == 0) {
            DivergenceControl.Instance.DivergencesSpawned = 1;
        }

        float divReportedPercent = (float)GameSystem.Instance.AnomaliesSuccesfullyReportedThisGame / DivergenceControl.Instance.DivergencesSpawned * 1.0f;
        divReportedBar.SetPercent(divReportedPercent);
        divReportedText.text = GameSystem.Instance.AnomaliesSuccesfullyReportedThisGame + " of " + DivergenceControl.Instance.DivergencesSpawned + " DIVERGENCES REPORTED (" + (divReportedPercent * 100).ToString("F0") + "%)";

        score.text = "" + finalScore.ToString("F0");
        timerText.text = GameSystem.Instance.gameTimer.text;

        if(GameSystem.Instance.Won == true) {
            GameOver.text = "PUNCTURE STABILIZED";
        }
        else if(GameSystem.Instance.GameOver == true) {
            GameOver.text = "FAILURE";
        }
        if(GameSystem.Instance.endReason == "puncture") {
            GameOver.text = "TAKEN BY THE PUNCTURE";
        }
        RetryButton.onClick.AddListener(Retry);
        ReturnToMenuButton.onClick.AddListener(ReturnToMenu);
    }
}
