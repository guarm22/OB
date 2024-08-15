using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class SettingsMenu : MonoBehaviour
{
    
    private String currentMenu;
    private List<String> menus = new List<String> { "Gameplay", "Controls", "Graphics", "Audio"};
    public Image underline;
    public Button Back;
    public Button Revert;
    public GameObject defaultMenu;

    public Image divider;

    public List<GameObject> fullMenus = new List<GameObject>();

    private void SetMenu(String menu) {
        currentMenu = menu;
        //Bold the selected menu and create a line underneath by using the list of levels
        foreach (String l in menus) {
            GameObject menuText = GameObject.Find(l);
            if (l == menu) {
                fullMenus[menus.IndexOf(l)].SetActive(true);
                menuText.GetComponentInChildren<TMP_Text>().fontStyle = FontStyles.Bold;
                menuText.GetComponentInChildren<TMP_Text>().color = Color.white;
                //activate underline image
                underline.transform.position = new Vector3(menuText.transform.position.x, divider.transform.position.y, menuText.transform.position.z);
            } else {
                fullMenus[menus.IndexOf(l)].SetActive(false);
                //change text color
                menuText.GetComponentInChildren<TMP_Text>().color = new Color(178/255f, 201/255f, 226/255f, 1);
                menuText.GetComponentInChildren<TMP_Text>().fontStyle = FontStyles.Normal;
            }
        }

    }

    private void RevertChanges() {
        fullMenus[0].GetComponent<GameplaySettings>().RevertChanges();
        fullMenus[1].GetComponent<ControlSettings>().RevertChanges();
        fullMenus[2].GetComponent<GraphicsSettings>().RevertChanges();
        fullMenus[3].GetComponent<AudioSettings>().RevertChanges();

        this.gameObject.SetActive(false);
        defaultMenu.SetActive(true);
    }

    private void BackEvent() {
        fullMenus[0].GetComponent<GameplaySettings>().SaveSettings();
        fullMenus[1].GetComponent<ControlSettings>().SaveSettings();
        fullMenus[2].GetComponent<GraphicsSettings>().SaveSettings();
        fullMenus[3].GetComponent<AudioSettings>().SaveSettings();

        //update volume for main menu and music
        GameObject.Find("MainScreen").GetComponent<AudioSource>().volume = PlayerPrefs.GetInt("MusicVolume") / 100f;
        AudioListener.volume = PlayerPrefs.GetInt("MasterVolume") / 100f;

        //update brightness
        GlobalPostProcessingSettings.Instance.SetGammaAlpha(PlayerPrefs.GetInt("Brightness"));

        this.gameObject.SetActive(false);
        defaultMenu.SetActive(true);
    }

    public void Open() {
        //initialize the sub menus
        foreach(GameObject menu in fullMenus) {
            menu.SetActive(true);
        }
        fullMenus[0].GetComponent<GameplaySettings>().SetValues();
        fullMenus[1].GetComponent<ControlSettings>().SetValues();
        fullMenus[2].GetComponent<GraphicsSettings>().SetValues();
        fullMenus[3].GetComponent<AudioSettings>().SetValues();

        SetMenu("Gameplay");

        foreach(GameObject menu in fullMenus) {
            if(menu.name != "GameplayMenu") {
                menu.SetActive(false);
            }
        }
    }

    private void AddOnClick(GameObject button, String level) {
        button.GetComponent<Button>().onClick.AddListener(() => SetMenu(level));
    }

    void Start() {
        foreach(String l in menus) {
            GameObject b = GameObject.Find(l);
            AddOnClick(b, l);
        }

        currentMenu = "Gameplay";
        SetMenu(currentMenu);
        Back.onClick.AddListener(BackEvent);
        Revert.onClick.AddListener(RevertChanges);
    }

    // Update is called once per frame
    void Update() {
        //pressing Q or E switches the current menu selection
        if (Input.GetKeyDown(KeyCode.Q)) {
            int index = menus.IndexOf(currentMenu);
            if (index == 0) {
                SetMenu(menus[menus.Count - 1]);
            } else {
                SetMenu(menus[index - 1]);
            }

        } else if (Input.GetKeyDown(KeyCode.E)) {
            int index = menus.IndexOf(currentMenu);
            if (index == menus.Count - 1) {
                SetMenu(menus[0]);
            } else {
                SetMenu(menus[index + 1]);
            }
        }
        if(Input.GetKeyDown(KeyCode.Escape)) {
            if(KeybindMenu.Instance != null && KeybindMenu.Instance.isOpen) {
                return;
            }
            RevertChanges();
        }
    }
}
