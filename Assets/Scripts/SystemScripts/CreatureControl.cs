using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class CreatureControl : MonoBehaviour
{
    public static CreatureControl Instance;
    public GameObject zombiePrefab;
    public GameObject chaserPrefab;
    public GameObject lurkerPrefab;
    public GameObject hiderPrefab;
    public int divergenceThreshold = 2;

    public Dictionary<string, int> CreaturesPerRoom = new Dictionary<string, int>();
    [HideInInspector]
    public bool IsJumpscareFinished = false;
    public float creatureSpawnRate = 20f;
    public float timeSinceLastCreature = 0f;
    private List<GameObject> creatures = new List<GameObject>();
    public float specialSpawnChance = 50f;
    [HideInInspector]
    public int TotalCreatures;
    public int CreaturesReported = 0;

    public List<GameObject> ActiveCreatures = new List<GameObject>();

    public List<GameObject> CreatureSpawnpoints = new List<GameObject>();

    public bool ActivateZombie = true;
    public bool ActivateLurker = true;
    public bool ActivateHider = true;
    public bool ActivateChaser = true;

    private float CollapseChance = 60f;

    private List<string> GetRoomsWithNoCreatures() {
        return CreaturesPerRoom.Keys.Where(k => CreaturesPerRoom[k] == 0).ToList();
    }

    private void createCreature(GameObject prefab, string type = "Zombie", string roomPref = "") {
        if(GetRoomsWithNoCreatures().Count == 0) {
            return;
        }
        int randomVal = UnityEngine.Random.Range(0, GetRoomsWithNoCreatures().Count);
        string room = GetRoomsWithNoCreatures()[randomVal];
        if(roomPref != "") {
            room = roomPref;
        }

        Vector3 spawnPos = FindSpawnPoint(room, type);
        //special case for if no spawn point is found for hider or lurker
        //in this case, a zombie should spawn in its place using a recursive function call
        if(spawnPos == new Vector3(-1,-1,-1)){ return; }

        GameObject roomObj = GameObject.Find(room);
        GameObject creature = Instantiate(prefab, spawnPos, Quaternion.identity, roomObj.transform);
        Debug.Log("Spawning " + type + " in " + room + " at " + spawnPos);

        if(type != "Hider") {
            //creature.GetComponent<NavMeshAgent>().Warp(spawnPos);
        }
        creature.transform.SetParent(roomObj.transform);
        creature.name = type + " - " + room;
        CreaturesPerRoom[room] += 1;
        creature.GetComponent<CreatureBase>().HomeRoom = room;
        creature.GetComponent<CreatureBase>().CurrentRoom = room;

        TotalCreatures += 1;
        ActiveCreatures.Add(creature);
    }

      private Vector3 FindSpawnPoint(string room, string type) {
            if(type == "Lurker") {
                return FindLurkerSpawn(room);
            }

            if(type == "Hider") {
                return FindHiderSpawn(room);
            }

            GameObject roomObj = GameObject.Find(room);
            
            List<GameObject> possibleSpawns = new List<GameObject>();
            foreach(GameObject spawnpoint in CreatureSpawnpoints) {
                if(spawnpoint.name.Contains(room)) {                   
                    possibleSpawns.Add(spawnpoint);
                }
            }
            //if no predetermined spawns were found, get one based on room bounds
            if(possibleSpawns.Count == 0) {
                return GetRoomCornerFurthestFromPlayer(roomObj);
            }

            Vector3 point = new Vector3();
            //choose furthest point from player from possible points
            point = possibleSpawns.OrderByDescending(spawn => Vector3.Distance(GameObject.Find("Player").transform.position, spawn.transform.position)).First().transform.position;
            //return the closest point of the navmesh to that point
            NavMeshHit hit;
            if (NavMesh.SamplePosition(point, out hit, 1f, NavMesh.AllAreas)) {
                point = hit.position;
            }
            return point;
    }

    public Vector3 FindHiderSpawn(String room) {
        if(!GameObject.Find("HiderSpawn"+room)) {
            createCreature(zombiePrefab, "Zombie", room);
            return new Vector3(-1, -1, -1);
        }
        Vector3 spawnPos = GameObject.Find("HiderSpawn"+room).transform.position;
        return spawnPos;
    }

    private Vector3 GetRoomCornerFurthestFromPlayer(GameObject roomObj) {
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
            //handle case where no point could be found on the NavMesh
            
            //find navmesh of room
            NavMeshTriangulation navMeshData = NavMesh.CalculateTriangulation();

            //find center of navmesh
            Vector3 center = Vector3.zero;
            for (int i = 0; i < navMeshData.vertices.Length; i++) {
                center += navMeshData.vertices[i];
            }
            center /= navMeshData.vertices.Length;
            furthestPoint = center;    
        }
        return furthestPoint;
    }

    private Vector3 FindLurkerSpawn(string room) {
        if(!GameObject.Find("LurkerSpawn"+room)) {
            createCreature(zombiePrefab, "Zombie", room);
        }

        Vector3 spawnPos = GameObject.Find("LurkerSpawn"+room).transform.position;
        return spawnPos;
    }

    public void ManuallySpawnCreature(string room) {
        createCreature(chaserPrefab, "Chaser", room);
    }

    private void doCreatureCheck() {
        //lose condition - all rooms have max anomalies
        ShouldStartCollapse();

        if(DivergenceControl.Instance.DivergenceList.Count >= divergenceThreshold) {
            int rng = UnityEngine.Random.Range(0,100);
            int randomIndex = UnityEngine.Random.Range(0, creatures.Count);

            bool coEnabled = PlayerPrefs.GetInt("Creature Overrun",0) == 1;
            bool onTutorial = SceneManager.GetActiveScene().name == "Tutorial";

            if(rng < specialSpawnChance || (coEnabled && !onTutorial))  {
                createCreature(creatures[randomIndex],  creatures[randomIndex].name);
            }
        }
    }

    private void ShouldStartCollapse() {
        if(GameSystem.InEditor()) {
            //return;
        }

        if(SceneManager.GetActiveScene().name == "Tutorial") {
            return;
        }

        //if all rooms have a divergence
        if(DivergenceControl.Instance.areAllRoomsFull()) {
            if(UnityEngine.Random.Range(0,100) > CollapseChance) {
                CollapseChance += 12;
                return;
            }
            StartCoroutine(PunctureCollapse.Instance.Collapse());
        }
        else {
            if(CollapseChance > 60) {
                CollapseChance -= 6;
            }
        }

        int divCount = DivergenceControl.Instance.DivergenceList.Count;
        int maxDivs = DivergenceControl.Instance.MaxDivergences;
        
        //this code determines if the endgame collapse should start early or not
        //based on if there are any divergences that have been active for a long time
        int t1 = 75;
        int t2 = 150;
        int t3 = 200;
        int t4 = 240;
        int t5 = 360;
        int x = DivergenceControl.Instance.DivergenceList.Count(div => Time.time - div.divTime > t1 && Time.time - div.divTime <= t2);
        int y = DivergenceControl.Instance.DivergenceList.Count(div => Time.time - div.divTime > t2 && Time.time - div.divTime <= t3);
        int z = DivergenceControl.Instance.DivergenceList.Count(div => Time.time - div.divTime > t3 && Time.time - div.divTime <= t4);
        int w = DivergenceControl.Instance.DivergenceList.Count(div => Time.time - div.divTime > t4 && Time.time - div.divTime <= t5);
        int v = DivergenceControl.Instance.DivergenceList.Count(div => Time.time - div.divTime > t5);
        float spawnChance = (0.2f*x) + (0.35f*y) + (0.5f*z) + (1f*w) + (3.33f*v);

        //chance to start collapse within 80% of the max divergences
        if(divCount >= Mathf.Ceil(maxDivs*0.8f)) {
            int rnum = UnityEngine.Random.Range(0,100);
            if(rnum < spawnChance) {
                StartCoroutine(PunctureCollapse.Instance.Collapse());
                return;
            }
        }

        if(divCount >= Mathf.Ceil(maxDivs*0.65f)) {
            int rnum = UnityEngine.Random.Range(0,100);
            if(rnum < spawnChance/2) {
                StartCoroutine(PunctureCollapse.Instance.Collapse());
                return;
            }
        }
    }

    public int CreatureReport(String room) {
        foreach(GameObject creature in ActiveCreatures) {
            if(creature.GetComponent<CreatureBase>().CurrentRoom == room) {
                CreaturesReported += 1;
                GameSystem.Instance.ChangeEnergy(25 - creature.GetComponent<DynamicData>().energyCost);
                RemoveCreature(creature);
                break;
            }
        }
        return CreaturesReported;
    }
    
    public void RemoveCreature(GameObject creature) {
        if(creature.GetComponent<AudioSource>() != null) {
            creature.GetComponent<AudioSource>().Stop();
        }
        string room = creature.name.Split('-')[1].Trim();
        ActiveCreatures.Remove(creature);
        CreaturesPerRoom[room] -= 1;
        Destroy(creature);
        TotalCreatures -= 1;
    }

    void Start() {
        Instance = this;

        if(ActivateLurker) {
            creatures.Add(lurkerPrefab);
        }
        if(ActivateHider) {
            creatures.Add(hiderPrefab);
        }

        if(ActivateZombie) {
            creatures.Add(zombiePrefab);
        }
        CreatureSpawnpoints = GameObject.FindGameObjectsWithTag("CreatureSpawnPoint").ToList();

        if(ActivateChaser) {
            creatures.Add(chaserPrefab);
        }
        setCreatureSettings();

        //create rooms
        List<GameObject> rooms = GameObject.FindGameObjectsWithTag("Room").ToList<GameObject>();
        foreach(GameObject room in rooms) {
            CreaturesPerRoom.Add(room.name, 0);
        }
    }

    private void setCreatureSettings() {
        if(GameSystem.InEditor()) {
            return;
        }
        creatureSpawnRate = PlayerPrefs.GetFloat("CreatureSpawnRate", 20f);
    }

    void Update() {
        if(GameSystem.Instance.GameOver || PlayerUI.paused) {
            return;
        }

        if(GameSystem.InEditor() && Input.GetKeyDown(KeyCode.Delete)) {
            StartCoroutine(PunctureCollapse.Instance.Collapse());
        }

        timeSinceLastCreature += Time.deltaTime;
        if(timeSinceLastCreature > creatureSpawnRate) {
            timeSinceLastCreature = 0;
            doCreatureCheck();
        }
    }
}
