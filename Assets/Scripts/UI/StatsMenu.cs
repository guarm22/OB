using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatsMenu : MonoBehaviour
{
    private String currentMenu;
    private List<String> menus = new List<String> { "Stats", "Achievements", /*"Relics",*/ "Compendium"};
    public Image underline;
    public Button Back;
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

    private void BackEvent() {
        this.gameObject.SetActive(false);
        defaultMenu.SetActive(true);
    }

    void Start() {
        currentMenu = "Stats";
        SetMenu(currentMenu);

        Back.onClick.AddListener(BackEvent);
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
            BackEvent();
        }
        
    }
}
