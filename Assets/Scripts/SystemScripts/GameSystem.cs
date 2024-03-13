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
  public float divergenceRandomnessMax = 1.5f;
  public float divergenceRandomnessMin = -2f;
  private float currentRandomness;
  public AudioClip correctGuess;
  public AudioClip incorrectGuess;
  public bool madeGuess;
  public List<DynamicObject> lockedObjects;
  public static int totalDynamicObjectsInScene;
  public bool killedLights = false;
  private Dictionary<Light, Coroutine> flickeringLights = new Dictionary<Light, Coroutine>();

  public GameObject jumpscareZombie;
  public bool IsJumpscareFinished = false;
  public AudioClip yippie;

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
    madeGuess = false;
    lockedObjects = new List<DynamicObject>();
    this.gameObject.AddComponent<AudioSource>();
    this.gameObject.GetComponent<AudioSource>().clip = DisappearSound;
    generateNewRandomness();
    SetGameSettings();
    InstantiateAllDynamicObjects();

    //InvokeRepeating("GetRandomDynamicObject", GameStartTime, GameObjectDisappearanceInterval);
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
            GameObjectDisappearanceInterval = 21;
            MaxDivergences = 4;
            creatureMax = 3;
            energyPerSecond = 1.1f;
            GameStartTime = 15;
            break;
        default:
            GameObjectDisappearanceInterval = PlayerPrefs.GetInt("DivergenceRate", 21);
            MaxDivergences = PlayerPrefs.GetInt("MaxDivergences", 4);
            creatureMax = 3;
            energyPerSecond = PlayerPrefs.GetFloat("EPS", 1.1f);
            GameStartTime = PlayerPrefs.GetInt("GameStartTime", 15);
            break;
    }
  
  }

