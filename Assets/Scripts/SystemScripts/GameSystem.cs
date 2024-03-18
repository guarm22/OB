using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using System.Collections;
using System;
public class GameSystem : MonoBehaviour, IDataPersistence
{
  public int GuessLockout;
  public static GameSystem Instance { get; private set; }
  public List<DynamicObject> Anomalies {get; private set;}
  public List<DynamicObject> DynamicObjects {get; private set;}
  public static float LastGuess {get; private set;}
  public static bool Guessed {get; private set;}
  public static List<DynamicObject> CorrectObject {get; private set;}
  public AudioClip DisappearSound;
  public int GameStartTime;
  public int GameObjectDisappearanceInterval;
  public int AnomaliesPerRoom;
  public int TotalAnomalies;
  public float MaxEnergy = 100;
  public float CurrentEnergy;
  private float LastEnergyCheck=0f;
  private float EnergyCheckInterval=1f;
  public float energyPerSecond = 1.2f;
  public int MaxDivergences = 4;
  public int EnergyPerGuess = 25;
  public Dictionary<string, int> Rooms;
  public float gameTime;
  public Text gameTimer;
  public float startTime = 60*15f;
  public float timeSinceLastDisappearance;
  private int AnomaliesSuccesfullyReported = 0;
  public int AnomaliesSuccesfullyReportedThisGame;
  public bool GameOver = false;
  public bool Won = false;
  public float reportTextTimer = 5f;
  public float divergenceRandomnessMax = 1.5f;
  public float divergenceRandomnessMin = -2f;
  public float currentRandomness;
  public bool madeGuess;
  public List<DynamicObject> lockedObjects;
  public static int totalDynamicObjectsInScene;
  public bool wasLastGuessCorrect = false;

  public void LoadData(GameData data) {
        AnomaliesSuccesfullyReported = data.AnomaliesSuccesfullyReported;
  }
  public void SaveData(ref GameData data) {
        data.AnomaliesSuccesfullyReported = data.AnomaliesSuccesfullyReported + AnomaliesSuccesfullyReportedThisGame;
  }
  void Start()
  {
    if (Instance != null) {
      Debug.LogError("There is more than one instance!");
      return;
    }
    SetGameSettings();
    gameTime = startTime;
    Instance = this;
    Anomalies = new List<DynamicObject>();
    DynamicObjects = new List<DynamicObject>();
    Rooms = new Dictionary<string, int>();
    Guessed = false;
    LastGuess = -5;
    CorrectObject = new List<DynamicObject>();
    timeSinceLastDisappearance = 0f - GameStartTime;
    AnomaliesSuccesfullyReportedThisGame = 0;
    CurrentEnergy = MaxEnergy;
    TotalAnomalies = 0;
    madeGuess = false;
    lockedObjects = new List<DynamicObject>();
    gameObject.AddComponent<AudioSource>();
    gameObject.GetComponent<AudioSource>().clip = DisappearSound;
    generateNewRandomness();
    InstantiateAllDynamicObjects();
  }

  private static bool InEditor() {
    #if UNITY_EDITOR
    return true;
    #endif
    return false;
  }

  private void SetGameSettings() {
    if(InEditor()) {
        return;
    }
    if(SceneManager.GetActiveScene().name == "Tutorial") {
        return;
    }
    Debug.Log("Difficulty: " + PlayerPrefs.GetString("Difficulty", "Normal"));
    switch(PlayerPrefs.GetString("Difficulty", "NotLoaded")) {
        case "NotLoaded":
            GameObjectDisappearanceInterval = GameSettings.NormalDivergenceRate;
            MaxDivergences = GameSettings.NormalCreatureThreshold;
            CreatureControl.Instance.creatureMax = 3;
            energyPerSecond = GameSettings.NormalEPS;
            GameStartTime = GameSettings.NormalGracePeriod;
            CreatureControl.Instance.creatureSpawnRate = GameSettings.NormalCreatureSpawnRate;
            break;
        default:
            GameObjectDisappearanceInterval = PlayerPrefs.GetInt("DivergenceRate", 22);
            MaxDivergences = PlayerPrefs.GetInt("MaxDivergences", 4);
            CreatureControl.Instance.creatureMax = 3;
            energyPerSecond = PlayerPrefs.GetFloat("EPS", 1.1f);
            GameStartTime = PlayerPrefs.GetInt("GameStartTime", 15);
            CreatureControl.Instance.creatureSpawnRate = PlayerPrefs.GetFloat("CreatureSpawnRate", 20f);
            break;
    }
  
  }

/// <summary>
/// Runs on game start, takes every object in the world with the dynamic data script and instantiates it as an object
/// 
/// </summary>
  void InstantiateAllDynamicObjects() {
    DynamicData[] objects = FindObjectsOfType<DynamicData>();
    foreach(DynamicData obj in objects) {
        CreateDynamicObject(obj.gameObject);
    }
    totalDynamicObjectsInScene = DynamicObjects.Count;
  }
    void CreateDynamicObject(GameObject obj) {
        DynamicData data = obj.gameObject.GetComponent<DynamicData>();
        CustomDivergence cd = obj.gameObject.GetComponent<CustomDivergence>();
        data.customDivergence = cd;
        string room = getRoomName(obj.transform);
        DynamicObject dynam = new DynamicObject(data,room,obj.name,obj);
        this.DynamicObjects.Add(dynam);
        if(!Rooms.ContainsKey(room)) {
            Rooms.Add(room, 0);
        }
    }

