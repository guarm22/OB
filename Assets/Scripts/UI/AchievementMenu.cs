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

    public Button nextButton;
    public Button prevButton;
    public TMP_Text pageText;
    private int pnum = 1;

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
            return;
        }
        ShowPage(pnum);
    }

    private void ShowPage(int num) {
        foreach(Transform child in achievementParent) {
            Destroy(child.gameObject);
        }

        int y = 0;
        for(int i = 0; i < 5; i++) {
            if(achievements.Count > i + (num - 1) * 5) {
                GameObject achievement = Instantiate(achievementPrefab, achievementParent);
                achievement.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, y);
                y -= 150;
                achievement.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = achievements[i + (num - 1) * 5].Name;
                achievement.transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = achievements[i + (num - 1) * 5].Description;
                achievement.transform.GetChild(2).GetComponent<TMPro.TextMeshProUGUI>().text = achievements[i + (num - 1) * 5].Progress + "/" + achievements[i + (num - 1) * 5].Goal;
                achievement.transform.GetChild(3).GetComponent<TMPro.TextMeshProUGUI>().text = "Date Earned: " + achievements[i + (num - 1) * 5].dateEarned;
            }
        }
        pageText.text = "Page " + num + " of " + Mathf.Ceil(achievements.Count / 5f);
    }

    void Start() {
        LoadAchievements();

        CreateAchievements();

        nextButton.onClick.AddListener(() => {
            if(achievements.Count > pnum * 5) {
                pnum++;
                ShowPage(pnum);
            }
        });

        prevButton.onClick.AddListener(() => {
            if(pnum > 1) {
                pnum--;
                ShowPage(pnum);
            }
        });
    }

    void Update() {
        
    }
}
