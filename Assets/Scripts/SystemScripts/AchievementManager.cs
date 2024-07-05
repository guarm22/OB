using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementManager : MonoBehaviour
{
    public int checkFrequency = 20;
    private int timeSinceLastCheck = 0;

    public List<Achievement> achievements = new List<Achievement>();
    private JsonWrapperUtil<Achievement> wrapper;

    public static AchievementManager Instance;
    
    void Start() {
        //load list from file
        if(PFileUtil.Load<JsonWrapperUtil<Achievement>>("achievementList.json") == null) {
            Debug.Log("No achievement file found, creating new one");
            achievements = FirstTimeLoad();
            Save();
        }
        else {
            achievements = PFileUtil.Load<JsonWrapperUtil<Achievement>>("achievementList.json").list;
        }

        AddNewAchievements();
    }

    void Save() {
        PFileUtil.Save("achievementList.json", new JsonWrapperUtil<Achievement>(achievements));
    }

    private void AddNewAchievements() {
        List<Achievement> newAchievements = new List<Achievement> {
            new Achievement("Beat Tutorial", "Complete the tutorial", false, 1),
            new Achievement("Beat Cabin", "Complete the cabin level", false, 1),
            new Achievement("Beat Graveyard", "Complete the Graveyard level", false, 1),
            new Achievement("Beat Normal", "Complete a level on normal", false, 1),
            new Achievement("Beat Hard", "Complete a level on hard", false, 1),
        };

        foreach(Achievement a in newAchievements) {
            if(!achievements.Contains(a)) {
                achievements.Add(a);
            }
        }

        Debug.Log(newAchievements.Count + " New achievements added");
        Save();
    }

    
    private List<Achievement> FirstTimeLoad() {
        List<Achievement> achievements = new List<Achievement>
        {
            new Achievement("First Report", "Make your first report", false, 1),
            new Achievement("Five Reports", "Make five reports", false, 5),
            new Achievement("5 Creatures Reported", "Report 5 Creatures", false, 5),
        };

        return achievements;
    }

    private void UnlockAchievement(string name) {
        foreach(Achievement a in achievements) {
            if(a.Name == name) {
                a.dateEarned = DateTime.Now.ToString();
                a.Unlocked = true;
                a.Progress = a.Goal;
                Debug.Log("Achievement unlocked: " + a.Name);
                Save();
                return;
            }
        }
    }

    public void CheckAllAchievementProgress() {
        PlayerDataManager data = PlayerDataManager.Instance;
        foreach(Achievement a in achievements) {
            if(a.Unlocked) {
                continue;
            }
            switch(a.Name) {
                case "First Report":
                    if(int.Parse(data.GetDataValue("ReportsMade")) >= 1) {
                        UnlockAchievement(a.Name);
                    }
                    else {
                        //update progress
                        a.Progress = int.Parse(data.GetDataValue("ReportsMade"));
                    }
                    break;

                case "Five Reports":
                    if(int.Parse(data.GetDataValue("ReportsMade")) >= 5) {
                        UnlockAchievement(a.Name);
                    }
                    else {
                        //update progress
                        a.Progress = int.Parse(data.GetDataValue("ReportsMade"));
                    }
                    break;

                case "5 Creatures Reported":
                    if(int.Parse(data.GetDataValue("CreaturesReported")) >= 5) {
                        UnlockAchievement(a.Name);
                    }
                    else {
                        //update progress
                        a.Progress = int.Parse(data.GetDataValue("CreaturesReported"));
                    }
                    break;
            }
        }
    }

    void Update() {
        if(PlayerUI.paused) {
            return;
        }
        if(timeSinceLastCheck >= checkFrequency) {
            CheckAllAchievementProgress();
            timeSinceLastCheck = 0;
        }
        else {
            timeSinceLastCheck += 1;
        }
    }
}