    public void CreateDivergence(GameObject obj) {
        DynamicData data = obj.gameObject.GetComponent<DynamicData>();
        CustomDivergence cd = obj.gameObject.GetComponent<CustomDivergence>();
        data.customDivergence = cd;
        string room = getRoomName(obj.transform);
        DynamicObject dynam = new DynamicObject(data, room, obj.name, obj);
        Anomalies.Add(dynam);
  }

    private static string getRoomName(Transform obj) {
        while (obj != null) {
            if (obj.tag == "Room") {
                return obj.name;
            }
            obj = obj.parent;
        }
        return "";
    }
    public string getRandomRoom() {
        List<string> rooms = new List<string>(Instance.Rooms.Keys);
        return rooms[UnityEngine.Random.Range(0, rooms.Count)];
    }

    public void GetRandomDynamicObject() {
        if(TotalAnomalies >= Rooms.Count*AnomaliesPerRoom || areAllRoomsFull() || DynamicObjects.Count == 0) {
            if(UnityEngine.Random.Range(0, 100) < 50) {
                LightControl.Instance.KillLights(0.40f);
            } 
            return;
        }
        DynamicObject randomObject = null;
        int amt = -1;
        List<string> availableRooms = DynamicObjects.GroupBy(d => d.Room)
        .Where(g => HowManyDivergencesInRoom(g.Key) < AnomaliesPerRoom)
        .Select(g => g.Key)
        .ToList();

        if (availableRooms.Count > 0) {
            do {
                string room = availableRooms[UnityEngine.Random.Range(0, availableRooms.Count)];
                // Choose a random object from the selected room
                randomObject = DynamicObjects
                    .Where(d => d.Room == room)
                    .OrderBy(d => Guid.NewGuid())
                    .FirstOrDefault();
                if (randomObject != null) {
                    Rooms.TryGetValue(randomObject.Room, out amt);
                }
            }
            while (randomObject == null || amt >= AnomaliesPerRoom);
        }
        if(randomObject.DoAnomalyAction(true) == 0) {
            return;
        }

        int lowerBound = TotalAnomalies < 3 ? 0 : TotalAnomalies == 3 ? 20 : TotalAnomalies > 3 ? 33 : 33;
        if(UnityEngine.Random.Range(0, 99) < lowerBound) {
            LightControl.Instance.FlickerLights();
        }
        GetComponent<AudioSource>().Play();
        DynamicObjects.Remove(randomObject);
        Anomalies.Add(randomObject);
        TotalAnomalies++;
        Rooms[randomObject.Room] += 1;
    }

    public void ManuallyCreateDivergence(string name) {
        DynamicObject obj = null;
        foreach(DynamicObject item in DynamicObjects) {
            if (item.Obj.name == name) {
                obj = item;
            }
        }
        if(obj == null) {
            return;
        }
        GetComponent<AudioSource>().Play(); 
        obj.DoAnomalyAction(true);
        DynamicObjects.Remove(obj);
        Anomalies.Add(obj);
        TotalAnomalies++;
        Rooms[obj.Room] += 1;
    }
    public int HowManyDivergencesInRoom(string room) {
        return Anomalies.Count(d => d.Room.Equals(room) && d.data.type != ANOMALY_TYPE.Creature);
    }

