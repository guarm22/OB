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

    public Image divider;

    private String currentLevel;

    private List<String> levels = new List<String> { "Tutorial", "Cabin", "Graveyard", "Apartment" };
    private List<String> unavailableLevels = new List<String> { "ThePuncture"};

    private void SetLevel(String level) {
        currentLevel = level;

        //Bold the selected level and create a line underneath by using the list of levels
        foreach (String l in levels) {
            GameObject levelText = GameObject.Find(l);
            if (l == level) {
                levelText.GetComponentInChildren<TMP_Text>().fontStyle = FontStyles.Bold;
                //activate underline image
                levelText.GetComponentInChildren<TMP_Text>().color = new Color(255f, 255f, 255f, 1);
                underline.transform.position = new Vector3(levelText.transform.position.x, divider.transform.position.y, levelText.transform.position.z);
            } else {
                //change text color
                levelText.GetComponentInChildren<TMP_Text>().color = new Color(178/255f, 201/255f, 226/255f, 1);
                levelText.GetComponentInChildren<TMP_Text>().fontStyle = FontStyles.Normal;
            }
        }
        //update level image
        currentLevelImage.sprite = levelImages[levels.IndexOf(currentLevel)];

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
