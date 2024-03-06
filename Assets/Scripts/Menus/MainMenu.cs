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

    public void LoadData(GameData data) {
        this.AnomaliesSuccesfullyReported = data.AnomaliesSuccesfullyReported;
        Debug.Log(this.AnomaliesSuccesfullyReported);
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
        apartment.onClick.AddListener(delegate { LoadLevel("Apartment"); });
        tutorial.onClick.AddListener(delegate { LoadLevel("Tutorial"); });
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

     if(Input.GetMouseButtonDown(0)) {
            if (EventSystem.current.IsPointerOverGameObject()) {
                // Get a reference to the selected UI game object
                GameObject selectedObject = EventSystem.current.currentSelectedGameObject;

                if(selectedObject == null) {
                    return;
                }
                
                if(selectedObject.name.Equals("ExitGame")) {
                    Application.Quit();
                }
                else if(selectedObject.name.Equals("ChooseLevel")) {
                    DefaultMenu.SetActive(false);
                    LevelSelectionMenu.SetActive(true);
                }

                else if(selectedObject.name.Equals("LevelBack")) {
                    DefaultMenu.SetActive(true);
                    LevelSelectionMenu.SetActive(false);
                }

                else if(selectedObject.name.Equals("SettingsButton")) {
                    DefaultMenu.SetActive(false);
                    Settings.SetActive(true);
                }

                else if(selectedObject.name.Equals("SettingsBack")) {
                    DefaultMenu.SetActive(true);
                    Settings.SetActive(false);
                }

                else if(selectedObject.name.Equals("StatsBack")) {
                    DefaultMenu.SetActive(true);
                    Stats.SetActive(false);
                }

                else if(selectedObject.name.Equals("StatsButton")) {
                    DefaultMenu.SetActive(false);
                    Stats.SetActive(true);
                    LoadStats();
                }


                //Level Selection
                else {
                    try {
                        SceneManager.LoadScene(selectedObject.name);
                    }
                    catch(Exception e) {
                        Debug.Log("Level does not exist: " +e);
                    }
                }
                
            }
        }

    }
}
