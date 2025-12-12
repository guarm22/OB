using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using NUnit.Framework.Constraints;

public class LevelSelect : MonoBehaviour {

    public Button Cabin;
    public Button Graveyard;
    public Button Apartment;
    public Button ThePuncture;
    public Button Tutorial;
    public List<Sprite> levelImages;
    public Image currentLevelImage;
    public Image underline;

    public GameObject levelImageLeft;
    public GameObject levelImageRight;

    public Image divider;

    public Image background;

    private String currentLevel;

    private bool inAnim = false;

    private List<String> levels = new List<String> { "Tutorial", "Cabin", "Graveyard", "Apartment", "The_Puncture" };
    private List<String> unavailableLevels = new List<String> { "ThePuncture"};

    private void SetLevel(String level, bool firstTime = false) {
        if(inAnim) {
            return;
        }
        int currentLevelNum = levels.IndexOf(currentLevel);
        currentLevel = level;
        int newLevelNum = levels.IndexOf(level);

        float moveTime = 0.5f;
        if(firstTime) {moveTime = 0.01f;}

        //Bold the selected level and create a line underneath by using the list of levels
        foreach (String l in levels) {
            GameObject levelText = GameObject.Find(l);
            if (l == level) {
                levelText.GetComponentInChildren<TMP_Text>().fontStyle = FontStyles.Bold;
                //activate underline image
                levelText.GetComponentInChildren<TMP_Text>().color = new Color(255f, 255f, 255f, 1);
                MoveUnderline(levelText, moveTime);
            } else {
                //change text color
                levelText.GetComponentInChildren<TMP_Text>().color = new Color(178/255f, 201/255f, 226/255f, 1);
                levelText.GetComponentInChildren<TMP_Text>().fontStyle = FontStyles.Normal;
            }
        }
        //update level image on initial open
        if(currentLevelImage.sprite == null) {
            currentLevelImage.sprite = levelImages[levels.IndexOf(currentLevel)];
            return;
        }
        bool left = newLevelNum > currentLevelNum;
        StartCoroutine(LevelAnimation(left));
    }

    private void MoveUnderline(GameObject parent, float moveTime=0.5f) {
        underline.transform.DOKill();
        underline.transform.DOMove(new Vector3(parent.transform.position.x, 
        divider.transform.position.y, parent.transform.position.z), moveTime);
    }

    private IEnumerator LevelAnimation(bool left) {
        inAnim = true;
        float moveDuration = 0.75f;

        currentLevelImage.transform.DOComplete();
        Vector3 originalLocation = background.transform.position;

        Vector3 movePos = left ? levelImageRight.transform.position : levelImageLeft.transform.position;

        background.transform.DOMove(movePos, moveDuration);
        yield return new WaitForSeconds(moveDuration);
        currentLevelImage.sprite = levelImages[levels.IndexOf(currentLevel)];
        yield return new WaitForSeconds(0.1f);
        background.transform.DOMove(originalLocation, moveDuration);
        yield return new WaitForSeconds(moveDuration);
        
        inAnim = false;
        yield break;
    }

    public String GetLevel() {
        return currentLevel;
    }

    private void AddOnClick(GameObject button, String level) {
        button.GetComponent<Button>().onClick.AddListener(() => SetLevel(level));
    }

    void Start() {
        foreach (String l in levels) {
            GameObject b = GameObject.Find(l);
            AddOnClick(b, l);
        }
        SetLevel("Cabin", true);
    }

    void Update() {
        //pressing Q or E switches the current level selection
        if (Input.GetKeyDown(KeyCode.Q)) {
            int index = levels.IndexOf(currentLevel);
            if (index == 0) {
                SetLevel(levels[levels.Count - 1]);
            } else {
                SetLevel(levels[index - 1]);
            }

        } else if (Input.GetKeyDown(KeyCode.E)) {
            int index = levels.IndexOf(currentLevel);
            if (index == levels.Count - 1) {
                SetLevel(levels[0]);
            } else {
                SetLevel(levels[index + 1]);
            }
        }


    }
}
