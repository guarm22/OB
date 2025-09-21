using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AchievementMenu : MonoBehaviour
{
    public GameObject achievementPrefab;
    public Transform achievementParent;
    public List<Achievement> achievements = new List<Achievement>();
    public GameObject noAchievementsText;
    
    public String currentProfile;

    private void LoadAchievements() {
        if(PFileUtil.Load<JsonWrapperUtil<Achievement>>("achievementList.json") == null) {
            return;
        }
        achievements = PFileUtil.Load<JsonWrapperUtil<Achievement>>("achievementList.json").list;
        Debug.Log("Loaded " + achievements.Count + " achievements");
    } 

    private void CreateAchievements() {
        if(achievements.Count == 0) {
            noAchievementsText.SetActive(true);
            achievementParent.gameObject.SetActive(false);
            return;
        }

        noAchievementsText.SetActive(false);
        achievementParent.gameObject.SetActive(true);
        int y = 0;
        foreach(Achievement a in achievements) {
            GameObject achievement = Instantiate(achievementPrefab, achievementParent);
            achievement.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, y);
            y -= 150;

            String dateEarnedText = "Date Earned: ";
            if(a.dateEarned == "") {
                dateEarnedText = "Not Unlocked";
            }

            achievement.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = a.Name;
            achievement.transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = a.Description;
            achievement.transform.GetChild(2).GetComponent<TMPro.TextMeshProUGUI>().text = a.Progress + "/" + a.Goal;
            achievement.transform.GetChild(3).GetComponent<TMPro.TextMeshProUGUI>().text = dateEarnedText + a.dateEarned;
        }
    }


    void InitData() {
        currentProfile = PlayerPrefs.GetString("currentProfile", "default");
        LoadAchievements();

        CreateAchievements();
    }

    void Start() {
        InitData();

    }

    void Update() {
        if(currentProfile != PlayerPrefs.GetString("currentProfile")) {
            achievements.Clear();
            InitData();
            currentProfile = PlayerPrefs.GetString("currentProfile");
        }
    }
}
