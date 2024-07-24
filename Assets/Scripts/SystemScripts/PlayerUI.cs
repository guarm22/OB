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
    public string targetTag = "Room";
    public string tutTag = "Tutorial";
    public bool inMenu = false;
    public static bool paused = false;
    public static PlayerUI Instance;

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        PopulateSelectorUI();
    }

    // Update is called once per frame
    void Update() {
        if(Popup.Instance != null) {
            if(Popup.Instance.isPopupOpen ) {
                return;
            }
        }
        if(CreatureControl.Instance.IsJumpscareFinished) {
            EndingGame();
            return;
        }
        //now waiting for jumpscare to finish, if any
        if(GameSystem.Instance.GameOver) {
            return;
        }
        EscapeMenu();
        if(paused) {
            return;
        }
        SelectionMenu();
    }

    public string GetPlayerRoom() {
        return roomText.GetComponent<TMP_Text>().text;
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
        selectionUI.SetActive(false);
        defaultBottomRight.SetActive(true);
        SC_FPSController.Instance.canMove = true;
    }
    private void turnOnSelection() {
        selectionUI.SetActive(true);
        defaultBottomRight.SetActive(false);
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        inMenu = true;
        SC_FPSController.Instance.canMove = false;
    }

    void SelectionMenu() {
        //if menu is open, check if tab is pressed to close, otherwise stop movement
        if(inMenu) {
            if(Input.GetKeyDown(KeyCode.Tab) || DivergenceControl.Instance.PendingReport) {
                turnOffSelection();
                PostProcessingControl.Instance.ActivateDepthOfField(false);
            }
        }
        else if(Input.GetKeyDown(KeyCode.Tab) && !DivergenceControl.Instance.PendingReport) {
            PostProcessingControl.Instance.ActivateDepthOfField(true, 50, 1);
            turnOnSelection();
        }
    }

    private void openEscape() {
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

        if(src=="escape") {
            if(paused) {
                openEscape();
            }
            else {
                closeEscape();
            }   
        }
    }

    private void EscapeMenu() {
        //CHANGE TO ESCAPE
        if(Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeybindManager.instance.GetKeybind("Escape"))) { 
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
        EndGameUI.SetActive(true);
    }
    //Currently used for figuring out which room the player is in and displaying it on the top right
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals(targetTag))
        {
            // Player is within a GameObject with the specified tag
        //Debug.Log("Player is within a GameObject with the tag: " + targetTag + " with object name: " + other.gameObject.name);
            roomText.GetComponent<TMP_Text>().text = other.gameObject.name;
        }

        if (other.tag.Equals(tutTag)) {
            Tutorial.Instance.ActivateTrigger(other.gameObject);
        }
    }
}
