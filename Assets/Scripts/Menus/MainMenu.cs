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
    public Button mariaApartment;
    public Button tutorial;
    public Button DifficultyButton;
    public Button DifficultyBack;
    public Button AdvancedDifficulty;
    public Button AdvancedBack;

    public AudioClip menuTheme;

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
        //play menu theme
        this.GetComponent<AudioSource>().clip = menuTheme;
        this.GetComponent<AudioSource>().loop = true;
        this.GetComponent<AudioSource>().Play();
        ExitGame.onClick.AddListener(ExitButtonEvent);
        ChooseLevel.onClick.AddListener(LevelSelectionEvent);
        LevelBack.onClick.AddListener(LevelBackEvent);
        SettingsButton.onClick.AddListener(SettingsButtonEvent);
        SettingsBack.onClick.AddListener(SettingsBackEvent);
        StatsButton.onClick.AddListener(StatsButtonEvent);
        StatsBack.onClick.AddListener(StatsBackEvent);
        apartment.onClick.AddListener(delegate { LoadLevel("Apartment_safe"); });
        tutorial.onClick.AddListener(delegate { LoadLevel("Tutorial"); });
        mariaApartment.onClick.AddListener(delegate { LoadLevel("Maria's Apartment"); });
        DifficultyButton.onClick.AddListener(DifficultyButtonEvent);
        DifficultyBack.onClick.AddListener(DifficultyBackEvent);
        AdvancedDifficulty.onClick.AddListener(AdvancedDifficultyEvent);
        AdvancedBack.onClick.AddListener(AdvancedBackEvent);
        DefaultMenu.SetActive(true);
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
        GameSetting.transform.localScale = new Vector3(0, 0, 0);
        LevelSelectionMenu.SetActive(true);
    }

    private void DifficultyButtonEvent() {  
        GameSetting.transform.localScale = new Vector3(1, 1, 1);
        LevelSelectionMenu.SetActive(false);
    }

    private void LoadStats() {
        AnomaliesStat.text = "Divergences Successfully Reported: " + this.AnomaliesSuccesfullyReported;
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
        Settings.transform.localScale = new Vector3(1, 1, 1);
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
        Settings.transform.localScale = new Vector3(0, 0, 0);
    }
    private void StatsBackEvent() {
        DefaultMenu.SetActive(true);
        Stats.SetActive(false);
    }
    private void LoadLevel(string level) {
        //sets difficulty settings before loading level
        LoadScreen.Instance.Loading = true;
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
