using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System;

public class MainMenu : MonoBehaviour
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
    public Button DifficultyButton;
    public Button DifficultyBack;
    public Button AdvancedDifficulty;
    public Button AdvancedBack;

    public AudioClip menuTheme;

    public string Difficulty;
    public int DivergenceRate;

    public List<GameObject> ObjToBeInitialized;

    // Start is called before the first frame update
    void Start()
    {
        //change brightness
        GlobalPostProcessingSettings.Instance.SetGammaAlpha(PlayerPrefs.GetInt("Brightness"));
        //change audio settings
        AudioListener.volume = PlayerPrefs.GetInt("MasterVolume") / 100f;

        //play menu theme
        this.GetComponent<AudioSource>().clip = menuTheme;
        this.GetComponent<AudioSource>().loop = true;
        //change volume
        this.GetComponent<AudioSource>().volume = PlayerPrefs.GetInt("MusicVolume") / 100f;
        this.GetComponent<AudioSource>().Play();
        ExitGame.onClick.AddListener(ExitButtonEvent);
        ChooseLevel.onClick.AddListener(LevelSelectionEvent);
        SettingsButton.onClick.AddListener(SettingsButtonEvent);
        StatsButton.onClick.AddListener(StatsButtonEvent);
        StatsBack.onClick.AddListener(StatsBackEvent);

        AdvancedDifficulty.onClick.AddListener(AdvancedDifficultyEvent);
        AdvancedBack.onClick.AddListener(AdvancedBackEvent);
        
        //add a hover listener for choose level
        EventTrigger trigger = ChooseLevel.GetComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerEnter;
        entry.callback.AddListener((data) => { ChooseLevel.GetComponentInChildren<Text>().color = Color.red; });

        //initialize all objects
        foreach(GameObject obj in ObjToBeInitialized) {
            obj.SetActive(true);
        }

        foreach(GameObject obj in ObjToBeInitialized) {
            obj.SetActive(false);
        }

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
        Settings.gameObject.SetActive(true);
        Settings.GetComponent<SettingsMenu>().Open();
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
