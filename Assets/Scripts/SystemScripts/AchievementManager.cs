using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementManager : MonoBehaviour
{
    public int checkFrequency = 20;
    private float timeSinceLastCheck = 0;

    public List<Achievement> achievements = new List<Achievement>();
    private JsonWrapperUtil<Achievement> wrapper;

    public static AchievementManager Instance;

    private int unlocked = 0;
    
    void Start() {
        //load list from file
        Instance = this;
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

    private List<Achievement> FirstTimeLoad() {
        List<Achievement> achievements = new List<Achievement>
        {
            new Achievement("Divergence Hunter", "Make your first report.", false, 1, "ACH_DIVERGENCEHUNTER"),
            new Achievement("Divergence Destroyer", "Make fifty reports.", false, 50, "ACH_DIVERGENCEDESTROYER"),
            new Achievement("Be Gone, Creatures", "Report twenty creatures.", false, 20, "ACH_BEGONECREATURES"),
            new Achievement("First Steps", "Complete the tutorial.", false, 1, "ACH_FIRSTSTEPS"),
            new Achievement("A Night in the Woods", "Complete the cabin level.", false, 1, "ACH_ANIGHTINTHEWOODS"),
            new Achievement("Haunting Disappearences", "Complete the Graveyard level.", false, 1, "ACH_HAUNTINGDISAPPEARENCES"),
            new Achievement("Beat Normal", "Complete a level on normal.", false, 1, "ACH_BEATNORMAL"),
            new Achievement("Beat Hard", "Complete a level on hard.", false, 1, "ACH_BEATHARD"),
            new Achievement("Eyes Peeled", "Beat a level on normal or harder without letting a divergence be active for more than 40 seconds.", false, 1, "ACH_EYESPEELED"),
        };

        return achievements;
    }
    private void AddNewAchievements() {
        List<Achievement> newAchievements = new List<Achievement> {
            
        };

        if(newAchievements.Count == 0) {
            Debug.Log("No new achievements added");
            return;
        }

        foreach(Achievement a in newAchievements) {
            if(!achievements.Contains(a)) {
                achievements.Add(a);
            }
        }
        Save();
    }

    public void CheckLevelFinishAchievements(String level, String diff, String reason) {
        CheckAllAchievementProgress();
        if(reason != "won") {
            return;
        }

        foreach(Achievement a in achievements) {
            if(a.Unlocked) {
                continue;
            }

            if(a.Name == "A Night in the Woods") {
                if(a.Unlocked) {
                    
                }
                else if(level=="Cabin" && a.Progress == 0) {
                    UnlockAchievement(a.Name);
                }
            }

            if(a.Name == "Haunting Disappearences") {
                if(a.Unlocked) {
                    
                }
                else if(level=="Graveyard" && a.Progress == 0) {
                    UnlockAchievement(a.Name);
                }
            }

            if(a.Name == "First Steps") {
                if(a.Unlocked) {
                    
                }
                else if(level=="Tutorial" && a.Progress == 0) {
                    UnlockAchievement(a.Name);
                }
            }

            if(a.Name == "Beat " + diff) {
                if(a.Unlocked) {
                    
                }
                else if(a.Name == "Beat " + diff && a.Progress == 0) {
                    UnlockAchievement(a.Name);
                }
            }

            if(a.Name == "Eyes Peeled") {
                if(a.Unlocked || GameSystem.Instance.Difficulty == "Easy" || GameSystem.Instance.Difficulty == "Custom") {
                    continue;
                }
                bool failed = false;
                //check each report to make sure an item wasn't active for more than 40s
                foreach(Report r in GameSystem.Instance.reports) {
                    foreach(float f in r.itemActiveTime) {
                        if(f > 40) {
                            failed = true;
                        }
                    }
                }
                //check each divergence to make sure it hasn't been active for more than 40s
                foreach(DynamicObject d in DivergenceControl.Instance.DivergenceList) {
                    if(Time.time - d.divTime > 40) {
                        failed = true;
                    }
                }
                if(!failed) {
                    UnlockAchievement(a.Name);
                }
            }

        }
        Save();
    }

    public void CheckAllAchievementProgress() {
        PlayerDataManager data = PlayerDataManager.Instance;
        foreach(Achievement a in achievements) {
            if(a.Unlocked) {
                continue;
            }
            switch(a.Name) {
                case "Divergence Hunter":
                    if(int.Parse(data.GetDataValue("ReportsMade")) >= 1) {
                        UnlockAchievement(a.Name);
                    }
                    else {
                        //update progress
                        a.Progress = int.Parse(data.GetDataValue("ReportsMade"));
                    }
                    break;

                case "Divergence Destroyer":
                    if(int.Parse(data.GetDataValue("ReportsMade")) >= 50) {
                        UnlockAchievement(a.Name);
                    }
                    else {
                        //update progress
                        a.Progress = int.Parse(data.GetDataValue("ReportsMade"));
                    }
                    break;

                case "Be Gone, Creatures":
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
        if(unlocked > 0) {
            Save();
        }
        UpdateSteamStats();
    }

    private void UpdateSteamStats() {
        List<String> statNames = new List<String> {
            "S_REPORTS",
            "S_CREATURESREPORTED"
        };

        foreach(String s in statNames) {
            SteamHelper.SetStat(s, int.Parse(PlayerDataManager.Instance.GetDataValue(PlayerDataManager.Instance.SteamToPunctureValue(s))));
        }
    }

    private void UnlockAchievement(string name) {
        foreach(Achievement a in achievements) {
            if(a.Name == name) {
                this.unlocked = this.unlocked + 1;
                a.dateEarned = DateTime.Now.ToString();
                a.Unlocked = true;
                a.Progress = a.Goal;
                SteamHelper.UnlockAchievement(a.steamID);
                Debug.Log("Achievement unlocked: " + a.Name);
                return;
            }
        }
    }

    void Save() {
        PFileUtil.Save("achievementList.json", new JsonWrapperUtil<Achievement>(achievements));
    }


    void Update() {
        if(PlayerUI.paused || GameSystem.Instance.GameOver) {
            return;
        }
        if(timeSinceLastCheck >= checkFrequency) {
            CheckAllAchievementProgress();
            timeSinceLastCheck = 0;
        }
        else {
            timeSinceLastCheck += Time.deltaTime;
        }
    }
}
