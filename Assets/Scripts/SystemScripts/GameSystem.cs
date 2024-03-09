using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
public class GameSystem : MonoBehaviour, IDataPersistence
{
  public int GuessLockout;
  public static GameSystem Instance { get; private set; }
  public static List<DynamicObject> Anomalies {get; private set;}
  public static List<DynamicObject> DynamicObjects {get; private set;}
  public GameObject zombiePrefab;
  public GameObject endCreaturePrefab;
  public static float LastGuess {get; private set;}
  public static bool Guessed {get; private set;}
  public static bool PrivateCorrectGuess {get; private set;}
  public static bool CorrectGuess {get; set;}
  public static List<DynamicObject> CorrectObject {get; private set;}
  public AudioClip DisappearSound;
  public int GameStartTime;
  public int GameObjectDisappearanceInterval;
  public int AnomaliesPerRoom;

  public int TotalAnomalies;

  public int maxCreaturesPerRoom = 1;

  public float MaxEnergy = 100;
  public float CurrentEnergy;
  private float LastEnergyCheck=0f;
  private float EnergyCheckInterval=1f;
  public float energyPerSecond = 1.2f;
  private bool justPaused = false;
  public int MaxDivergences = 4;

  public int EnergyPerGuess = 25;
  private Dictionary<AudioSource, float> pausedAudioSources = new Dictionary<AudioSource, float>();


  public static Dictionary<string, int> Rooms;

  public static Dictionary<string, int> CreaturesPerRoom;

  public float gameTime;
  public Text gameTimer;
  public float startTime = 60*15f;

  public float timeSinceLastDisappearance;

  private int AnomaliesSuccesfullyReported = 0;
  public int AnomaliesSuccesfullyReportedThisGame;

  public bool GameOver = false;

  public bool Won = false;

  public int creatureMax;
  public float reportTextTimer = 5f;
  
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
    gameTime = startTime;
    Instance = this;
    Anomalies = new List<DynamicObject>();
    DynamicObjects = new List<DynamicObject>();
    Rooms = new Dictionary<string, int>();
    CreaturesPerRoom = new Dictionary<string, int>();
    Guessed = false;
    LastGuess = -5;
    PrivateCorrectGuess = false;
    CorrectObject = new List<DynamicObject>();
    timeSinceLastDisappearance = 0f - GameStartTime;
    AnomaliesSuccesfullyReportedThisGame = 0;
    CurrentEnergy = MaxEnergy;
    TotalAnomalies = 0;

    SetGameSettings();

    InstantiateAllDynamicObjects();

    //InvokeRepeating("GetRandomDynamicObject", GameStartTime, GameObjectDisappearanceInterval);
  }

  private void SetGameSettings() {
    #if UNITY_EDITOR
    return;
    #endif

    if(SceneManager.GetActiveScene().name == "Tutorial") {
        return;
    }

    Debug.Log("Difficulty: " + PlayerPrefs.GetString("Difficulty", "Normal"));
    switch(PlayerPrefs.GetString("Difficulty", "Normal")) {
        case "Easy":
            GameObjectDisappearanceInterval = 28;
            AnomaliesPerRoom = 1;
            MaxDivergences = 4; //divergences before creatures start spawning
            creatureMax = 3; //max creatures in the world before enders start spawning
            energyPerSecond = 0.95f;
            break;
        case "Normal":
            GameObjectDisappearanceInterval = 22;
            AnomaliesPerRoom = 1;
            MaxDivergences = 4;
            creatureMax = 3;
            energyPerSecond = 1.1f;
            break;
        case "Hard":
            GameObjectDisappearanceInterval = 18;
            AnomaliesPerRoom = 1;
            MaxDivergences = 5;
            creatureMax = 2;
            energyPerSecond = 1.3f;
            break;
        case "Custom":
            GameObjectDisappearanceInterval = PlayerPrefs.GetInt("DivergenceRate", 22);
            AnomaliesPerRoom = 1;
            MaxDivergences = 4;
            creatureMax = 3;
            energyPerSecond = PlayerPrefs.GetFloat("EPS", 1.1f);
            break;
    }
  
  }

