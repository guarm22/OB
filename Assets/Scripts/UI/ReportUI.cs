using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public AudioSource audioSource;

    public TMP_Text energyCostText;

    [HideInInspector]
    public List<GameObject> rooms;
    [HideInInspector]
    public List<string> SelectedTypes;
    [HideInInspector]
    public string SelectedRoom;
    public static ReportUI Instance;

    public List<string> Statuses = new List<string> {"Stable", "Unstable", "Danger"};
    public List<Color> StatusColors = new List<Color> {Color.green, Color.yellow, Color.red};
    public TMP_Text StatusText;

    private bool ScrambledUI = false;
    private bool CurrentlyScrambling = false;

    public Light greenLight;
    public Light yellowLight;
    public Light redLight;

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
        UpdateEnergyCost();

        if(PunctureCollapse.Instance.isCollapsing) {
            if(!PlayerUI.Instance.reportDeviceUp) {return;}
            if(ScrambledUI == false) {
                ScrambledUI = true;
                ScrambleUI(true);
            }
            else if(!CurrentlyScrambling){
                ScrambleUI(true);
            }
            return;
        }

        if(PlayerUI.Instance.reportDeviceUp) {
            if(!ScrambledUI) {
                if(CanAnyHidersSeePlayer()){
                    ScrambleUI(true);
                }
            }
        }

        if(ScrambledUI) {
            if(!CanAnyHidersSeePlayer()) {
                ScrambleUI(false);
            }
            else if(CurrentlyScrambling == false){
                ScrambleUI(true);
            }
        }
    }

    public void ScrambleUI(bool active) {
        ScrambledUI = active;
        if(active) {
            StartCoroutine(Scramble());
        }
        else {
            StopAllCoroutines();
            ResetSelections();
        }
    }

    public void TurnOn() {
        if(CanAnyHidersSeePlayer()) {
            ScrambleUI(true);
        }
        else {
            CurrentlyScrambling = false;
            ScrambledUI = false;
        }
    }

    public bool CanAnyHidersSeePlayer() {
        List<Hider> hiders = FindObjectsByType<Hider>(FindObjectsSortMode.None).ToList();
        foreach(Hider hider in hiders) {
            if(hider.currentlySeeingPlayer){
                return true;
            }
        }
        return false;
    }

    public void TurnOff() {
        ScrambleUI(false);
        CurrentlyScrambling = false;
    }

    private IEnumerator Scramble() {
        //randomly select and deselect a few types and rooms
        //wait a few seconds
        //repeat
        CurrentlyScrambling = true;
        while(ScrambledUI) {

            yield return new WaitForSeconds(UnityEngine.Random.Range(0.08f,0.18f));
            foreach(Toggle toggle in typeSelectionUI.GetComponentsInChildren<Toggle>()) {
                if(UnityEngine.Random.Range(0, 2) == 0) {
                    toggle.isOn = !toggle.isOn;
                    SelectType(toggle.gameObject);
                }
            }
            foreach(GameObject room in rooms) {
                if(UnityEngine.Random.Range(0, 2) == 0) {
                    SelectRoom(room);
                }
            }
        }
    }

    private void UpdateEnergyCost() {
        if(SelectedTypes.Count*DivergenceControl.Instance.EnergyPerGuess > GameSystem.Instance.CurrentEnergy) {
            energyCostText.color = Color.red;
        }
        else if(SelectedTypes.Count*DivergenceControl.Instance.EnergyPerGuess >= GameSystem.Instance.CurrentEnergy-6) {
            energyCostText.color = Color.yellow;
        }
        else if(SelectedTypes.Count == 0) {
            energyCostText.color = Color.grey;
        }
        else {
            energyCostText.color = Color.green;
        }
        int totalCost = SelectedTypes.Count*DivergenceControl.Instance.EnergyPerGuess;
        String creatureCost = SelectedTypes.Contains("Creature") ? "/"+(totalCost-15).ToString() : "";
        energyCostText.text = "Energy Cost: " + (SelectedTypes.Count*DivergenceControl.Instance.EnergyPerGuess).ToString() + creatureCost + "%";
    }   

    private void UpdateStatus() {
        if(GameSystem.Instance.shouldPlaySound == false) {
            StatusText.text = "Unknown";
            StatusText.color = Color.grey;
            return;
        }

        int divergences = DivergenceControl.Instance.DivergenceList.Count;
        int maxDivergences = DivergenceControl.Instance.Rooms.Count;
        float divergenceRatio = (float) divergences / maxDivergences;

        if(divergenceRatio <= Warning.warningThreshold) {
            StatusText.text = " " + Statuses[0];
            StatusText.color = StatusColors[0];
            greenLight.gameObject.SetActive(true);
            yellowLight.gameObject.SetActive(false);
            redLight.gameObject.SetActive(false);
        }
        else if(divergenceRatio <= Warning.dangerThreshold) {
            StatusText.text = " " + Statuses[1];
            StatusText.color = StatusColors[1];
            greenLight.gameObject.SetActive(false);
            yellowLight.gameObject.SetActive(true);
            redLight.gameObject.SetActive(false);
        }
        else {
            StatusText.text = " " + Statuses[2];
            StatusText.color = StatusColors[2];
            greenLight.gameObject.SetActive(false);
            yellowLight.gameObject.SetActive(false);
            redLight.gameObject.SetActive(true);
        }
    }

    private void UpdateButton() {
        if(SelectedTypes.Count*DivergenceControl.Instance.EnergyPerGuess > GameSystem.Instance.CurrentEnergy || SelectedRoom == null || SelectedTypes.Count == 0 || SelectedRoom == "") {
            reportButton.GetComponentInChildren<TMP_Text>().color = DisabledButtonColor;
        }
        else {
            reportButton.GetComponentInChildren<TMP_Text>().color = NormalButtonColor;
        }
    }

    public void findPlayerLoc() {
        foreach(GameObject room in rooms) {
            if(PlayerUI.Instance.GetCurrentRoom() == room.name) {
                playerLoc.transform.localPosition = room.transform.localPosition - new Vector3(0,50,0);
                room.GetComponent<Image>().color = Color.green;
                continue;
            }
            room.GetComponent<Image>().color = Color.white;
        }
    }

    public void Report() {
        if(CurrentlyScrambling) {return;}

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
        float iterY = -100f;
        float iterX = 0f;
        //Creates each of the type selectors
        foreach(string type in types) {
            if(iterY >= 295f) {
                iterX = 500f;
                iterY = -100f;
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
