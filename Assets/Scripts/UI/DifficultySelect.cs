using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DifficultySelect : MonoBehaviour {

    public Button EasyButton;
    public Button MediumButton;
    public Button HardButton;

    private String difficulty;

    public Image underline;

    private void SetDifficulty(String diff, bool firstTime = false) {
        difficulty = diff;
        GameSettings.Instance.setDifficulty(diff);
        //Bold the selected difficulty and create a line underneath
        float moveTime = 0.25f;
        if(firstTime) {moveTime = 0.01f;}
        switch(diff) {
            case "Easy":
                EasyButton.GetComponentInChildren<TMP_Text>().fontStyle = FontStyles.Bold;
                EasyButton.GetComponentInChildren<TMP_Text>().color = Color.white;

                MediumButton.GetComponentInChildren<TMP_Text>().fontStyle = FontStyles.Normal;
                MediumButton.GetComponentInChildren<TMP_Text>().color = new Color(178/255f, 201/255f, 226/255f, 1);

                HardButton.GetComponentInChildren<TMP_Text>().fontStyle = FontStyles.Normal;
                HardButton.GetComponentInChildren<TMP_Text>().color = new Color(178/255f, 201/255f, 226/255f, 1);
                MoveUnderline(EasyButton.gameObject,moveTime);                
                break;
            case "Normal":
                EasyButton.GetComponentInChildren<TMP_Text>().fontStyle = FontStyles.Normal;
                EasyButton.GetComponentInChildren<TMP_Text>().color = new Color(178/255f, 201/255f, 226/255f, 1);;

                MediumButton.GetComponentInChildren<TMP_Text>().fontStyle = FontStyles.Bold;
                MediumButton.GetComponentInChildren<TMP_Text>().color = Color.white;

                HardButton.GetComponentInChildren<TMP_Text>().fontStyle = FontStyles.Normal;
                HardButton.GetComponentInChildren<TMP_Text>().color = new Color(178/255f, 201/255f, 226/255f, 1);;
                MoveUnderline(MediumButton.gameObject,moveTime);
                break;
            case "Hard":
                MoveUnderline(HardButton.gameObject,moveTime);
                EasyButton.GetComponentInChildren<TMP_Text>().fontStyle = FontStyles.Normal;
                EasyButton.GetComponentInChildren<TMP_Text>().color = new Color(178/255f, 201/255f, 226/255f, 1);;

                MediumButton.GetComponentInChildren<TMP_Text>().fontStyle = FontStyles.Normal;
                MediumButton.GetComponentInChildren<TMP_Text>().color = new Color(178/255f, 201/255f, 226/255f, 1);;

                HardButton.GetComponentInChildren<TMP_Text>().fontStyle = FontStyles.Bold;
                HardButton.GetComponentInChildren<TMP_Text>().color = Color.white;
                break;
        }
        
    }

    private void MoveUnderline(GameObject parent, float moveTime=0.5f) {
        underline.transform.DOKill();
        float y = Display.main.systemHeight/42f;
        underline.transform.DOMove(new Vector3(parent.transform.position.x, 
        parent.transform.position.y-y, parent.transform.position.z), moveTime);
    }

    public String GetDifficulty() {
        return difficulty;
    }

    void Start() {
        EasyButton.onClick.AddListener( delegate{SetDifficulty("Easy"); });
        MediumButton.onClick.AddListener( delegate{SetDifficulty("Normal"); });
        HardButton.onClick.AddListener( delegate{SetDifficulty("Hard"); });

        SetDifficulty(PlayerPrefs.GetString("lastChosenDiff", "Normal"), true);
    }
}
