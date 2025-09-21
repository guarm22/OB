using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;
using System.Collections;
using System;
using TMPro;
public class GameSystem : MonoBehaviour {
    public static GameSystem Instance { get; private set; }
    public AudioClip DisappearSound;
    public int GracePeriod;
    public float MaxEnergy = 100;
    public float CurrentEnergy;
    private float LastEnergyCheck=0f;
    private float EnergyCheckInterval=1f;
    public float energyPerSecond = 1.2f;
    public float gameTime;
    public TMP_Text gameTimer;
    public float startTime = 60*15f;
    [HideInInspector]
    public int AnomaliesSuccesfullyReportedThisGame;
    [HideInInspector]
    public bool GameOver = false;
    [HideInInspector]
    public bool Won = false;
    [HideInInspector]
    public string Difficulty;
    [HideInInspector]
    public int ReportsMade = 0;
    [HideInInspector]
    public int DivergencesReported = 0;
    [HideInInspector]
    public int CreaturesReported = 0;

    [HideInInspector]
    public int TimesPlayedOnEasy = 0;

    [HideInInspector]
    public int TimesPlayedOnNormal = 0;

    [HideInInspector]
    public int TimesPlayedOnHard = 0;

    [HideInInspector]
    public List<Report> reports = new List<Report>();

    public float TimeInLevel = 0f;
    public String endReason = "";

    private AudioSource audioSource;

    public bool shouldPlaySound = true;

    public float EnergyUsed = 0;

  void Awake() {
    if (Instance != null) {
      Debug.LogError("There is more than one instance!");
      return;
    }

    Instance = this;
    SetGameSettings();
    gameTime = startTime;
    AnomaliesSuccesfullyReportedThisGame = 0;
    CurrentEnergy = MaxEnergy;
    audioSource = gameObject.AddComponent<AudioSource>();
    gameObject.GetComponent<AudioSource>().clip = DisappearSound;
    //change volume
    audioSource.volume = PlayerPrefs.GetInt("AlertVolume")/100f;
    shouldPlaySound = PlayerPrefs.GetInt("No Warnings", 0) == 1 && SceneManager.GetActiveScene().name != "Tutorial" ? false : true;
  }

  public static bool InEditor() {
    #if UNITY_EDITOR
    return true;
    #endif
    return false;
  }

  private void SetGameSettings() {
    if(InEditor()) {
        Difficulty = PlayerPrefs.GetString("lastChosenDiff", "Normal");
        return;
    }
    if(SceneManager.GetActiveScene().name == "Tutorial") {
        return;
    }
    Difficulty = PlayerPrefs.GetString("lastChosenDiff", "Normal");
    switch(PlayerPrefs.GetString("lastChosenDiff", "NotLoaded")) {
        case "NotLoaded":
            Difficulty = "Normal";
            energyPerSecond = GameSettings.NormalEPS;
            GracePeriod = GameSettings.NormalGracePeriod;
            break;
        default:
            Difficulty = PlayerPrefs.GetString("lastChosenDiff", "Normal");
            energyPerSecond = PlayerPrefs.GetFloat("EPS", 1.1f);
            GracePeriod = PlayerPrefs.GetInt("GracePeriod", 15);
            break;
    }

    if(Difficulty == "Easy") {
        TimesPlayedOnEasy++;
    }
    else if(Difficulty == "Normal") {
        TimesPlayedOnNormal++;
    }
    else if(Difficulty == "Hard") {
        TimesPlayedOnHard++;
    }
  
  }

    public void PlayDivergenceSound() {
        if(shouldPlaySound==false) {
            return;
        }
        GetComponent<AudioSource>().Play();
    }

    ///<summary>
    ///Sets the game time variable and updates the UI.
    ///</summary>
    public void SetGameTime(float t=-1) {
        if (Input.GetKeyDown(KeyCode.N)) {
            EndGame("manual");
            return;
        }
        if (t > 0) {
            gameTime = t;
        }
        if (gameTime <= 0.5f) {
            Won = true;
            EndGame("won");
            return;
        }
        TimeInLevel += Time.deltaTime;
        gameTime -= Time.deltaTime;
        int minutes = Mathf.FloorToInt(gameTime / 60);
        int seconds = Mathf.FloorToInt(gameTime % 60);
        gameTimer.text = $"{minutes:D2}:{seconds:D2}";
    }
    
    private void UpdateEnergy() {
        if(Time.time - LastEnergyCheck >= EnergyCheckInterval) {
            ChangeEnergy(energyPerSecond);
            LastEnergyCheck = Time.time;
        }
    }

    public void ChangeEnergy(float amount) {
        CurrentEnergy += amount;
        if(amount < 0) {
            EnergyUsed += amount;
        }

        if(CurrentEnergy >= 100) {
            CurrentEnergy = 100;
        }
        else if(CurrentEnergy <= 0) {
            CurrentEnergy = 0;
        }
    }

    public void EndGame(string reason="") {
        Debug.Log("Reason:" + reason);
        GameOver = true;
        endReason = reason;
        AchievementManager.Instance.CheckLevelFinishAchievements(SceneManager.GetActiveScene().name, Difficulty, reason);
        PlayerDataManager.Instance.EndGameStats(TimeInLevel);
    }

    void Update() {
        if(PlayerUI.paused || GameOver) {
            return;
        };
        SetGameTime();
        UpdateEnergy();
    }
}
