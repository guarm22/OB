using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    ///Default UI that appears on the bottom right of the players screen
    public GameObject defaultUI;
    //UI that appears on the bottom right when the player presses tab
    public GameObject selectionUI;
    public GameObject roomSelectionUI;
    public GameObject typeSelectionUI;
    public GameObject togglePrefab;
    public GameObject roomText;
    public GameObject escapeMenuUI;
    public GameObject EndGameUI;
    public GameObject debugUI;
    public GameObject defaultBottomRight;
    public TMP_Text tabText;
    public string targetTag = "Room";
    public string tutTag = "Tutorial";
    public bool inMenu = false;
    public static bool paused = false;
    public static PlayerUI Instance;
    public GameObject prompt;
    public bool havePausedAtleastOnce = false;
    public String currentRoom;

    public bool reportScramble = false;

    public bool isGlitching = false;

    public AudioClip glitchSound;
    private AudioSource audioSource;
    private bool isReportTextGlitching = false;

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        audioSource = this.gameObject.AddComponent<AudioSource>();
        PopulateSelectorUI();
    }

    // Update is called once per frame
    void Update() {
        if(Popup.Instance != null) {
            if(Popup.Instance.isPopupOpen ) {
                return;
            }
        }
        //now waiting for jumpscare to finish, if any
        if(GameSystem.Instance.GameOver) {
            EndingGame();
            return;
        }
        EscapeMenu();
        if(paused) {
            return;
        }
        SelectionMenu();
    }

    public void ChangePrompt(string text, bool activate) {
        prompt.SetActive(activate);
        prompt.GetComponent<TMP_Text>().text = text;
    }

    public void ScrambleReportUI(bool activate) {
        reportScramble = activate;
    }

    void PopulateSelectorUI() {
        GameObject[] rooms = GameObject.FindGameObjectsWithTag("Room");
        //Float that determines height between each selector
        float iter = 0f;
        //for getting each element in the list
        int i = 0;

        //Creates each of the room selectors in the selection UI
        foreach(GameObject room in rooms) {
            GameObject ui = Instantiate(togglePrefab, transform);
            ui.transform.SetParent(roomSelectionUI.transform);
            ui.transform.localPosition = new Vector3(0f, iter, 0f);
            ui.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = rooms[i].name;
            //ui.transform.GetChild(1).gameObject.GetComponent<Text>().fontSize = 40;

            ui.GetComponent<Toggle>().onValueChanged.AddListener(
                delegate { RoomSelection.Instance.Select(ui); });

            ui.transform.localScale = new Vector3(3f,3f,3f);
            ui.name = rooms[i++].name;
            iter+=100f;
        }

        List<string> types = DynamicObject.GetAllAnomalyTypes();
        iter = 0f;
        //Creates each of the type selectors
        foreach(string type in types) {
            GameObject ui = Instantiate(togglePrefab, transform);
            ui.transform.SetParent(typeSelectionUI.transform);
            ui.transform.localPosition = new Vector3(0f, iter, 0f);
            ui.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = type;
            ui.transform.localScale = new Vector3(3f,3f,3f);
            ui.name = type;
            iter+=100f;
            ui.GetComponent<Toggle>().onValueChanged.AddListener(
            delegate { TypeSelection.Instance.Select(ui); });       
        }
    }

    private void turnOffSelection() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        inMenu = false;
        if(selectionUI.GetComponent<ReportUI>() != null) {
            selectionUI.GetComponent<ReportUI>().TurnOff();
        }
        else {
            selectionUI.SetActive(false);
        }
        defaultBottomRight.SetActive(true);
        SC_FPSController.Instance.canMove = true;
    }
    private void turnOnSelection() {
        selectionUI.SetActive(true);
        if(selectionUI.GetComponent<ReportUI>() != null) {
            selectionUI.GetComponent<ReportUI>().TurnOn();
        }

        if(CreatureControl.Instance.ActiveCreatures.FindAll(x => x.name.Contains("Hider")).Count == 0) {
            ScrambleReportUI(false);
        }

        //turn off any currently playing warnings
        if(Warning.Instance != null) {
            Warning.Instance.TurnOffAlert();
        }
        defaultBottomRight.SetActive(false);
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        inMenu = true;
        SC_FPSController.Instance.canMove = false;
    }

    private IEnumerator GlitchReportText() {
        //for 0.5 seconds, change the tab text to random characters
        float elapsedTime = 0f;
        String originalText = tabText.text;
        while(elapsedTime < 0.8f) {
            elapsedTime += Time.deltaTime;
            char[] chars = tabText.text.ToCharArray();
            for(int i = 0; i < chars.Length; i++) {
                if(UnityEngine.Random.Range(0, 5) == 0) {
                    chars[i] = (char)UnityEngine.Random.Range(65, 91);
                }
            }
            tabText.text = new string(chars);
            yield return null;
        }
        tabText.text = originalText;
        isReportTextGlitching = false;
    }

    void SelectionMenu() {
        if(isGlitching) {
            if(inMenu) {
                turnOffSelection();
                PostProcessingControl.Instance.ActivateDepthOfField(false);
            }
            if(Input.GetKeyDown(KeybindManager.instance.GetKeybind("Report Menu"))) {
                if(!isReportTextGlitching) {
                    audioSource.PlayOneShot(glitchSound);
                    isReportTextGlitching = true;
                    StartCoroutine(GlitchReportText());
                }
            }
            return;
        }
        //if menu is open, check if tab is pressed to close, otherwise stop movement

        if(inMenu) {
            if(Input.GetKeyDown(KeybindManager.instance.GetKeybind("Report Menu")) || DivergenceControl.Instance.PendingReport) {
                turnOffSelection();
                PostProcessingControl.Instance.ActivateDepthOfField(false);
            }
        }
        else if(Input.GetKeyDown(KeybindManager.instance.GetKeybind("Report Menu")) && !DivergenceControl.Instance.PendingReport) {
            PostProcessingControl.Instance.ActivateDepthOfField(true, 50, 1);
            turnOnSelection();
        }
    }

    public String GetCurrentRoom() {
        return currentRoom;
    }

    private void openEscape() {
        turnOffSelection();
        havePausedAtleastOnce = true;
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        escapeMenuUI.SetActive(true);
        paused = true;
        defaultUI.SetActive(false);
    }

    public void closeEscape() {
        paused = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        escapeMenuUI.SetActive(false);
        defaultUI.SetActive(true);

    }

    public void PauseControl(String src="") {
        paused = !paused;
        PostProcessingControl.Instance.ActivateDepthOfField(paused);
        SoundControl.Instance.PauseSound(paused);

        if(src=="escape") {
            if(paused) {
                openEscape();
            }
            else {
                closeEscape();
            }   
        }

        if(src=="popup") {
            if(paused) {
                turnOffSelection();
            }
        }
    }

    private void EscapeMenu() {
        //CHANGE TO ESCAPE
        if(Input.GetKeyDown(KeybindManager.instance.GetKeybind("Pause"))) { 
            PauseControl("escape");
        }
        if(Input.GetKeyDown(KeyCode.P)) {
            //disable all UI
            if(defaultUI.activeSelf) {
                defaultUI.SetActive(false);
            }
            else {
                defaultUI.SetActive(true);
            }
        }
        if(Input.GetKeyDown(KeyCode.O)) {
            debugUI.SetActive(!debugUI.activeSelf);
        }
    }

    private void EndingGame() {
        if(GameSystem.Instance.endReason == "quit") {
            return;
        }
        EndGameUI.SetActive(true);
    }
    //Currently used for figuring out which room the player is in and displaying it on the top right
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals(targetTag) || other.tag.Equals("FakeRoom"))
        {
            // Player is within a GameObject with the specified tag
            //Debug.Log("Player is within a GameObject with the tag: " + targetTag + " with object name: " + other.gameObject.name);
            roomText.GetComponent<TMP_Text>().text = other.gameObject.name;
            currentRoom = other.gameObject.name;
        }

        if (other.tag.Equals(tutTag)) {
            Tutorial.Instance.ActivateTrigger(other.gameObject);
        }
    }
}
