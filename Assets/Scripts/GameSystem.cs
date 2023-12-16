using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
public class GameSystem : MonoBehaviour
{
  public int GuessLockout;
  public static GameSystem Instance { get; private set; }
  public static List<DynamicObject> Anomalies {get; private set;}
  public static List<DynamicObject> DynamicObjects {get; private set;}
  public static float LastGuess {get; private set;}
  public static bool Guessed {get; private set;}
  public static bool PrivateCorrectGuess {get; private set;}
  public static bool CorrectGuess {get; set;}
  public static DynamicObject CorrectObject {get; private set;}
  public AudioClip DisappearSound;
  public int GameStartTime;
  public int GameObjectDisappearanceInterval;
  public int AnomaliesPerRoom;

  public static Dictionary<string, int> Rooms;


  void Start()
  {
    if (Instance != null) {
      Debug.LogError("There is more than one instance!");
      return;
    }

    Instance = this;
    Anomalies = new List<DynamicObject>();
    DynamicObjects = new List<DynamicObject>();
    Rooms = new Dictionary<string, int>();
    Guessed = false;
    LastGuess = 0;
    PrivateCorrectGuess = false;
    CorrectObject = null;

    InstantiateAllDynamicObjects();

    InvokeRepeating("GetRandomDynamicObject", GameStartTime, GameObjectDisappearanceInterval);
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
            DynamicObject dynam = new DynamicObject
            {
                Type = data.type,

                Room = obj.transform.parent.name,
                Name = obj.name,
                Obj = gameobj,
                normal = true
            };
            DynamicObjects.Add(dynam);

        if(dynam.Type == ANOMALY_TYPE.Creature) {
            dynam.Obj.SetActive(false);
        }
    }
    
    foreach(DynamicObject obj in DynamicObjects) {
        if(!Rooms.Keys.Contains<string>(obj.Room)) {
            Rooms.Add(obj.Room, 0);
        }
    }
  }
  private static ANOMALY_TYPE GetAnomalyTypeByName(string name) {
    switch(name){
        case "Light":
            return ANOMALY_TYPE.Light;
        case "ObjectDisappearance":
            return ANOMALY_TYPE.ObjectDisappearance;
        case "ObjectMovement":
            return ANOMALY_TYPE.ObjectMovement;
        case "ObjectChange":
            return ANOMALY_TYPE.ObjectChange;
        case "Creature":
            return ANOMALY_TYPE.Creature;
        default:
            return ANOMALY_TYPE.NONE;
    }
  }

    public void GetRandomDynamicObject() {
        if(areAllRoomsFull()) {
            Application.Quit();
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
        //Debug.Log("count for " + randomObject.Room + "  -  " + amt);
        if(amt == AnomaliesPerRoom) {
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
    }

    /// <summary>
    /// Takes in a type and room, and sets the "guessed" variable true or false depending on whether there is an anomaly with that room and type
    /// </summary>
    /// <param name="type"></param>
    /// <param name="room"></param>
    public static void MakeSelection(string type, string room) {
        DynamicObject found = null;
        foreach(DynamicObject dynam in Anomalies) {
            if(GetAnomalyTypeByName(type)==dynam.Type && room == dynam.Room) {
                found = dynam;
            }
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
            if(Rooms[str] == AnomaliesPerRoom || isRoomAllAnomaliesActive(str)) {
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
        if(alen == AnomaliesPerRoom) {

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

    public void EndGame() {
        Debug.Log("Game over!");
    }

  void Update()
  {
    SetGuessTime();
  }

}
