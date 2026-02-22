using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EscapeMenu : MonoBehaviour {
    public GameObject escapeMenuUI;
    public GameObject defaultUI;
    public GameObject extrasMenu;
    public GameObject menuObjects;
    public GameObject settingsMenu;
    public Button quitButton;
    public Button returnButton;
    public Button extrasButton;
    public Button settingsButton;
    public static EscapeMenu Instance;
    public bool inExtrasMenu = false;
    public bool inOptionsMenu = false;

    public void ReturnToGame() {
        PlayerUI.Instance.PauseControl("resume");
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        defaultUI.SetActive(true);
        escapeMenuUI.SetActive(false);
    }
    private void ShowOptions() {
        inOptionsMenu = true;
        settingsMenu.SetActive(true);
        menuObjects.SetActive(false);
    }
    public void CloseOptions() {
        inOptionsMenu = false;
        SettingsMenu.Instance.RevertChanges();
        settingsMenu.SetActive(false);
        menuObjects.SetActive(true);
    }

    private void ShowExtras() {
        inExtrasMenu = true;
        extrasMenu.SetActive(true);
        menuObjects.SetActive(false);
    }

    public void CloseExtras() {
        inExtrasMenu = false;
        extrasMenu.SetActive(false);
        menuObjects.SetActive(true);
    }

    public void CloseEscapeMenu() {
        extrasMenu.SetActive(false);
        settingsMenu.SetActive(false);
        menuObjects.SetActive(true);
    }

    public void QuitGame() {
        String Difficulty = PlayerPrefs.GetString("Difficulty", "Normal");
        GameSystem.Instance.EndGame("quit");
        PlayerDataManager.Instance.SavePlayerData();
        SceneManager.LoadScene("MainMenuScene");
    }

    // Start is called before the first frame update
    void Start()
    {
        if(defaultUI == null) {
            defaultUI = GameObject.Find("DefaultUI");
        }
        Instance = this;
        quitButton.onClick.AddListener(QuitGame);
        returnButton.onClick.AddListener(ReturnToGame);
        extrasButton.onClick.AddListener(ShowExtras);
        settingsButton.onClick.AddListener(ShowOptions);
    }

    // Update is called once per frame
    void Update() {
        if(Input.GetKeyDown(KeyCode.R)) {
            CloseEscapeMenu();
            PlayerDataManager.Instance.SavePlayerData();
            PlayerUI.paused = false;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
