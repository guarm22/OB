using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementMenu : MonoBehaviour
{
    public GameObject achievementPrefab;
    public Transform achievementParent;
    public List<Achievement> achievements = new List<Achievement>();

    public GameObject noAchievementsText;

    private void LoadAchievements() {
        achievements = PFileUtil.Load<JsonWrapperUtil<Achievement>>("achievementList.json").list;
        if(achievements == null) {
            return;
        }
        Debug.Log("Loaded " + achievements.Count + " achievements");
    } 

    private void CreateAchievements() {
        if(achievements.Count == 0) {
            noAchievementsText.SetActive(true);
            return;
        }
        int y = 0;

        foreach(Achievement a in achievements) {
            GameObject achievement = Instantiate(achievementPrefab, achievementParent);
            achievement.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, y);
            y -= 100;
            achievement.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = a.Name;
            achievement.transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = a.Description;
            achievement.transform.GetChild(2).GetComponent<TMPro.TextMeshProUGUI>().text = a.Progress + "/" + a.Goal;
            achievement.transform.GetChild(3).GetComponent<TMPro.TextMeshProUGUI>().text = "Date Earned: " + a.dateEarned;
        }
    }

    void Start() {
        LoadAchievements();

        CreateAchievements();
    }

    void Update() {
        
    }
}