    /// <summary>
    /// Takes in a type and room, and sets the "PrivateCorrectGuess" variable true or false depending on whether there is an anomaly with that room and type
    /// </summary>
    /// <param name="type"></param>
    /// <param name="room"></param>
    public void MakeSelection(List<string> types, string room) {
        List<DynamicObject> found = Anomalies
            .Where(dynam => types.Any(type => DynamicObject.GetAnomalyTypeByName(type) == dynam.data.type && room == dynam.Room))
            .ToList();

        int totalCost = types.Count * Instance.EnergyPerGuess;
        foreach(DynamicObject d in found) {
             totalCost -= Instance.EnergyPerGuess - d.data.energyCost;
        }
        if(Instance.CurrentEnergy < totalCost || room.Length<1) {
            Instance.StartCoroutine(SoundControl.Instance.guessFeedbackSound(false));
            return;
        }
        Instance.madeGuess = true;
        Instance.CurrentEnergy -= totalCost;

        Guessed = true;
        LastGuess = Time.time;
        TypeSelection.CurrentlySelected.Clear();
        TypeSelection.Instance.ResetToggles();
        RoomSelection.Instance.ResetToggles();
        foreach(DynamicObject d in found) {CorrectObject.Add(d);}
    }

    /// <summary>
    /// Updates each frame with the time since the last guess
    /// </summary>
    void SetGuessTime() {
        if(Time.time - LastGuess >= GuessLockout/2 && madeGuess == true) {
            madeGuess = false;
            wasLastGuessCorrect = CorrectObject.Count > 0;
            if(wasLastGuessCorrect) {
                StartCoroutine(SoundControl.Instance.guessFeedbackSound(true));
                foreach(DynamicObject d in CorrectObject) {
                    d.DoAnomalyAction(false);
                    Anomalies.Remove(d);
                    if(d.data.type == ANOMALY_TYPE.Creature) {
                        CreatureControl.Instance.RemoveCreature(d.Obj);
                    }
                    else {
                        TotalAnomalies--;
                        Rooms[d.Room] -= 1;       
                        lockedObjects.Add(d);
                        if(lockedObjects.Count >= totalDynamicObjectsInScene/3) {
                            DynamicObjects.Add(lockedObjects[0]);
                            lockedObjects.RemoveAt(0);
                        }
                    }
                }
                AnomaliesSuccesfullyReportedThisGame += CorrectObject.Count;
                CorrectObject = new List<DynamicObject>();
            }
            else {
                StartCoroutine(SoundControl.Instance.guessFeedbackSound(false));
            }
        }
        if(Time.time - LastGuess >= GuessLockout && Guessed == true) {
            Guessed = false;
        }
    }

    private bool areAllRoomsFull() {
        foreach (int amt in Rooms.Values) {
            if(amt >= AnomaliesPerRoom) {
                continue;
            }
            else {
                return false;
            }
        }
        return true;
    }

    public void SetGameTime(float t=-1) {
        if (Input.GetKeyDown(KeyCode.N)) {
            StartCoroutine(EndGame("manual"));
            return;
        }
        if (t > 0) {
            gameTime = t;
        }
        if (gameTime <= 1) {
            Won = true;
            StartCoroutine(EndGame("won"));
            return;
        }
        gameTime -= Time.deltaTime;
        int minutes = Mathf.FloorToInt(gameTime / 60);
        int seconds = Mathf.FloorToInt(gameTime % 60);
        gameTimer.text = $"{minutes:D2}:{seconds:D2}";
    }

    public IEnumerator EndGame(string reason="") {
        GameOver = true;
        if(reason.Equals("zombie") || reason.Equals("yippie")) {
            yield return StartCoroutine(CreatureControl.Instance.ZombieJumpscare());
        }
        CreatureControl.Instance.IsJumpscareFinished=true;
        cleanUpGame();
    }

    public void cleanUpGame() {
        foreach (DynamicObject div in Anomalies) {
            //destroy all creatures because it looks weird when theyre walking around
            if (div.data.type == ANOMALY_TYPE.Creature) {
                Destroy(div.Obj);
            }
        }
    }
    private void generateNewRandomness() {
        currentRandomness = UnityEngine.Random.Range(divergenceRandomnessMin, divergenceRandomnessMax);
        if(GameObjectDisappearanceInterval + currentRandomness <= 0.5f) {
            currentRandomness = 0;
        }
    }
    private void CheckTimer() {
        if(timeSinceLastDisappearance > GameObjectDisappearanceInterval + currentRandomness) {
            generateNewRandomness();
            timeSinceLastDisappearance = 0f;
            GetRandomDynamicObject();
        }
        else {
            timeSinceLastDisappearance += Time.deltaTime;
        }
    }
    private void UpdateEnergy() {
        if(Time.time - LastEnergyCheck >= EnergyCheckInterval) {
            if(CurrentEnergy >= 100) {
                CurrentEnergy = 100;
            }
            else {
                CurrentEnergy = CurrentEnergy + energyPerSecond;
            }
            LastEnergyCheck = Time.time;
        }
    }

  void Update()
  {
    if(SC_FPSController.paused || GameOver) {
        return;
    }
    CheckTimer();
    SetGameTime();
    SetGuessTime();
    UpdateEnergy();
  }

}
