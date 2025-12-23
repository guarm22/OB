using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using DG.Tweening;

public class SettingsMenu : MonoBehaviour
{
    
    private String currentMenu;
    private List<String> menus = new List<String> { "Gameplay", "Controls", "Graphics", "Audio"};
    public Image underline;
    public Button Back;
    public Button Revert;
    public GameObject defaultMenu;

    public Image divider;
    public static SettingsMenu Instance;

    public List<GameObject> fullMenus = new List<GameObject>();

    public GameObject popup;
    public Button popupYes;
    public Button popupNo;

    public bool isPopupOpen = false;

    public GameObject bg;

    public void popupYesEvent() {
        closePopup();
        SaveChanges();
    }
    public void popupNoEvent() {
        closePopup();
        RevertChanges();
        if(defaultMenu == null) {
            EscapeMenu.Instance.CloseOptions();
            this.gameObject.SetActive(false);
            return;
        }
        this.gameObject.SetActive(false);
        defaultMenu.SetActive(true);
    }

    private void SetMenu(String menu, bool firstTime = false) {
        currentMenu = menu;
        float moveTime = 0.25f;
        if(firstTime) {moveTime = 0.01f;}
        //Bold the selected menu and create a line underneath by using the list of levels
        foreach (String l in menus) {
            GameObject menuText = GameObject.Find(l+"Btn");
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

    public void RevertChanges() {
        fullMenus[0].GetComponent<GameplaySettings>().RevertChanges();
        fullMenus[1].GetComponent<ControlSettings>().RevertChanges();
        fullMenus[2].GetComponent<GraphicsSettings>().RevertChanges();
        fullMenus[3].GetComponent<AudioSettings>().RevertChanges();
        SetMenu(currentMenu);
        /*if(defaultMenu == null) {
            //EscapeMenu.Instance.CloseOptions();
            //this.gameObject.SetActive(false);
            return;
        }
        this.gameObject.SetActive(false);
        defaultMenu.SetActive(true);*/
    }

    private void SaveChanges() {
        fullMenus[0].GetComponent<GameplaySettings>().SaveSettings();
        fullMenus[1].GetComponent<ControlSettings>().SaveSettings();
        fullMenus[2].GetComponent<GraphicsSettings>().SaveSettings();
        fullMenus[3].GetComponent<AudioSettings>().SaveSettings();

        //update volume for main menu and music
        if(GameObject.Find("MainScreen") != null) {
            GameObject.Find("MainScreen").GetComponent<AudioSource>().volume = PlayerPrefs.GetInt("MusicVolume", 50) / 100f;
        }
        AudioListener.volume = PlayerPrefs.GetInt("MasterVolume", 50) / 100f;

        //update brightness
        GlobalPostProcessingSettings.Instance.SetGammaAlpha(PlayerPrefs.GetInt("Brightness", 50));

        if(defaultMenu == null) {
            EscapeMenu.Instance.CloseOptions();
            this.gameObject.SetActive(false);
            return;
        }
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

    public void closePopup() {
        isPopupOpen = false;
        popup.SetActive(false);
    }

    void Start() {
        foreach(String l in menus) {
            GameObject b = GameObject.Find(l+"Btn");
            AddOnClick(b, l);
        }
        if(defaultMenu == null){
            Open();
        }
        
        EventTrigger trigger = bg.AddComponent<EventTrigger>();
        // Create a new entry for the click event
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerClick;

        // Add a callback to the entry
        entry.callback.AddListener(delegate { closePopup(); });

        // Add the entry to the trigger
        trigger.triggers.Add(entry);

        Instance = this;
        currentMenu = "Gameplay";
        SetMenu(currentMenu, true);
        Back.onClick.AddListener(ShowPopup);
        Revert.onClick.AddListener(RevertChanges);
        popupYes.onClick.AddListener(popupYesEvent);
        popupNo.onClick.AddListener(popupNoEvent);
    }

    public void ShowPopup() {
        isPopupOpen = true;
        popup.SetActive(true);
    }

    // Update is called once per frame
    void Update() {
        //pressing Q or E switches the current menu selection
        if(KeybindMenu.Instance != null && KeybindMenu.Instance.isOpen) {
            return;
        }
        if(isPopupOpen) {
            return;
        }
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
            if(defaultMenu == null) {
                return;
            }
            if(isPopupOpen) {
                closePopup();
                return;
            }
            ShowPopup();
        }
    }
}
