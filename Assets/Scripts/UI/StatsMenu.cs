using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class StatsMenu : MonoBehaviour
{
    private String currentMenu;
    private List<String> menus = new List<String> { "Stats", "Achievements", "Relics", "Compendium"};
    public Image underline;
    public Button Back;
    public GameObject defaultMenu;

    public GameObject divider;

    public List<GameObject> fullMenus = new List<GameObject>();

    //to use in game rather than in main menu
    public GameObject pauseMenu;

    private void SetMenu(String menu, bool firstTime = false) {
        currentMenu = menu;
        //Bold the selected menu and create a line underneath by using the list of levels
        float moveTime = 0.25f;
        if (firstTime) {moveTime = 0.01f;}
        foreach (String l in menus) {
            GameObject menuText = GameObject.Find(l);
            if (l == menu) {
                fullMenus[menus.IndexOf(l)].SetActive(true);
                menuText.GetComponentInChildren<TMP_Text>().fontStyle = FontStyles.Bold;
                menuText.GetComponentInChildren<TMP_Text>().color = Color.white;
                //activate underline image
                MoveUnderline(menuText, moveTime);
            } else {
                fullMenus[menus.IndexOf(l)].SetActive(false);
                //change text color
                menuText.GetComponentInChildren<TMP_Text>().color = new Color(178/255f, 201/255f, 226/255f, 1);
                menuText.GetComponentInChildren<TMP_Text>().fontStyle = FontStyles.Normal;
            }
        }
    }

    private void MoveUnderline(GameObject parent, float moveTime=0.5f) {
        underline.transform.DOKill();
        float y = Display.main.systemHeight/42f;
        underline.transform.DOMove(new Vector3(parent.transform.position.x, 
        divider.transform.position.y, parent.transform.position.z), moveTime);

        parent.GetComponentInChildren<TMP_Text>().ForceMeshUpdate();
        float newX = parent.GetComponentInChildren<TMP_Text>().GetRenderedValues(true).x;
        underline.rectTransform.DOSizeDelta(new Vector2(newX, underline.rectTransform.sizeDelta.y), moveTime+0.2f);
    }

    private void BackEvent() {
        if(defaultMenu == null) {
            pauseMenu.SetActive(true);
            this.gameObject.SetActive(false);
            return;
        }
        else {
            this.gameObject.SetActive(false);
            defaultMenu.SetActive(true);
        }
    }

    private void AddOnClick(GameObject button, String level) {
        button.GetComponent<Button>().onClick.AddListener(() => SetMenu(level));
    }

    void Start() {
         foreach (String l in menus) {
            GameObject b = GameObject.Find(l);
            AddOnClick(b, l);
        }
        currentMenu = "Stats";
        SetMenu(currentMenu, true);

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
