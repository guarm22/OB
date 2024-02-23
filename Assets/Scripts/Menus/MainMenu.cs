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
        
    }

    private void LoadStats() {
        AnomaliesStat.text = "Anomalies Successfully Reported: " + this.AnomaliesSuccesfullyReported;
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
