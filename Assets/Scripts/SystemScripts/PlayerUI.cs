using System.Collections;
using System.Collections.Generic;
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
    public GameObject journalUI;
    public GameObject journalPanel;
    public GameObject togglePrefab;
    public GameObject roomText;
    public GameObject escapeMenuUI;
    public GameObject EndGameUI;
    public GameObject debugUI;
    public string targetTag = "Room";
    public string tutTag = "Tutorial";
    public string interactableTag;
    public bool inJournal = false;
    public bool inMenu = false;
    public static bool paused = false;
    public float maxDistance = 5.0f;
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
        Interact();
    }

    public string GetPlayerRoom() {
        return roomText.GetComponent<Text>().text;
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
            ui.transform.GetChild(1).gameObject.GetComponent<Text>().text = rooms[i].name;
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
            ui.transform.GetChild(1).gameObject.GetComponent<Text>().text = type;
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
        defaultUI.transform.GetChild(0).GetChild(1).gameObject.SetActive(true);
        SC_FPSController.Instance.canMove = true;
    }
    private void turnOnSelection() {
        selectionUI.SetActive(true);
        defaultUI.transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        inMenu = true;
        SC_FPSController.Instance.canMove = false;
    }

    void SelectionMenu() {
        if(inJournal) {
            return;
        }
        bool beforeTimer = Time.time - GameSystem.LastGuess >= GameSystem.Instance.GuessLockout-GameSystem.Instance.reportTextTimer;
        //if menu is open, check if tab is pressed to close, otherwise stop movement
        if(inMenu) {
            if(Input.GetKeyDown(KeyCode.Tab) || Time.time - GameSystem.LastGuess < 1) {
                turnOffSelection();
            }
        }
        else if(Input.GetKeyDown(KeyCode.Tab) && beforeTimer) {
            turnOnSelection();
        }
    }

    void Interact() {
        if(Input.GetKeyDown(KeyCode.E)) { 
            if(inJournal) {
                SC_FPSController.Instance.canMove = true;
                inJournal = false;
                journalUI.SetActive(false);
                return;
            }
            // Determine the direction that the player is looking
            Vector3 lookDirection = Camera.main.transform.forward;

            // Cast a ray in the look direction
            RaycastHit hit;
            if (Physics.Raycast(transform.position, lookDirection, out hit, maxDistance)) {
                // Check if the object has the desired tag
                if (hit.collider.gameObject.CompareTag(interactableTag)){
                    // The ray hit an object with the target tag
                    //hit.collider.gameObject.name
                    GameObject found = Interactable.GetInteractableByName(hit.collider.gameObject.name);

                    if(found != null) {
                        Interactable script = found.GetComponent<Interactable>();
                        string[] text = script.GetJournalText();

                        JournalPopup journalScript = journalPanel.GetComponent<JournalPopup>();
                        journalScript.SetText(text);

                        journalUI.SetActive(true);
                        inJournal = true;
                        SC_FPSController.Instance.canMove = false;
                    }
                }
                else {
                    // The ray did not hit an object with the target tag

                }
            }
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

    private void EscapeMenu() {
        //CHANGE TO ESCAPE
        if(Input.GetKeyDown(KeyCode.Q)) { 
            if(!paused) {
                openEscape();
            }
            else {
                closeEscape();
            }
        }
        if(Input.GetKeyDown(KeyCode.P)) {
            Popup.Instance.OpenPopup("Test message!");
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
            roomText.GetComponent<Text>().text = other.gameObject.name;
        }

        if (other.tag.Equals(tutTag)) {
            Tutorial.Instance.ActivateTrigger(other.gameObject);
        }
    }
}
