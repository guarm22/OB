using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class ReportUI : MonoBehaviour {
    
    public GameObject togglePrefab;
    public GameObject typeSelectionUI;
    public Button reportButton;
    public GameObject playerLoc;

    public Color OriginalBGColor;
    public Color SelectedBGColor;

    public Color NormalButtonColor;
    public Color DisabledButtonColor;

    public AudioClip selectSound;
    public AudioClip pendingSound;
    public AudioSource audioSource;

    [HideInInspector]
    public List<GameObject> rooms;
    [HideInInspector]
    public List<string> SelectedTypes;
    [HideInInspector]
    public string SelectedRoom;
    public static ReportUI Instance;

    public List<string> Statuses = new List<string> {"Good", "Warning", "Danger"};
    public List<Color> StatusColors = new List<Color> {Color.green, Color.yellow, Color.red};
    public TMP_Text StatusText;

    void Start() {
        CreateUI();
        Instance = this;
        reportButton.onClick.AddListener(Report);
        GetRooms();
        audioSource = this.gameObject.GetComponent<AudioSource>();
    }

    void Update() {
        findPlayerLoc();
        UpdateButton();
        UpdateStatus();
    }

    private void UpdateStatus() {
        int divergences = DivergenceControl.Instance.DivergenceList.Count;
        int maxDivergences = DivergenceControl.Instance.Rooms.Count;
        float divergenceRatio = (float) divergences / maxDivergences;

        if(divergenceRatio <= Warning.warningThreshold) {
            StatusText.text = Statuses[0];
            StatusText.color = StatusColors[0];
        }
        else if(divergenceRatio <= Warning.dangerThreshold) {
            StatusText.text = Statuses[1];
            StatusText.color = StatusColors[1];
        }
        else {
            StatusText.text = Statuses[2];
            StatusText.color = StatusColors[2];
        }
    }

    private void UpdateButton() {
        if(SelectedTypes.Count*DivergenceControl.Instance.EnergyPerGuess > GameSystem.Instance.CurrentEnergy || SelectedRoom == null || SelectedTypes.Count == 0 || SelectedRoom == "") {
            reportButton.GetComponent<Image>().color = DisabledButtonColor;
        }
        else {
            reportButton.GetComponent<Image>().color = NormalButtonColor;
        }
    }

    public void findPlayerLoc() {
        foreach(GameObject room in rooms) {
            if(PlayerUI.Instance.GetPlayerRoom() == room.name) {
                playerLoc.transform.position = room.transform.position - new Vector3(0,Display.main.systemHeight*0.022f,0);
            }
        }
    }

    public void Report() {
        audioSource.PlayOneShot(selectSound);
        DivergenceControl.Instance.MakeSelection(SelectedTypes, SelectedRoom);
    }
    
    public void ResetSelections() {
        SelectedTypes = new List<string>();
        SelectedRoom = null;
        foreach(Toggle toggle in typeSelectionUI.GetComponentsInChildren<Toggle>()) {
            toggle.isOn = false;
            toggle.transform.GetChild(2).GetComponent<Image>().color = OriginalBGColor; // Change the color of the checkmark
        }
        foreach(GameObject room in rooms) {
            ChangeBGColor(room, OriginalBGColor);
        }
    }

    public void SelectType(GameObject obj) {
        Toggle toggle = obj.GetComponent<Toggle>();
        audioSource.PlayOneShot(selectSound);
        if(toggle.isOn) {
            toggle.transform.GetChild(2).GetComponent<Image>().color = SelectedBGColor; // Change the color of all images
            SelectedTypes.Add(obj.name);
        }
        else {
            toggle.transform.GetChild(2).GetComponent<Image>().color = OriginalBGColor; // Change the color of the checkmark
            SelectedTypes.Remove(obj.name);
        }
    }
    public void SelectRoom(GameObject room) {
        audioSource.PlayOneShot(selectSound);

        if(SelectedRoom == null) {
            //change image alpha to max
            ChangeBGColor(room, SelectedBGColor);
            SelectedRoom = room.name;
        }

        else if(SelectedRoom == room.name) {
            ChangeBGColor(room, OriginalBGColor);
            SelectedRoom = null;
        }

        else if(SelectedRoom != room.name) {
            foreach (GameObject r in rooms) {
                ChangeBGColor(r, OriginalBGColor);
            }
            ChangeBGColor(room, SelectedBGColor);
            SelectedRoom = room.name;
        }
    }

    public void GetRooms() {
        rooms = new List<GameObject>();
        foreach(GameObject child in GameObject.FindGameObjectsWithTag("RoomUI")) {
            EventTrigger trigger = child.AddComponent<EventTrigger>();
            rooms.Add(child.gameObject);
            // Create a new entry for the click event
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerClick;

            // Add a callback to the entry
            entry.callback.AddListener(delegate { SelectRoom(child); });

            // Add the entry to the trigger
            trigger.triggers.Add(entry);
        }
    }

    private void ChangeBGColor(GameObject room, Color color) {
        room.transform.GetChild(0).GetComponent<Image>().color = color;

    }

    public void CreateUI() {
        List<string> types = DynamicObject.GetAllAnomalyTypes();
        float iterY = 0f;
        float iterX = 0f;
        //Creates each of the type selectors
        foreach(string type in types) {
            if(iterY >= 295f) {
                iterX = 500f;
                iterY = 0f;
            }

            GameObject ui = Instantiate(togglePrefab, transform);
            ui.transform.SetParent(typeSelectionUI.transform);
            ui.transform.localPosition = new Vector3(iterX, iterY, 0f);
            ui.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = type;
            ui.transform.localScale = new Vector3(3f,3f,3f);
            ui.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
            ui.name = type;
            iterY+=100f;
            
            EventTrigger trigger = ui.AddComponent<EventTrigger>();
            // Create a new entry for the click event
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerClick;

            // Add a callback to the entry
            entry.callback.AddListener(delegate { SelectType(ui); });

            // Add the entry to the trigger
            trigger.triggers.Add(entry);
        }
    }
}
