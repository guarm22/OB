using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ReportUI : MonoBehaviour {
    
    public GameObject togglePrefab;
    public GameObject typeSelectionUI;
    public Button reportButton;
    public GameObject playerLoc;

    public Color OriginalBGColor;
    public Color SelectedBGColor;

    [HideInInspector]
    public List<GameObject> rooms;
    [HideInInspector]
    public List<string> SelectedTypes;
    [HideInInspector]
    public string SelectedRoom;

    public static ReportUI Instance;

    void Start() {
        CreateUI();
        GetRooms();
        Instance = this;
        reportButton.onClick.AddListener(Report);
    }

    void Update() {
        findPlayerLoc();
    }

    public void findPlayerLoc() {
        GameObject player = GameObject.Find("Player");
        foreach(GameObject room in rooms) {
            if(PlayerUI.Instance.GetPlayerRoom() == room.name) {
                playerLoc.transform.position = room.transform.position - new Vector3(0,-50,0);
            }
        }
    }

    public void Report() {
        GameSystem.Instance.MakeSelection(SelectedTypes, SelectedRoom);
    }
    
    public void ResetSelections() {
        SelectedTypes = new List<string>();
        SelectedRoom = null;
        foreach(Toggle toggle in typeSelectionUI.GetComponentsInChildren<Toggle>()) {
            toggle.isOn = false;
        }
        foreach(GameObject room in rooms) {
            room.GetComponent<Image>().color = OriginalBGColor;
        }
    }

    public void SelectType(GameObject obj) {
        Toggle toggle = obj.GetComponent<Toggle>();
        Image toggleBackground = toggle.targetGraphic as Image; // Get the Image component
        if(toggle.isOn) {
            ChangeColorOfAllImages(obj, Color.gray); // Change the color of all images
            SelectedTypes.Add(obj.name);
        }
        else {
            toggleBackground.color = Color.white; // Change the background color back
            toggle.transform.GetChild(0).GetComponent<Image>().color = Color.black; // Change the color of the checkmark
            SelectedTypes.Remove(obj.name);
        }
    }
    public void SelectRoom(GameObject room) {
        Color roomColor = room.GetComponent<Image>().color;
        if(roomColor == OriginalBGColor) {
            foreach (GameObject r in rooms) {
                r.GetComponent<Image>().color = OriginalBGColor;
            }
            room.GetComponent<Image>().color = SelectedBGColor;
            SelectedRoom = room.name;
            }
            else {
                room.GetComponent<Image>().color = OriginalBGColor;
                SelectedRoom = null;
            }
    }

    public void GetRooms() {
        rooms = new List<GameObject>();
        foreach(GameObject child in GameObject.FindGameObjectsWithTag("RoomUI")) {
            rooms.Add(child.gameObject);
            EventTrigger trigger = child.AddComponent<EventTrigger>();

            // Create a new entry for the click event
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerClick;

            // Add a callback to the entry
            entry.callback.AddListener(delegate { SelectRoom(child); });

            // Add the entry to the trigger
            trigger.triggers.Add(entry);
        }
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
            ui.transform.GetChild(1).gameObject.GetComponent<Text>().text = type;
            ui.transform.localScale = new Vector3(3f,3f,3f);
            ui.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
            ui.name = type;
            iterY+=100f;
            ui.GetComponent<Toggle>().onValueChanged.AddListener(
            delegate { SelectType(ui); });       
        }
    }

    private void ChangeColorOfAllImages(GameObject gameObject, Color color) {
        Image[] images = gameObject.GetComponentsInChildren<Image>();
        foreach (Image image in images) {
            image.color = color;
        }
    }
}
