using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelect : MonoBehaviour {

    public Button Cabin;
    public Button Graveyard;
    public Button Apartment;
    public Button ThePuncture;
    public Button Tutorial;
    public List<Sprite> levelImages;
    public Image currentLevelImage;
    public Image underline;

    private String currentLevel;

    private List<String> levels = new List<String> { "Tutorial", "Cabin", "Graveyard", };
    private List<String> unavailableLevels = new List<String> { "Apartment", "ThePuncture"};

    private void SetLevel(String level) {
        currentLevel = level;

        //Bold the selected level and create a line underneath by using the list of levels
        foreach (String l in levels) {
            GameObject levelText = GameObject.Find(l);
            if (l == level) {
                levelText.GetComponentInChildren<TMP_Text>().fontStyle = FontStyles.Bold;
                //activate underline image
                underline.transform.position = new Vector3(levelText.transform.position.x, levelText.transform.position.y - 50, levelText.transform.position.z);
            } else {
                levelText.GetComponentInChildren<TMP_Text>().fontStyle = FontStyles.Normal;
            }
        }
        //update level image
        currentLevelImage.sprite = levelImages[levels.IndexOf(currentLevel)];

    }

    public String GetLevel() {
        return currentLevel;
    }

    void Start() {
        SetLevel("Cabin");
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
