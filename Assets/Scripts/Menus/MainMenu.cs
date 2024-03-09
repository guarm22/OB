using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System;

public class MainMenu : MonoBehaviour, IDataPersistence
{
    public GameObject DefaultMenu;

    public GameObject LevelSelectionMenu;

    public GameObject Settings;

    public GameObject Stats;
    public GameObject GameSetting;
    public GameObject AdvancedSettings;

    public Text AnomaliesStat;

    private int AnomaliesSuccesfullyReported;
    private int LevelsWon;
    private int LevelsFailed;

    public Button ExitGame;
    public Button ChooseLevel;
    public Button LevelBack;
    public Button SettingsButton;
    public Button SettingsBack;
    public Button StatsButton;
    public Button StatsBack;
    public Button apartment;
    public Button tutorial;
    public Button DifficultyButton;
    public Button DifficultyBack;
    public Button AdvancedDifficulty;
    public Button AdvancedBack;

    public string Difficulty;
    public int DivergenceRate;

    public void LoadData(GameData data) {
        this.AnomaliesSuccesfullyReported = data.AnomaliesSuccesfullyReported;
        this.LevelsFailed = data.LevelsFailed;
        this.LevelsWon = data.LevelsWon;
    }

    public void SaveData(ref GameData data) {
        //data should not change on this scene
    }

    // Start is called before the first frame update
    void Start()
    {
        ExitGame.onClick.AddListener(ExitButtonEvent);
        ChooseLevel.onClick.AddListener(LevelSelectionEvent);
        LevelBack.onClick.AddListener(LevelBackEvent);
        SettingsButton.onClick.AddListener(SettingsButtonEvent);
        SettingsBack.onClick.AddListener(SettingsBackEvent);
        StatsButton.onClick.AddListener(StatsButtonEvent);
        StatsBack.onClick.AddListener(StatsBackEvent);
        apartment.onClick.AddListener(delegate { LoadLevel("Apartment_safe"); });
        tutorial.onClick.AddListener(delegate { LoadLevel("Tutorial"); });
        DifficultyButton.onClick.AddListener(DifficultyButtonEvent);
        DifficultyBack.onClick.AddListener(DifficultyBackEvent);
        AdvancedDifficulty.onClick.AddListener(AdvancedDifficultyEvent);
        AdvancedBack.onClick.AddListener(AdvancedBackEvent);
        GameSetting.SetActive(false);
        Settings.SetActive(false);
    }

    private void AdvancedBackEvent() {
        AdvancedSettings.SetActive(false);
        AdvancedBack.gameObject.SetActive(false);
        AdvancedDifficulty.gameObject.SetActive(true);
    }
    private void AdvancedDifficultyEvent() {
        AdvancedSettings.SetActive(true);
        AdvancedBack.gameObject.SetActive(true);
        AdvancedDifficulty.gameObject.SetActive(false);
    }

    private void DifficultyBackEvent() {
        GameSetting.SetActive(false);
        LevelSelectionMenu.SetActive(true);
    }

    private void DifficultyButtonEvent() {  
        GameSetting.SetActive(true);
        LevelSelectionMenu.SetActive(false);
    }

    private void LoadStats() {
        AnomaliesStat.text = "Anomalies Successfully Reported: " + this.AnomaliesSuccesfullyReported;
    }

    private void LevelSelectionEvent() {
        DefaultMenu.SetActive(false);
        LevelSelectionMenu.SetActive(true);
    }

    private void ExitButtonEvent() {
        Application.Quit();
    }
    private void SettingsButtonEvent() {
        DefaultMenu.SetActive(false);
        Settings.SetActive(true);
    }
    private void StatsButtonEvent() {
        DefaultMenu.SetActive(false);
        Stats.SetActive(true);
        LoadStats();
    }
    private void LevelBackEvent() {
        DefaultMenu.SetActive(true);
        LevelSelectionMenu.SetActive(false);
    }
    private void SettingsBackEvent() {
        DefaultMenu.SetActive(true);
        Settings.SetActive(false);
    }
    private void StatsBackEvent() {
        DefaultMenu.SetActive(true);
        Stats.SetActive(false);
    }
    private void LoadLevel(string level) {
        //sets difficulty settings before loading level
        GameSettings.Instance.SetValues();
        try {
            SceneManager.LoadScene(level);
        }
        catch(Exception e) {
            Debug.Log("Level does not exist: " +e);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
