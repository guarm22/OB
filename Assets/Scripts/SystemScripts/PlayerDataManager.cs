using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerDataManager : MonoBehaviour
{
    [HideInInspector]
    public string currentProfile;

    public float dataUpdateFrequency;
    private float timeSinceLastUpdate;

    PlayerData playerData;

    public static PlayerDataManager Instance;
    void Start() {
        //load current profile from file
        Instance = this;
        currentProfile = PlayerPrefs.GetString("currentProfile", "default");

        //load player data from file
        playerData = PFileUtil.Load<PlayerData>("playerData.json");
        if(playerData == null) {
            Debug.Log("No player data file found, creating new one");
            playerData = new PlayerData();
            PFileUtil.Save("playerData.json", playerData);
        }

        timeSinceLastUpdate = 0;
    }

    // Update is called once per frame
    void Update() {
        if(PlayerUI.paused) {
            return;
        }

        if(timeSinceLastUpdate >= dataUpdateFrequency) {
            SavePlayerData();
            timeSinceLastUpdate = 0;
        }
        else {
            timeSinceLastUpdate += Time.deltaTime;
        }
    }

    public void SavePlayerData() {
        UpdateData();
        Debug.Log("Saving player data");
        PFileUtil.Save("playerData.json", playerData);
    }

    public static void SetProfile(string profile) {
        PlayerPrefs.SetString("currentProfile", profile);
    }

    private void UpdateData() {
        //update player data
        playerData.ReportsMade.value += GameSystem.Instance.ReportsMade;
        playerData.DivergencesReported.value += GameSystem.Instance.DivergencesReported;
        playerData.CreaturesReported.value += GameSystem.Instance.CreaturesReported;

        GameSystem.Instance.ReportsMade = 0;
        GameSystem.Instance.DivergencesReported = 0;
        GameSystem.Instance.CreaturesReported = 0;
    }

    public String GetDataValue(String name) {
        if(playerData == null) {
            return "0";
        }

        switch(name) {
            case "ReportsMade":
                return playerData.ReportsMade.value.ToString();
            case "DivergencesReported":
                return playerData.DivergencesReported.value.ToString();
            case "CreaturesReported":
                return playerData.CreaturesReported.value.ToString();
            default:
                return "0";
        }
    }

    public static string GetProfile() {
        return PlayerPrefs.GetString("currentProfile", "default");
    }
}
