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
        //Debug.Log("Saving player data");
        PFileUtil.Save("playerData.json", playerData);
    }

    public static void SetProfile(string profile) {
        PlayerPrefs.SetString("currentProfile", profile);
    }

    private void UpdateData() {
        //update player data
        playerData.ReportsMade.value += GameSystem.Instance.ReportsMade;
        playerData.DivergencesReported.value += GameSystem.Instance.DivergencesReported;
        playerData.CreaturesReported.value += CreatureControl.Instance.CreaturesReported;
        playerData.TimesCrouched.value += SC_FPSController.Instance.timesCrouched;
        playerData.TimesPlayedOnEasy.value += GameSystem.Instance.TimesPlayedOnEasy;
        playerData.TimesPlayedOnNormal.value += GameSystem.Instance.TimesPlayedOnNormal;
        playerData.TimesPlayedOnHard.value += GameSystem.Instance.TimesPlayedOnHard;

        GameSystem.Instance.ReportsMade = 0;
        GameSystem.Instance.DivergencesReported = 0;
        CreatureControl.Instance.CreaturesReported = 0;
        SC_FPSController.Instance.timesCrouched = 0;
        GameSystem.Instance.TimesPlayedOnEasy = 0;
        GameSystem.Instance.TimesPlayedOnNormal = 0;
        GameSystem.Instance.TimesPlayedOnHard = 0;
    }

    public void EndGameStats(float endTime) {
        playerData.TimeInLevel.value += ((int)Mathf.Ceil(endTime));

        UpdateData();
        SavePlayerData();
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

    public String SteamToPunctureValue(String s) {
        switch(s) {
            case "S_REPORTS":
                return "ReportsMade";
            case "S_CREATURESREPORTED":
                return "CreaturesReported";
            default:
                return "ReportsMade";
        }
    }

    public static string GetProfile() {
        return PlayerPrefs.GetString("currentProfile", "default");
    }
}
