using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsMenu : MonoBehaviour
{
    
    private String currentMenu;
    private List<String> menus = new List<String> { "Gameplay", "Controls", "Graphics", "Audio"};
    public Image underline;
    public Button Back;
    public Button Revert;
    public GameObject defaultMenu;

    public List<GameObject> fullMenus = new List<GameObject>();

    private void SetMenu(String menu) {
        currentMenu = menu;
        //Bold the selected menu and create a line underneath by using the list of levels
        foreach (String l in menus) {
            GameObject menuText = GameObject.Find(l);
            if (l == menu) {
                fullMenus[menus.IndexOf(l)].SetActive(true);
                menuText.GetComponentInChildren<TMP_Text>().fontStyle = FontStyles.Bold;
                //activate underline image
                underline.transform.position = new Vector3(menuText.transform.position.x, menuText.transform.position.y - 50, menuText.transform.position.z);
            } else {
                fullMenus[menus.IndexOf(l)].SetActive(false);
                menuText.GetComponentInChildren<TMP_Text>().fontStyle = FontStyles.Normal;
            }
        }

    }

    private void RevertChanges() {
        fullMenus[0].GetComponent<GameplaySettings>().RevertChanges();
        //fullMenus[1].GetComponent<ControlsSettings>().Start();
        //fullMenus[2].GetComponent<GraphicsSettings>().Start();
        //fullMenus[3].GetComponent<AudioSettings>().Start();
    }

    private void BackEvent() {
        this.gameObject.SetActive(false);
        defaultMenu.SetActive(true);

        fullMenus[0].GetComponent<GameplaySettings>().SaveSettings();
        //fullMenus[1].GetComponent<ControlsSettings>().SaveSettings();
        //fullMenus[2].GetComponent<GraphicsSettings>().SaveSettings();
        //fullMenus[3].GetComponent<AudioSettings>().SaveSettings();

    }

    void Start() {
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
            RevertChanges();
            BackEvent();
        }
    }
}
