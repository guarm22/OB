using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
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

  public static List<string> Rooms;


  void Start()
  {
    if (Instance != null) {
      Debug.LogError("There is more than one instance!");
      return;
    }

    Instance = this;
    Anomalies = new List<DynamicObject>();
    DynamicObjects = new List<DynamicObject>();
    Rooms = new List<string>();
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
    GameObject[] objects = GameObject.FindGameObjectsWithTag("Dynamic");
    foreach(GameObject obj in objects) {
        DynamicData data = obj.gameObject.GetComponent<DynamicData>();
            DynamicObject dynam = new DynamicObject
            {
                Type = data.type,

                Room = obj.transform.parent.name,
                Name = obj.name,
                Obj = obj,
                normal = true
            };
            DynamicObjects.Add(dynam);

        if(dynam.Type == ANOMALY_TYPE.Creature) {
            dynam.Obj.SetActive(false);
        }
    }
    
    foreach(DynamicObject obj in DynamicObjects) {
        if(!Rooms.Contains(obj.Room)) {
            Rooms.Add(obj.Room);
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
        if(Anomalies.Count*AnomaliesPerRoom == Rooms.Count) {
            //Each room has an Anomaly
            return;
        }

        //Find all dynamic objects
        int index = UnityEngine.Random.Range(0, DynamicObjects.Count);
        // Select the object at the random index
        DynamicObject randomObject = DynamicObjects[index];

        foreach(DynamicObject obj in Anomalies) {
            //we dont want to move 2 objects from the same room
            if(obj.Room.Equals(randomObject.Room)) {
                //Debug.Log("Already an anomaly in " + obj.Obj.transform.parent.name);
                GetRandomDynamicObject();
                return;
            }
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

    public void EndGame() {
        Debug.Log("Game over!");
    }

  void Update()
  {
    SetGuessTime();
  }

}
