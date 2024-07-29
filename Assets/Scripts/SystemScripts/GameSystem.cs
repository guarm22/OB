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
    gameObject.AddComponent<AudioSource>();
    gameObject.GetComponent<AudioSource>().clip = DisappearSound;
  }

  public static bool InEditor() {
    #if UNITY_EDITOR
    return true;
    #endif
    return false;
  }

  private void SetGameSettings() {
    if(InEditor()) {
        Difficulty = PlayerPrefs.GetString("Difficulty", "Normal");
        return;
    }
    if(SceneManager.GetActiveScene().name == "Tutorial") {
        return;
    }
    Debug.Log("Difficulty: " + PlayerPrefs.GetString("Difficulty", "Normal"));
    switch(PlayerPrefs.GetString("Difficulty", "NotLoaded")) {
        case "NotLoaded":
            Difficulty = "Normal";
            energyPerSecond = GameSettings.NormalEPS;
            GracePeriod = GameSettings.NormalGracePeriod;
            break;
        default:
            Difficulty = PlayerPrefs.GetString("Difficulty", "Normal");
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
        GetComponent<AudioSource>().Play();
    }

    ///<summary>
    ///Sets the game time variable and updates the UI.
    ///</summary>
    public void SetGameTime(float t=-1) {
        if (Input.GetKeyDown(KeyCode.N)) {
            StartCoroutine(EndGame("manual"));
            return;
        }
        if (t > 0) {
            gameTime = t;
        }
        if (gameTime <= 0.5f) {
            Won = true;
            StartCoroutine(EndGame("won"));
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
        if(CurrentEnergy >= 100) {
            CurrentEnergy = 100;
        }
        else if(CurrentEnergy <= 0) {
            CurrentEnergy = 0;
        }
    }

    public IEnumerator EndGame(string reason="") {
        GameOver = true;
        endReason = reason;
        if(reason.Equals("zombie") || reason.Equals("yippie")) {
            yield return StartCoroutine(CreatureControl.Instance.ZombieJumpscare());
        }
        CreatureControl.Instance.IsJumpscareFinished=true;
        AchievementManager.Instance.CheckLevelFinishAchievements(SceneManager.GetActiveScene().name, Difficulty, reason);
        PlayerDataManager.Instance.EndGameStats(startTime-gameTime);
    }

    void Update() {
        if(PlayerUI.paused || GameOver) {
            return;
        };
        SetGameTime();
        UpdateEnergy();
    }
}