/// <summary>
/// Runs on game start, takes every object in the world with the dynamic data script and instantiates it as an object
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
        totalDynamicObjectsInScene = DynamicObjects.Count;
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
        if(data.type == ANOMALY_TYPE.Creature) {
            return;
        }
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
        if(TotalAnomalies >= Rooms.Count*AnomaliesPerRoom || areAllRoomsFull() || DynamicObjects.Count == 0) {
            if(UnityEngine.Random.Range(0, 100) < 50 && !killedLights) {
                KillLights(0.40f);
            } 
            return;
        }
        DynamicObject randomObject = null;
        int amt = -1;

        List<string> availableRooms = DynamicObjects
        .GroupBy(d => d.Room)
        .Where(g => HowManyDivergencesInRoom(g.Key) < AnomaliesPerRoom)
        .Select(g => g.Key)
        .ToList();

        if (availableRooms.Count > 0)
        {
            do {
                // Choose a random room from the available rooms
                string room = availableRooms[UnityEngine.Random.Range(0, availableRooms.Count)];

                // Choose a random object from the selected room
                randomObject = DynamicObjects
                    .Where(d => d.Room == room)
                    .OrderBy(d => Guid.NewGuid())
                    .FirstOrDefault();

                if (randomObject != null)
                {
                    Rooms.TryGetValue(randomObject.Room, out amt);
                }
            }
            while (randomObject == null || amt >= AnomaliesPerRoom);
        }
        if(randomObject.DoAnomalyAction(true) == 0) {
            return;
        }

        int lowerBound = TotalAnomalies < 3 ? 0 : TotalAnomalies == 3 ? 20 : TotalAnomalies > 3 ? 33 : 33;
        if(UnityEngine.Random.Range(0, 100) < lowerBound) {
            FlickerLights();
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

    public bool AnyAvailableDynamicObjectsInRoom(string room) {
        return DynamicObjects.Any(d => d.Room.Equals(room));
    }
    public int HowManyDivergencesInRoom(string room) {
        return Anomalies.Count(d => d.Room.Equals(room) && d.data.type != ANOMALY_TYPE.Creature);
    }

    /// <summary>
    /// Takes in a type and room, and sets the "PrivateCorrectGuess" variable true or false depending on whether there is an anomaly with that room and type
    /// </summary>
    /// <param name="type"></param>
    /// <param name="room"></param>
    public static void MakeSelection(List<string> types, string room) {
        List<DynamicObject> found = Anomalies
            .Where(dynam => types.Any(type => GetAnomalyTypeByName(type) == dynam.data.type && room == dynam.Room))
            .ToList();

        int totalCost = types.Count * Instance.EnergyPerGuess;
        foreach(DynamicObject d in found) {
             totalCost -= Instance.EnergyPerGuess - d.data.energyCost;
        }
        Debug.Log("Total cost: " + totalCost);

        if(Instance.CurrentEnergy < totalCost || room.Length<1) {
            Instance.StartCoroutine(Instance.guessFeedbackSound(false));
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
        PrivateCorrectGuess = CorrectObject.Count > 0;
    }

    /// <summary>
    /// Updates each frame with the time since the last guess
    /// </summary>
    void SetGuessTime() {
        if(Time.time - LastGuess >= GuessLockout/2 && madeGuess == true) {
            CorrectGuess = PrivateCorrectGuess;
            madeGuess = false;
            if(CorrectObject.Count > 0) {
                StartCoroutine(guessFeedbackSound(true));
                foreach(DynamicObject d in CorrectObject) {
                    this.AnomaliesSuccesfullyReportedThisGame += 1;
                    d.DoAnomalyAction(false);
                    Anomalies.Remove(d);

                    if(d.data.type == ANOMALY_TYPE.Creature) {
                        Destroy(d.Obj);
                        CreaturesPerRoom[d.Room] -= 1;
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
                CorrectObject = new List<DynamicObject>();
            }
            else {
                StartCoroutine(guessFeedbackSound(false));
            }
        }
        if(Time.time - LastGuess >= GuessLockout && Guessed == true) {
            Guessed = false;
            CorrectGuess = PrivateCorrectGuess;
        }
    }

    private IEnumerator guessFeedbackSound(bool correct) {
        GameObject player = GameObject.Find("Player");
        player.AddComponent<AudioSource>();
        AudioSource audioSource = player.GetComponent<AudioSource>();
        if(correct) {
            audioSource.clip = correctGuess;
        }
        else {
            audioSource.volume = 0.5f;
            audioSource.clip = incorrectGuess;
        }
        audioSource.Play();
        yield return new WaitForSeconds(audioSource.clip.length);
        Destroy(audioSource);
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
            yield return StartCoroutine(ZombieJumpscare());
        }
        IsJumpscareFinished =true;
        cleanUpGame();
    }
    private IEnumerator ZombieJumpscare() {
        GameObject player = GameObject.Find("Player");
        player.transform.position = jumpscareZombie.transform.position - new Vector3(0, -1, -2f);
        Vector3 direction = jumpscareZombie.transform.position - player.transform.position + new Vector3(0, 1, 0);
        Quaternion rotation = Quaternion.LookRotation(direction);
        player.transform.rotation = rotation;
        float originalLightIntensity = GameObject.Find("JumpscareLight").GetComponent<Light>().intensity;
        StartCoroutine(FlickerLight(GameObject.Find("JumpscareLight").GetComponent<Light>(), 2, 4, 1.5f));

        while(Vector3.Distance(player.transform.position, jumpscareZombie.transform.position) > 1.2f) {
            player.transform.position = Vector3.MoveTowards(player.transform.position, 
            jumpscareZombie.transform.position + new Vector3(0,1,0), 1.8f * Time.deltaTime);
            yield return null;
        }
        GameObject audioSourceObject = new GameObject("JumpscareAudioSource");
        AudioSource audioSource = audioSourceObject.AddComponent<AudioSource>();
        audioSource.clip = yippie;
        audioSource.transform.position = jumpscareZombie.transform.position;
        audioSource.Play();

        yield return new WaitForSeconds(1);
        GameObject.Find("JumpscareLight").GetComponent<Light>().intensity = originalLightIntensity;
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
        int spawnChance = UnityEngine.Random.Range(0,33*divergencesAboveMax);
        //print("Spawn chance: " + spawnChance + "     Minimum #:" + (spawnChance > 20-divergencesAboveMax*2));
        string room = Rooms.ElementAt(UnityEngine.Random.Range(0, Rooms.Count)).Key;

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

            Vector3 spawnPos = FindSpawnPoint(room);
            GameObject zombie = Instantiate(zombiePrefab);
            zombie.name = "Zombie - " + room;
            CreaturesPerRoom[room] += 1;
            GameObject roomObj = GameObject.Find(room);
            zombie.transform.SetParent(roomObj.transform);
            zombie.transform.position = spawnPos;

            BoxCollider roomCollider = roomObj.GetComponent<BoxCollider>();
            if (roomCollider.bounds.Contains(zombie.transform.position)) {
            }
            else {
                zombie.transform.position = roomObj.transform.position;
            }
            CreateDivergence(zombie);
        }
    }
    private Vector3 FindSpawnPoint(string room) {
            GameObject roomObj = GameObject.Find(room);
            BoxCollider roomCollider = roomObj.GetComponent<BoxCollider>();
            List<Vector3> roomPoints = new List<Vector3> {
                new Vector3(roomObj.transform.position.x + roomCollider.bounds.size.x / 2, roomObj.transform.position.y, roomObj.transform.position.z + roomCollider.bounds.size.z / 2), // Top right corner
                new Vector3(roomObj.transform.position.x - roomCollider.bounds.size.x / 2, roomObj.transform.position.y, roomObj.transform.position.z - roomCollider.bounds.size.z / 2), // Bottom left corner
                new Vector3(roomObj.transform.position.x + roomCollider.bounds.size.x / 2, roomObj.transform.position.y, roomObj.transform.position.z - roomCollider.bounds.size.z / 2), // Bottom right corner
                new Vector3(roomObj.transform.position.x - roomCollider.bounds.size.x / 2, roomObj.transform.position.y, roomObj.transform.position.z + roomCollider.bounds.size.z / 2), // Top left corner
                roomObj.transform.position // Center
            };

            Vector3 playerPosition = GameObject.Find("Player").transform.position;
            Vector3 furthestPoint = roomPoints.OrderByDescending(point => Vector3.Distance(playerPosition, point)).First();
            
            NavMeshHit hit;
            if (NavMesh.SamplePosition(furthestPoint, out hit, 5f, NavMesh.AllAreas)) {
                furthestPoint = hit.position;
            } else {
                // Handle case where no point could be found on the NavMesh
            }
            return furthestPoint;
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
                if (!audioSource.isPlaying || audioSource.gameObject.name.Equals("JumpscareAudioSource")) continue;
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

    IEnumerator FlickerLight(Light light, float minIntensity, float maxIntensity, float duration)
{
    float elapsedTime = 0f;
    while (elapsedTime < duration)
    {
        // Randomly change the light's intensity
        light.intensity = UnityEngine.Random.Range(minIntensity, maxIntensity);

        // Wait for a short amount of time
        yield return new WaitForSeconds(UnityEngine.Random.Range(0.05f, 0.1f));

        elapsedTime += Time.deltaTime;
    }
    flickeringLights.Remove(light);
    // Ensure the light's intensity is reset to its original state
    light.intensity = maxIntensity;
}

    public void FlickerLights()
    {
        foreach (GameObject lightObject in GameObject.FindGameObjectsWithTag("Light"))
        {
            Light light = lightObject.GetComponent<Light>();
            flickeringLights.Add(light, StartCoroutine(FlickerLight(light, 2f, light.intensity, .1f)));
        }
    }

    public void KillLights(float amt) {
        killedLights = true;
        int amtOfLightsInScene = GameObject.FindGameObjectsWithTag("Light").Length;
        int lightsToKill = (int)Math.Ceiling(amtOfLightsInScene * amt);
        for(int i = 0; i < lightsToKill; i++) {
            GameObject light = GameObject.FindGameObjectsWithTag("Light")[UnityEngine.Random.Range(0, amtOfLightsInScene-i)];
            light.tag = "Untagged";
            light.GetComponent<Light>().intensity = 2f;
            if(flickeringLights.ContainsKey(light.GetComponent<Light>()) && flickeringLights[light.GetComponent<Light>()] != null) {
                StopCoroutine(flickeringLights[light.GetComponent<Light>()]);
            }
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
