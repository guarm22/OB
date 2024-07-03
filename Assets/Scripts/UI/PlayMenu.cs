using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayMenu : MonoBehaviour
{
    
    public GameObject defaultMenu;

    public GameObject levelSelect;
    public GameObject difficultySelect;

    public Button playButton;
    public Button backButton;

    private void PlayButtonEvent() {
        //Debug.Log("Play button pressed");
        String level = levelSelect.GetComponent<LevelSelect>().GetLevel();
        //Debug.Log("Difficulty: " + difficultySelect.GetComponent<DifficultySelect>().GetDifficulty());

        //Save difficulty settings for next load in playerprefs
        PlayerPrefs.SetString("lastChosenDiff", difficultySelect.GetComponent<DifficultySelect>().GetDifficulty());

        //sets difficulty settings before loading level
        LoadScreen.Instance.Loading = true;
        GameSettings.Instance.SetValues();
        try {
            SceneManager.LoadScene(level);
        }
        catch(Exception e) {
            Debug.Log("Level does not exist: " +e);
        }
    }
    
    private void BackButtonEvent() {
        this.gameObject.SetActive(false);
        defaultMenu.SetActive(true);
    }

    void Start() {
        playButton.onClick.AddListener(PlayButtonEvent);
        backButton.onClick.AddListener(BackButtonEvent);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape)) {
            BackButtonEvent();
        }
    }
}
