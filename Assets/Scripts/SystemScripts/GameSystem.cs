using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
public class GameSystem : MonoBehaviour, IDataPersistence
{
  public int GuessLockout;
  public static GameSystem Instance { get; private set; }
  public static List<DynamicObject> Anomalies {get; private set;}
  public static List<DynamicObject> DynamicObjects {get; private set;}
  public GameObject zombiePrefab;
  public static float LastGuess {get; private set;}
  public static bool Guessed {get; private set;}
  public static bool PrivateCorrectGuess {get; private set;}
  public static bool CorrectGuess {get; set;}
  public static DynamicObject CorrectObject {get; private set;}
  public AudioClip DisappearSound;
  public int GameStartTime;
  public int GameObjectDisappearanceInterval;
  public int AnomaliesPerRoom;

  public int TotalAnomalies;

  public int MaxEnergy = 100;
  public int CurrentEnergy;
  private float LastEnergyCheck=0f;
  private float EnergyCheckInterval=1f;

  public int MaxDivergences = 4;

  public int EnergyPerGuess = 25;

  public static Dictionary<string, int> Rooms;

  public float gameTime;
  public Text gameTimer;
  public float startTime = 60*15f;

  public float timeSinceLastDisappearance;

  private int AnomaliesSuccesfullyReported = 0;
  public int AnomaliesSuccesfullyReportedThisGame;

  public bool GameOver = false;

  public bool Won = false;
  
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
    Guessed = false;
    LastGuess = 0;
    PrivateCorrectGuess = false;
    CorrectObject = null;
    timeSinceLastDisappearance = 0f - GameStartTime;
    AnomaliesSuccesfullyReportedThisGame = 0;
    CurrentEnergy = MaxEnergy;
    TotalAnomalies = 0;

    InstantiateAllDynamicObjects();

    //InvokeRepeating("GetRandomDynamicObject", GameStartTime, GameObjectDisappearanceInterval);
  }