/// <summary>
/// Runs on game start, takes every object in the world with the dynamic tag and instantiates it as an object
/// 
/// </summary>
  static void InstantiateAllDynamicObjects() {
    DynamicData[] objects = FindObjectsOfType<DynamicData>();
    foreach(DynamicData obj in objects) {
        GameObject gameobj = obj.transform.gameObject; 

        CustomDivergence cd = obj.gameObject.GetComponent<CustomDivergence>();
        obj.customDivergence = cd;

        string room = getRoomName(obj.transform);

        DynamicObject dynam = new DynamicObject(
            obj,
            room,
            obj.name,
            gameobj
        );
        DynamicObjects.Add(dynam);

        if(!Rooms.ContainsKey(room)) {
            Rooms.Add(room, 0);
            CreaturesPerRoom.Add(room, 0);
        }
    }
  }
    static void CreateDivergence(GameObject obj) {
        DynamicData data = obj.gameObject.GetComponent<DynamicData>();
        CustomDivergence cd = obj.gameObject.GetComponent<CustomDivergence>();
        data.customDivergence = cd;

        string room = getRoomName(obj.transform);

        DynamicObject dynam = new DynamicObject(
            data,
            room,
            obj.name,
            obj
        );
        Anomalies.Add(dynam);
        Rooms[room] += 1;
        Instance.TotalAnomalies++;
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

  private static ANOMALY_TYPE GetAnomalyTypeByName(string name) {
    switch(name){
        case "Light":
            return ANOMALY_TYPE.Light;
        case "Disappearance":
            return ANOMALY_TYPE.Disappearance;
        case "Replacement":
            return ANOMALY_TYPE.Replacement;
        case "Creature":
            return ANOMALY_TYPE.Creature;
        case "Audio":
            return ANOMALY_TYPE.Audio;
        case "Movement":
            return ANOMALY_TYPE.Movement;
        default:
            return ANOMALY_TYPE.NONE;
    }
  }

    public void GetRandomDynamicObject() {
        if(Anomalies.Count >= Rooms.Count*AnomaliesPerRoom || areAllRoomsFull() || DynamicObjects.Count == 0) {
            //Debug.Log("Full --- DynamicObjects Count: " + DynamicObjects.Count);
            //Each room has an Anomaly
            return;
        }
        //Find all dynamic objects
        int index = UnityEngine.Random.Range(0, DynamicObjects.Count);
        // Select the object at the random index
        DynamicObject randomObject = DynamicObjects[index];
        //we dont want to move 2 objects from the same room
        //var to hold amount of anomalies in room of obj
        int amt = Rooms.ContainsKey(randomObject.Room) ? Rooms[randomObject.Room] : -1;
        if(amt >= AnomaliesPerRoom || AnyAvailableDynamicObjectsInRoom(randomObject.Room)) {
            //Debug.Log("Already an anomaly in " + obj.Obj.transform.parent.name);
            GetRandomDynamicObject();
            return;
        }
        Rooms[randomObject.Room] += 1;
        
        // Do something with the random object
        if(randomObject.DoAnomalyAction(true) == 0) {
            return;
        }
        AudioSource audioSource = randomObject.Obj.AddComponent<AudioSource>();
        audioSource.clip = DisappearSound;
        audioSource.Play();

        DynamicObjects.Remove(randomObject);
        //Add to current anomalies
        Anomalies.Add(randomObject);
        TotalAnomalies++;
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
        
        obj.DoAnomalyAction(true);
        AudioSource audioSource = obj.Obj.AddComponent<AudioSource>();
        audioSource.clip = DisappearSound;
        audioSource.Play();

        DynamicObjects.Remove(obj);
        //Add to current anomalies
        Anomalies.Add(obj);
        TotalAnomalies++;
        Rooms[obj.Room] += 1;
    }

    public bool AnyAvailableDynamicObjectsInRoom(string room) {
        List<DynamicObject> anoms = getAnomaliesByRoom(room);
        List<DynamicObject> dynams = getDynamicObjectsByRoom(room);
        int dlen = dynams.Count;
        int count = 0;
        foreach(DynamicObject d in dynams) {
            foreach(DynamicObject a in anoms) {
                if(d.data.type == a.data.type) {
                    count++;
                    break;
                }   
            }
        }
        if(count == dlen) {
            return true;
        }
        return false;
    }

    /// <summary>
    /// Takes in a type and room, and sets the "PrivateCorrectGuess" variable true or false depending on whether there is an anomaly with that room and type
    /// </summary>
    /// <param name="type"></param>
    /// <param name="room"></param>
    public static void MakeSelection(List<string> types, string room) {
        if(Instance.CurrentEnergy < Instance.EnergyPerGuess * types.Count || room.Length<1) {
            return;
        }
        List<DynamicObject> found = new List<DynamicObject>();
        foreach(DynamicObject dynam in Anomalies) {
            foreach(string type in types) {
                if(GetAnomalyTypeByName(type)==dynam.data.type && room == dynam.Room) {
                    found.Add(dynam);
                }
            }
        }

        //Creatures cost less energy to report
        Instance.CurrentEnergy = Instance.CurrentEnergy-(Instance.EnergyPerGuess * types.Count);

        foreach(DynamicObject d in found) {
            if(d.data.type == ANOMALY_TYPE.Creature) {
                Instance.CurrentEnergy = Instance.CurrentEnergy + 18;
            }
        }

        Guessed = true;
        LastGuess = Time.time;
        if(found.Count == 0) {
            //incorrect guess
            PrivateCorrectGuess = false;
            return;
        }
        else {
            foreach(DynamicObject d in found) {CorrectObject.Add(d);}
            PrivateCorrectGuess = true;
        }
    }

    /// <summary>
    /// Updates each frame with the time since the last guess
    /// </summary>
    void SetGuessTime() {
        if(Time.time - LastGuess >= GuessLockout-reportTextTimer && Guessed == true) {
            CorrectGuess = PrivateCorrectGuess;
            if(CorrectObject != null) {
                foreach(DynamicObject d in CorrectObject) {
                    //Stops multiple audio sources from going on a single gameobject
                    Destroy(d.Obj.GetComponent<AudioSource>());
                    //counter
                    this.AnomaliesSuccesfullyReportedThisGame += 1;
                    //there is 1 less divergence in the reported room
                    Rooms[d.Room] -= 1;
                    //undo divergence
                    d.DoAnomalyAction(false);
                    Anomalies.Remove(d);

                    if(d.data.type == ANOMALY_TYPE.Creature) {
                        Destroy(d.Obj);
                        CreaturesPerRoom[d.Room] -= 1;
                    }
                    else {       
                        DynamicObjects.Add(d);
                    }
                    CorrectObject = new List<DynamicObject>();
                }
            }
        }
        if(Time.time - LastGuess >= GuessLockout && Guessed == true) {
            Guessed = false;
            CorrectGuess = PrivateCorrectGuess;
        }
    }

    private bool areAllRoomsFull() {
        foreach (string str in Rooms.Keys) {
            if(isRoomAllAnomaliesActive(str)) {
                continue;
            }
            else {
                return false;
            }
        }
        return true;
    }

    private bool isRoomAllAnomaliesActive(string str) {
        List<DynamicObject> anoms = getAnomaliesByRoom(str);
        List<DynamicObject> dynams = getDynamicObjectsByRoom(str);
        int alen = anoms.Count;
        int dlen = dynams.Count;

        //we need to check that based on the current active anomalies
        //that the only other possible anomalies are ones that share a type with a currently active anomaly
        int count = 0;
        foreach(DynamicObject d in dynams) {
            foreach(DynamicObject a in anoms) {
                if(d.data.type == a.data.type) {
                    count++;
                    break;
                }   
            }
        }
        if(count == dlen) {
            return true;
        }

        //if there are NO dynamic objects in the room, just return false
        if(alen+dlen == 0) {
            return false;
        }

        //no anomalies in the room
        if(alen == 0) {
            return false;
        }

        if(dlen == 0) {
            return true;
        }

        if(Rooms[str] == AnomaliesPerRoom) {
            return true;
        }

        return false;
    }

    private List<DynamicObject> getAnomaliesByRoom(string room) {
        return Anomalies.Where(d => d.Room.Equals(room)).ToList();
    }

    private List<DynamicObject> getDynamicObjectsByRoom(string room) {
        return DynamicObjects.Where(d => d.Room.Equals(room)).ToList();
    }

    public void SetGameTime(float t=-1) {
        if (Input.GetKeyDown(KeyCode.N)) {
            EndGame();
            return;
        }

        if (t > 0)
        {
            gameTime = t;
        }

        if (gameTime <= 1)
        {
            Won = true;
            EndGame();
            return;
        }

        gameTime -= Time.deltaTime;
        int minutes = Mathf.FloorToInt(gameTime / 60);
        int seconds = Mathf.FloorToInt(gameTime % 60);
        gameTimer.text = $"{minutes:D2}:{seconds:D2}";
    }

    public void EndGame() {
        GameOver = true;
        cleanUpGame();
    }

    public void cleanUpGame() {
        foreach(DynamicObject div in Anomalies) {
            //destroy all creatures because it looks weird when theyre walking around
            if(div.data.type == ANOMALY_TYPE.Creature) {
                Destroy(div.Obj);
            }
        }
    }

    private void CheckTimer() {
        if(timeSinceLastDisappearance > GameObjectDisappearanceInterval) {
            timeSinceLastDisappearance = 0f;
            GetRandomDynamicObject();
            CreatureSpawn();
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

    private void CreatureSpawn() {
        //If we have less than the maximum, we come back later
        if(Anomalies.Count <= MaxDivergences) {
            return;
        }
        int divergencesAboveMax = Anomalies.Count - MaxDivergences;
        int spawnChance = Random.Range(0,33*divergencesAboveMax);
        print("Spawn chance: " + spawnChance + "     Minimum #:" + (spawnChance > 20-divergencesAboveMax*2));
        string room = Rooms.ElementAt(Random.Range(0, Rooms.Count)).Key;
        while(room == SC_FPSController.Instance.GetPlayerRoom()) {
            room = Rooms.ElementAt(Random.Range(0, Rooms.Count)).Key;
        }
        if(spawnChance > 20-divergencesAboveMax*2) {
            if(Anomalies.Where(d => d.data.type.Equals(ANOMALY_TYPE.Creature)).ToList().Count >= creatureMax) {
                GameObject ender = Instantiate(endCreaturePrefab);
                ender.name = "Ender - " + room;
                ender.transform.position = GameObject.Find(room).transform.position;
                CreaturesPerRoom[room] += 1;
                ender.transform.SetParent(GameObject.Find(room).transform);
                CreateDivergence(ender);
            }
            if(CreaturesPerRoom[room] >= maxCreaturesPerRoom) {
                return;
            }
            //get a random room
            //put the guy in the room as a zombie
            GameObject zombie = Instantiate(zombiePrefab);
            zombie.name = "Zombie - " + room;
            zombie.transform.position = GameObject.Find(room).transform.position;
            CreaturesPerRoom[room] += 1;
            zombie.transform.SetParent(GameObject.Find(room).transform);
            CreateDivergence(zombie);
        }
    }

    public void ManuallySpawnCreature(string room) {
        GameObject zombie = Instantiate(zombiePrefab);
        zombie.name = "Zombie - " + room;
        zombie.transform.position = GameObject.Find(room).transform.position;
        CreaturesPerRoom[room] += 1;
        zombie.transform.SetParent(GameObject.Find(room).transform);
        CreateDivergence(zombie);
    }

    private void SoundControl() {
        if(Instance.GameOver || SC_FPSController.paused) {
        justPaused = true;
        foreach (var audioSource in FindObjectsOfType<AudioSource>()) {
            if (!audioSource.isPlaying) continue;
            audioSource.loop = true;
            pausedAudioSources[audioSource] = audioSource.time;
            audioSource.Pause();
        }
    }
    else if(justPaused) {
        justPaused = false;
        foreach (var audioSource in pausedAudioSources.Keys) {
            audioSource.loop = false;
            audioSource.time = pausedAudioSources[audioSource];
            audioSource.Play();
        }
        pausedAudioSources.Clear();
    }
    }

  void Update()
  {
    SoundControl();
    if(SC_FPSController.paused || GameOver) {
        return;
    }
    CheckTimer();
    SetGameTime();
    SetGuessTime();
    UpdateEnergy();
  }

}
