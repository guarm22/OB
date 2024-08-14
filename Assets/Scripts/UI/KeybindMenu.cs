using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KeybindMenu : MonoBehaviour {

    public GameObject settingsMenu;

    public Button saveButton;
    public Button backButton;
    public Button resetButton;
    public List<GameObject> otherMenus;

    public List<GameObject> keybinds;
    public GameObject keybindPrefab;
    public GameObject keybindInitialLoc;

    public Keybind currentlyEditing = null;

    public List<Keybind> keybindsList;

    public bool isOpen = false;
    public static KeybindMenu Instance;

    void Awake() {
        Instance = this;
        saveButton.onClick.AddListener(() => {
            SaveKeybinds();
            otherMenus.ForEach(m => m.SetActive(true));
            settingsMenu.SetActive(true);
            KeybindMenu.Instance.isOpen = false;            
            gameObject.SetActive(false);
        });

        backButton.onClick.AddListener(() => {
            DiscardChanges();
            otherMenus.ForEach(m => m.SetActive(true));
            settingsMenu.SetActive(true);
            KeybindMenu.Instance.isOpen = false;
            gameObject.SetActive(false);
        });
        
        foreach(Keybind k in KeybindManager.instance.keybinds) {
            keybindsList.Add(new Keybind(k.action, k.key));
        }

        resetButton.onClick.AddListener(() => {
            ResetToDefault();
        });

        InitKeybinds();
    }

    public void InitKeybinds() {
        // Load keybinds
        float x = 0;
        float y = 0;
        int items = 0;
        int itemsPerColumn = 9;
        Vector3 initialLoc = keybindInitialLoc.transform.position;

        foreach(Keybind k in keybindsList) {
            if(items%itemsPerColumn == 0 && items != 0) {
                x += Display.main.systemWidth / 2.8f;
                y = 0;
            }

            if(k.action == "FileSystemVersion") {
                continue;
            }
            if(k.action == "Forward" || k.action == "Back" || k.action == "Left" || k.action == "Right") {
                continue;
            }

            GameObject newKeybind = Instantiate(keybindPrefab, new Vector3(initialLoc.x + x, initialLoc.y + y, initialLoc.z), Quaternion.identity, keybindInitialLoc.transform);
            TMP_Text action = newKeybind.transform.GetChild(0).GetComponent<TMP_Text>();
            TMP_Text key = newKeybind.transform.GetChild(1).GetComponentInChildren<TMP_Text>();
            GameObject bg = newKeybind.transform.GetChild(2).gameObject;
            Button b = newKeybind.GetComponentInChildren<Button>();

            action.text = k.action;
            key.text = k.key.ToString();
            if(key.text == "Mouse1") {
                key.text = "R Click";
            }
            if(key.text == "Mouse0") {
                key.text = "L Click";
            }
            if(key.text == "None") {
                key.text = "Unbound";
            }
            if(key.text == "Mouse2") {
                key.text = "M Click";
            }
            b.onClick.AddListener(delegate {StartEditKeybind(action, key, bg);});
            keybinds.Add(newKeybind);
            y -= Display.main.systemHeight / 15f;
            items+=1;
        }
    }

    private void ResetUI() {
        foreach(GameObject k in keybinds) {
            Destroy(k);
        }
        keybinds.Clear();
        InitKeybinds();
    }

    private void StartEditKeybind(TMP_Text action, TMP_Text key, GameObject bg) {
        // Change keybind
        key.text = "_";
        currentlyEditing = KeybindManager.instance.keybinds.Find(k => k.action == action.text);
        bg.SetActive(true);
        StartCoroutine(WaitForKeyPress());
    }

    private IEnumerator WaitForKeyPress() {
        yield return new WaitForSeconds(Time.deltaTime+0.01f);
        bool pressed = false;
        while(!pressed) {
            foreach(KeyCode kcode in System.Enum.GetValues(typeof(KeyCode))) {
                if(Input.GetKeyDown(kcode)) {
                    ChangeKeybind(kcode);
                    pressed = true;
                }
            }
            yield return null;
        }
    }

    private void ChangeKeybind(KeyCode pressedKey) {
        String action = currentlyEditing.action;
        keybindsList.Find(k => k.action == action).key = pressedKey;
        ResetUI();
        currentlyEditing = null;
    }

    private void ResetToDefault() {
        keybindsList = KeybindManager.instance.GetDefaultKeybinds();
        SaveKeybinds();
        ResetUI();
    }

    private void DiscardChanges() {
        keybindsList = KeybindManager.instance.keybinds;
        ResetUI();
    }


    private void SaveKeybinds() {
        KeybindManager.instance.keybinds = keybindsList;
        KeybindManager.instance.SaveKeybinds();
    }

    void Update() {
        if(Input.GetKeyDown(KeyCode.Escape) && currentlyEditing != null) {
            DiscardChanges();
            otherMenus.ForEach(m => m.SetActive(true));
            settingsMenu.SetActive(true);
            KeybindMenu.Instance.isOpen = false;
            gameObject.SetActive(false);
        }
    }
}