/// <summary>
/// Runs on game start, takes every object in the world with the dynamic tag and instantiates it as an object
/// 
/// </summary>
  static void InstantiateAllDynamicObjects() {
    DynamicData[] objects = GameObject.FindObjectsOfType<DynamicData>();
    foreach(DynamicData obj in objects) {
        DynamicData data = obj.gameObject.GetComponent<DynamicData>();

        GameObject gameobj = obj.transform.gameObject; 

        CustomDivergence cd = obj.gameObject.GetComponent<CustomDivergence>();
        data.customDivergence = cd;

        string room = getRoomName(obj.transform);

        DynamicObject dynam = new DynamicObject(
            data,
            room,
            obj.name,
            gameobj
        );
        DynamicObjects.Add(dynam);

        if(dynam.data.type == ANOMALY_TYPE.Creature) {
            dynam.Obj.SetActive(false);
        }
    }
    
    foreach(DynamicObject obj in DynamicObjects) {
        if(!Rooms.Keys.Contains<string>(obj.Room)) {
            Rooms.Add(obj.Room, 0);
        }
    }
  }

    private static string getRoomName(Transform obj) {
        if(obj.parent.tag != "Room") {
            while(true) {
                if(obj.parent == null) {
                    return "";
                }
                else if(obj.parent.tag == "Room") {
                    return obj.parent.name;
                }
                else {
                    obj = obj.parent;
                }
            }
        }
        return obj.parent.name;
    }

  private static ANOMALY_TYPE GetAnomalyTypeByName(string name) {
    switch(name){
        case "Light":
            return ANOMALY_TYPE.Light;
        case "ObjectDisappearance":
            return ANOMALY_TYPE.ObjectDisappearance;
        case "ObjectChange":
            return ANOMALY_TYPE.ObjectChange;
        case "Creature":
            return ANOMALY_TYPE.Creature;
        default:
            return ANOMALY_TYPE.NONE;
    }
  }

    public void GetRandomDynamicObject() {
        if(areAllRoomsFull() || DynamicObjects.Count == 0) {
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
        if(amt == AnomaliesPerRoom || AnyAvailableDynamicObjectsInRoom(randomObject.Room)) {
            //Debug.Log("Already an anomaly in " + obj.Obj.transform.parent.name);
            GetRandomDynamicObject();
            return;
        }
        else {
            Rooms[randomObject.Room] += 1;
        }
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
        if(Instance.CurrentEnergy < Instance.EnergyPerGuess * types.Count) {
            return;
        }
        DynamicObject found = null;
        foreach(DynamicObject dynam in Anomalies) {
            foreach(string type in types) {
                if(GetAnomalyTypeByName(type)==dynam.data.type && room == dynam.Room) {
                    found = dynam;
                }
            }
        }
        Instance.CurrentEnergy = Instance.CurrentEnergy-(Instance.EnergyPerGuess * types.Count);

        //Creatures cost less energy to report
        if(found.data.type == ANOMALY_TYPE.Creature) {
            Instance.CurrentEnergy = Instance.CurrentEnergy + Instance.EnergyPerGuess-18;
        }

        Guessed = true;
        LastGuess = Time.time;
        if(found == null) {
            //incorrect guess
            PrivateCorrectGuess = false;
            return;
        }
        else {
            //remove from list of invisible objects and return to initial position
            CorrectObject = found;
            PrivateCorrectGuess = true;
        }
    }

    /// <summary>
    /// Updates each frame with the time since the last guess
    /// </summary>
    void SetGuessTime() {
        if(Time.time - LastGuess >= GuessLockout-5 && Guessed == true) {
            CorrectGuess = PrivateCorrectGuess;
            if(CorrectObject != null) {
                //Stops multiple audio sources from going on a single gameobject
                Destroy(CorrectObject.Obj.GetComponent<AudioSource>());
                //counter
                this.AnomaliesSuccesfullyReportedThisGame += 1;
                //there is 1 less divergence in the reported room
                Rooms[CorrectObject.Room] -= 1;
                //undo divergence
                CorrectObject.DoAnomalyAction(false);
                Anomalies.Remove(CorrectObject);
                DynamicObjects.Add(CorrectObject);
                CorrectObject = null;
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
        List<DynamicObject> res = new List<DynamicObject>();
        foreach(DynamicObject d in Anomalies) {
            if(d.Room.Equals(room)) {
                res.Add(d);
            }
        }
        return res;
    }

    private List<DynamicObject> getDynamicObjectsByRoom(string room) {
        List<DynamicObject> res = new List<DynamicObject>();
        foreach(DynamicObject d in DynamicObjects) {
            if(d.Room.Equals(room)) {
                res.Add(d);
            }
        }
        return res;
    }

    public void SetGameTime() {
        if (Input.GetKeyDown(KeyCode.N)) {
            GameSystem.Instance.EndGame();
        }

        if (gameTime > 0)
        {
            string zero = "";
            if(Mathf.FloorToInt(gameTime % 60) < 10) {
                zero = "0";
            }

            gameTime -= Time.deltaTime;
            gameTimer.text = "" + Mathf.FloorToInt(gameTime/60) + ":" + zero + Mathf.FloorToInt(gameTime % 60);
        }
        else {
            Won = true;
            GameSystem.Instance.EndGame();
        }
    }

    public void EndGame() {
        GameOver = true;
        Debug.Log("Game over!");
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
                CurrentEnergy = CurrentEnergy + 1;
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
        if(spawnChance > 20-divergencesAboveMax*2) {
            //get a random room
            string room = Rooms.ElementAt(Random.Range(0, Rooms.Count)).Key;
            //put the guy in the room as a zombie
            GameObject zombie = Instantiate(zombiePrefab, GameObject.Find(room).transform);
            Debug.Log("Spawning creature in " + room);
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
