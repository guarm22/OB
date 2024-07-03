using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DifficultySelect : MonoBehaviour
{

    public Button EasyButton;
    public Button MediumButton;
    public Button HardButton;

    private String difficulty;

    public Image underline;

    private void SetDifficulty(String diff) {
        difficulty = diff;
        GameSettings.Instance.setDifficulty(diff);
        //Bold the selected difficulty and create a line underneath
        switch(diff) {
            case "Easy":
                EasyButton.GetComponentInChildren<TMP_Text>().fontStyle = FontStyles.Bold;
                MediumButton.GetComponentInChildren<TMP_Text>().fontStyle = FontStyles.Normal;
                HardButton.GetComponentInChildren<TMP_Text>().fontStyle = FontStyles.Normal;
                underline.transform.position = new Vector3(EasyButton.transform.position.x, EasyButton.transform.position.y - 50, EasyButton.transform.position.z);
                break;
            case "Normal":
                EasyButton.GetComponentInChildren<TMP_Text>().fontStyle = FontStyles.Normal;
                MediumButton.GetComponentInChildren<TMP_Text>().fontStyle = FontStyles.Bold;
                HardButton.GetComponentInChildren<TMP_Text>().fontStyle = FontStyles.Normal;
                underline.transform.position = new Vector3(MediumButton.transform.position.x, MediumButton.transform.position.y - 50, MediumButton.transform.position.z);
                break;
            case "Hard":
                underline.transform.position = new Vector3(HardButton.transform.position.x, HardButton.transform.position.y - 50, HardButton.transform.position.z);
                EasyButton.GetComponentInChildren<TMP_Text>().fontStyle = FontStyles.Normal;
                MediumButton.GetComponentInChildren<TMP_Text>().fontStyle = FontStyles.Normal;
                HardButton.GetComponentInChildren<TMP_Text>().fontStyle = FontStyles.Bold;
                break;
        }
        
    }

    public String GetDifficulty() {
        return difficulty;
    }

    void Start() {
        EasyButton.onClick.AddListener( delegate{SetDifficulty("Easy"); });
        MediumButton.onClick.AddListener( delegate{SetDifficulty("Normal"); });
        HardButton.onClick.AddListener( delegate{SetDifficulty("Hard"); });

        SetDifficulty(PlayerPrefs.GetString("lastChosenDiff", "Normal"));
    }

    void Update() {
        
    }
}
