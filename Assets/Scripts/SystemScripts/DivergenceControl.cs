using System;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DivergenceControl : MonoBehaviour {

    [HideInInspector]
    public static DivergenceControl Instance;

    [HideInInspector]
    /// <summary>
    /// List of all active divergences in the scene.
    /// When a divergence is created, it is added to this list.
    /// </summary>
    public List<DynamicObject> DivergenceList = new List<DynamicObject>();

    [HideInInspector]
    /// <summary>
    /// List of all dynamic objects in the scene.
    /// This is what divergences are before they are activated.
    /// </summary>
    public List<DynamicObject> DynamicObjectList = new List<DynamicObject>();

    [HideInInspector]
    /// <summary>
    /// A dictionary where each key is a string representing a room name and the value is the number of dynamic objects in that room.
    /// </summary>
    public Dictionary<String, int> Rooms = new Dictionary<String, int>();

    /// <summary>
    /// The interval of time at which divergences are created.
    /// </summary>
    public float DivergenceInterval = 25f;

    [HideInInspector]
    /// <summary>
    /// The total number of dynamic objects in the scene.
    /// </summary>
    public int totalDynamicObjectsInScene;

    [HideInInspector]
    /// <summary>
    /// The time at which the last divergence was created.
    /// </summary>
    public float lastDivergenceTime = 0f;

    /// <summary>
    /// The total number of divergences allowed in a room.
    /// </summary>
    public int DivergencesPerRoom = 1;

    /// <summary>
    /// The amount of energy a report takes by default.
    /// </summary>
    public int EnergyPerGuess = 25;

    [HideInInspector]
    /// <summary>
    /// A list of reports made throughout a game.
    /// </summary>
    public List<Report> reports = new List<Report>();

    [HideInInspector]
    /// <summary>
    /// True if a report is currently pending, false if not.
    /// </summary>
    public bool PendingReport = false;

    /// <summary>
    /// The amount of time after a report is made before another report can be made.
    /// </summary>
    public float ReportLockout = 5f;

    [HideInInspector]
    /// <summary>
    /// Time of the last report made.
    /// </summary>
    public float TimeOfLastreport = 0f;

    [HideInInspector]
    public List<DynamicObject> DivergencesReportedCorrectly = new List<DynamicObject>();

    [HideInInspector]
    public bool WasMostRecentReportCorrect = false;

    [HideInInspector]
    /// <summary>
    /// A list of objects that were reported recently. These should be locked for a certain amount of time as to not have instant
    /// repeating divergences.
    /// </summary>
    public List<DynamicObject> LockedObjects = new List<DynamicObject>();

    public float divergenceRandomnessMax = 1.5f;
    public float divergenceRandomnessMin = -2f;
    [HideInInspector]
    public float currentRandomness;

    public int MaxDivergences = 4;

    /// <summary>
    /// Finds each game object with the DynamicData script and adds it to the DynamicObjectList.
    /// </summary>
    private void InitializeAllDynamicObjects() {
        DynamicData[] objects = FindObjectsOfType<DynamicData>();
        foreach(DynamicData obj in objects) {
            InitNewDynamicObject(obj.gameObject);
        }
        totalDynamicObjectsInScene = DynamicObjectList.Count;
    }

    private void InitNewDynamicObject(GameObject obj) {
        DynamicData data = obj.gameObject.GetComponent<DynamicData>();
        CustomDivergence cd = obj.gameObject.GetComponent<CustomDivergence>();
        data.customDivergence = cd;
        string room = DynamicObject.getRoomName(obj.transform);
        DynamicObject dynam = new DynamicObject(data, room, obj.name, obj);
        this.DynamicObjectList.Add(dynam);
    }

    /// <summary>
    /// Finds each game object with the tag "Room" and adds it to the Rooms dictionary.
    /// </summary>
    private void InitRooms() {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("Room");
        foreach(GameObject obj in objs) {
            Rooms.Add(obj.name, 0);
        }
    }

    /// <summary>
    /// Sets certain variables based on game settings
    /// </summary>
    private void InitGame() {
        lastDivergenceTime -= GameSystem.Instance.GracePeriod;
        generateNewRandomness();

        if(GameSystem.InEditor()) {
            return;
        }
        if(SceneManager.GetActiveScene().name == "Tutorial") {
            return;
        }
        Debug.Log("Difficulty: " + PlayerPrefs.GetString("Difficulty", "Normal"));
        switch(PlayerPrefs.GetString("Difficulty", "NotLoaded")) {
            case "NotLoaded":
                DivergenceInterval = GameSettings.NormalDivergenceRate;
                break;
            default:
                DivergenceInterval = PlayerPrefs.GetInt("DivergenceRate", 26);
                break;
        }
    }

    void Start() {
        Instance = this;

        InitializeAllDynamicObjects();
        InitRooms();

        InitGame();
    }

    /// <summary>
    /// Determines if any divergences can spawn in the scene.s
    /// </summary>
    /// <returns></returns>
    private bool CanAnyDivergencesActivate() {
        //Are there any dynamic objects available to spawn?
        //Are the divergences in the scene less than the total number of rooms * divergences per room?
        //Does each room have less than the maximum number of divergences?
        return DynamicObjectList.Count > 0 
        && DivergenceList.Count < Rooms.Count*DivergencesPerRoom 
        && areAllRoomsFull() == false;
    }

    /// <summary>
    /// Activates a random divergence in the scene.
    /// </summary>
    private void ActivateRandomDivergence() {
        //Check if any divergences are allowed to spawn
        if(CanAnyDivergencesActivate() == false) {
            //Random chance to make the level darker
            if(UnityEngine.Random.Range(0, 100) < 50) {
                LightControl.Instance.KillLights(0.40f);
            } 
            return;
        }

        //Choose a random object from the list of dynamic objects
        DynamicObject randomObject = null;
        //Search the dictionary of rooms for each room that has a value less than the maximum number of anomalies per room.
        List<string> availableRooms = new List<string>();
        foreach(KeyValuePair<string, int> entry in Rooms) {
            if(entry.Value < DivergencesPerRoom) {
                availableRooms.Add(entry.Key);
            }
        }

        if (availableRooms.Count > 0) {
            //Choose a random room from the list of available rooms
            string room = availableRooms[UnityEngine.Random.Range(0, availableRooms.Count)];
            //Choose a random object from the list of dynamic objects in the chosen room
            List<DynamicObject> objectsInRoom = DynamicObjectList.FindAll(x => x.Room == room);
            if(objectsInRoom.Count > 0) {
                randomObject = objectsInRoom[UnityEngine.Random.Range(0, objectsInRoom.Count)];
            }
        }
        //no objects in any room
        else {
            return;
        }

        //Activate divergence
        if(!randomObject.DoAnomalyAction(true)) {
            return;
        }

        GameSystem.Instance.PlayDivergenceSound();
        DynamicObjectList.Remove(randomObject);
        DivergenceList.Add(randomObject);
        Rooms[randomObject.Room] += 1;
    }

    /// <summary>
    /// Takes the name of a dynamic object and activates it as a divergence.
    /// </summary>
    /// <param name="name"></param>
    public void ManuallyActivateDivergence(string name) {
        DynamicObject obj = null;
        foreach(DynamicObject item in DynamicObjectList) {
            if (item.Obj.name == name) {
                obj = item;
            }
        }
        if(obj == null) {
            return;
        }
        GetComponent<AudioSource>().Play(); 
        obj.DoAnomalyAction(true);
        DynamicObjectList.Remove(obj);
        DivergenceList.Add(obj);
        Rooms[obj.Room] += 1;
    }

    public void CreateDivergence(GameObject obj) {
        DynamicData data = obj.gameObject.GetComponent<DynamicData>();
        CustomDivergence cd = obj.gameObject.GetComponent<CustomDivergence>();
        data.customDivergence = cd;
        string room = DynamicObject.getRoomName(obj.transform);
        DynamicObject dynam = new DynamicObject(data, room, obj.name, obj);
        DivergenceList.Add(dynam);
  }

    /// <summary>
    /// Checks if all rooms have the maximum amount of anomalies
    /// </summary>
    /// <returns>true if all rooms have maximum amount of anomalies, false if at least one room has room for an anomaly</returns>
    private bool areAllRoomsFull() {
        foreach (int amt in Rooms.Values) {
            if(amt >= DivergencesPerRoom) {
                continue;
            }
            else {
                return false;
            }
        }
        return true;
    }


    public void MakeSelection(List<String> types, String room) {
        List<DynamicObject> found = DivergenceList
            .Where(dynam => types.Any(type => DynamicObject.GetAnomalyTypeByName(type)==dynam.data.type && room == dynam.Room))
            .ToList();

        int totalCost = types.Count * Instance.EnergyPerGuess;
        foreach(DynamicObject d in found) {
             totalCost -= Instance.EnergyPerGuess - d.data.energyCost;
        }
        
        //if the player does:
        //  not have enough energy
        //  did not select a room
        //  did not select any types
        //play error sound and return
        if(GameSystem.Instance.CurrentEnergy < totalCost || room == null || types.Count < 1 || types == null) {
            StartCoroutine(SoundControl.Instance.guessFeedbackSound(false));
            return;
        }

        GameSystem.Instance.ChangeEnergy(-totalCost);

        PendingReport = true;
        TimeOfLastreport = Time.time;

        //Reset UI elements
        if(TypeSelection.Instance) {
            TypeSelection.CurrentlySelected.Clear();
            TypeSelection.Instance.ResetToggles();
            RoomSelection.Instance.ResetToggles();
        }

        if(ReportUI.Instance) {
            ReportUI.Instance.ResetSelections();
        }
        foreach(DynamicObject d in found) {DivergencesReportedCorrectly.Add(d);}

        Report r = new Report(
            types,
            found.Select(d => d.Obj.name).ToList(),
            found.Select(d => d.data.energyCost).ToList(),
            DateTime.Now.ToString("HH:mm:ss"),
            found.Select(d => (Time.time - d.divTime)+(ReportLockout/2)).ToList(),
            found.Count > 0,
            room,
            totalCost
        );
        reports.Add(r);

        GameSystem.Instance.ReportsMade += 1;
        GameSystem.Instance.DivergencesReported += found.Count;
    }

    public void CheckPendingReport() {
        PendingReport = false;

        int correctCount = DivergencesReportedCorrectly.Count + 
        (reports.Last().reportTypes.Contains("Creature") ? CreatureControl.Instance.CreatureReport(reports.Last().room) : 0);
        WasMostRecentReportCorrect = correctCount > 0;

        if(correctCount > 0) {

            GameSystem.Instance.AnomaliesSuccesfullyReportedThisGame += DivergencesReportedCorrectly.Count;
            StartCoroutine(SoundControl.Instance.guessFeedbackSound(true));

            foreach(DynamicObject d in DivergencesReportedCorrectly) {

                d.DoAnomalyAction(false);
                DivergenceList.Remove(d);

                Rooms[d.Room] -= 1;       
                LockedObjects.Add(d);
                if(LockedObjects.Count >= totalDynamicObjectsInScene/3) {
                    DynamicObjectList.Add(LockedObjects[0]);
                    LockedObjects.RemoveAt(0);
                }
            }
            DivergencesReportedCorrectly = new List<DynamicObject>();
        }
        else {
            StartCoroutine(SoundControl.Instance.guessFeedbackSound(false));
        }
    }

    private void CheckDivergenceSpawn() {
        if(Time.time - lastDivergenceTime > DivergenceInterval + currentRandomness) {
            lastDivergenceTime = Time.time;
            generateNewRandomness();
            ActivateRandomDivergence();
        }
    }

    private void CheckReport() {
        if(PendingReport && Time.time - TimeOfLastreport > ReportLockout) {
            CheckPendingReport();
        }
    }

    /// <summary>
    /// Generates a new randomness value between the min and max values for the divergence timer
    /// </summary>
    private void generateNewRandomness() {
        currentRandomness = UnityEngine.Random.Range(divergenceRandomnessMin, divergenceRandomnessMax);
        if(DivergenceInterval + currentRandomness <= 0.5f) {
            currentRandomness = 0;
        }
    }

    void Update() {
        CheckDivergenceSpawn();
        CheckReport();
    }
}
