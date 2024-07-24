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
    public GameObject endCreaturePrefab;
    public GameObject chaserPrefab;
    public GameObject lurkerPrefab;
    public int maxCreaturesPerRoom = 1;
    public Dictionary<string, int> CreaturesPerRoom = new Dictionary<string, int>();
    public int creatureMax = 3;
    public GameObject jumpscareZombie;
    [HideInInspector]
    public bool IsJumpscareFinished = false;
    public AudioClip jumpscareSound;
    public float creatureSpawnRate = 20f;
    public float timeSinceLastCreature = 0f;
    private List<GameObject> specialCreatures = new List<GameObject>();
    public float zombieSpawnChance = 50f;
    public float specialSpawnChance = 50f;
    [HideInInspector]
    public int TotalCreatures;
    public int CreaturesReported = 0;

    [HideInInspector]
    public float enderSpawnChance = 5f;

    public List<GameObject> ActiveCreatures = new List<GameObject>();

    public List<GameObject> CreatureSpawnpoints = new List<GameObject>();


    public IEnumerator ZombieJumpscare() {
        GameObject player = GameObject.Find("Player");
        player.transform.position = jumpscareZombie.transform.position - new Vector3(0, -1, -2f);
        Vector3 direction = jumpscareZombie.transform.position - player.transform.position + new Vector3(0, 1, 0);
        Quaternion rotation = Quaternion.LookRotation(direction);
        player.transform.rotation = rotation;

        float originalLightIntensity = GameObject.Find("JumpscareLight").GetComponent<Light>().intensity;
        StartCoroutine(LightControl.Instance.FlickerLight(GameObject.Find("JumpscareLight").GetComponent<Light>(), 2, 4, 1.5f));

        while(Vector3.Distance(player.transform.position, jumpscareZombie.transform.position) > 1.2f) {
            player.transform.position = Vector3.MoveTowards(player.transform.position, 
            jumpscareZombie.transform.position + new Vector3(0,1,0), 1.8f * Time.deltaTime);
            yield return null;
        }
        GameObject audioSourceObject = new GameObject("JumpscareAudioSource");
        AudioSource audioSource = audioSourceObject.AddComponent<AudioSource>();
        audioSource.clip = jumpscareSound;
        audioSource.transform.position = jumpscareZombie.transform.position;
        audioSource.Play();

        yield return new WaitForSeconds(1);
        GameObject.Find("JumpscareLight").GetComponent<Light>().intensity = originalLightIntensity;
    }

    private void createCreature(GameObject prefab, string room, string type = "Zombie") {
        if(!CreaturesPerRoom.TryGetValue(room, out int v)) {
            CreaturesPerRoom.Add(room, 0);
        } 
        if(CreaturesPerRoom[room] >= maxCreaturesPerRoom) {
            return;
        }
        Vector3 spawnPos = FindSpawnPoint(room, type);
        GameObject roomObj = GameObject.Find(room);
        GameObject creature = Instantiate(prefab, spawnPos, Quaternion.identity);
        creature.GetComponent<NavMeshAgent>().Warp(spawnPos);
        creature.transform.SetParent(roomObj.transform);
        creature.name = type + " - " + room;
        CreaturesPerRoom[room] += 1;
        creature.GetComponent<CreatureBase>().HomeRoom = room;

        BoxCollider roomCollider = roomObj.GetComponent<BoxCollider>();
        if (roomCollider.bounds.Contains(creature.transform.position)) {
        }
        else {
            creature.transform.position = roomObj.transform.position;
        }
        TotalCreatures += 1;
        ActiveCreatures.Add(creature);
    }

      private Vector3 FindSpawnPoint(string room, string type) {
            if(type == "Lurker") {
                return FindLurkerSpawn(room);
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
            if (NavMesh.SamplePosition(point, out hit, 5f, NavMesh.AllAreas)) {
                point = hit.position;
            }
            return point;
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
        Vector3 spawnPos = GameObject.Find("LurkerSpawn"+room).transform.position;
        return spawnPos;
    }

    
    public void ManuallySpawnCreature(string room) {
        createCreature(chaserPrefab, room, "Chaser");
    }

    public void ManuallySpawnEnder(string room) {
        createCreature(endCreaturePrefab, room, "Ender");
    }

    private void doCreatureCheck() {
        //pick a random room
        string room = DivergenceControl.Instance.Rooms.ElementAt(UnityEngine.Random.Range(0, DivergenceControl.Instance.Rooms.Count)).Key;
        //make sure room exists in the list of rooms
        if(!CreaturesPerRoom.TryGetValue(room, out int v)) {
            CreaturesPerRoom.Add(room, 0);
        } 

        //check if the player is in that room
        if(PlayerUI.Instance.GetPlayerRoom() == room) {

        }

        //lose condition - all rooms have max anomalies
        ShouldSpawnEnder(room);

        if(DivergenceControl.Instance.DivergenceList.Count >= DivergenceControl.Instance.MaxDivergences) {
            int spawnChance = UnityEngine.Random.Range(0,100);
            if(spawnChance < zombieSpawnChance) {
                createCreature(zombiePrefab, room);
                return;
            }
        }
        if(DivergenceControl.Instance.DivergenceList.Count >= DivergenceControl.Instance.MaxDivergences/2) {
            int spawnChance = UnityEngine.Random.Range(0,100);
            int randomIndex = UnityEngine.Random.Range(0, specialCreatures.Count);
            if(spawnChance < specialSpawnChance) {
                createCreature(specialCreatures[randomIndex], room, specialCreatures[randomIndex].name);
            }
        }
    }

    private void ShouldSpawnEnder(string room) {
        if(GameSystem.InEditor()) {
            return;
        }

        if(DivergenceControl.Instance.MaxDivergences >= DivergenceControl.Instance.DivergencesPerRoom*DivergenceControl.Instance.Rooms.Count) {
            if(!GameSystem.InEditor()) {
                ManuallySpawnEnder(room);
            }
        }

        int divCount = DivergenceControl.Instance.DivergenceList.Count;
        int maxDivs = DivergenceControl.Instance.MaxDivergences;
        //get amount of divergences that have been alive for more than x seconds
        int x = DivergenceControl.Instance.DivergenceList.Count(div => Time.time - div.divTime > 90);
        int y = DivergenceControl.Instance.DivergenceList.Count(div => Time.time - div.divTime > 150);
        int z = DivergenceControl.Instance.DivergenceList.Count(div => Time.time - div.divTime > 200);
        int w = DivergenceControl.Instance.DivergenceList.Count(div => Time.time - div.divTime > 260);
        float spawnChance = enderSpawnChance + ((1.5f*x) + (3*y) + (4*z) + (5*w));

        //chance to spawn an ender within 80% of the max divergences
        if(divCount >= Mathf.Ceil(maxDivs*0.8f)) {
            int rnum = UnityEngine.Random.Range(0,100);
            if(rnum < spawnChance) {
                ManuallySpawnEnder(room);
                return;
            }
        }

        //half chance to spawn an ender within 70% of the max divergences
        if(divCount >= Mathf.Ceil(maxDivs*0.65f)) {
            int rnum = UnityEngine.Random.Range(0,100);
            if(rnum < spawnChance/2) {
                ManuallySpawnEnder(room);
                return;
            }
        }
    }

    public int CreatureReport(String room) {
        foreach(GameObject creature in ActiveCreatures) {
            if(creature.name.Contains(room) || creature.GetComponent<CreatureBase>().HomeRoom == room) {
                CreaturesReported += 1;
                GameSystem.Instance.ChangeEnergy(25 - creature.GetComponent<DynamicData>().energyCost);
                RemoveCreature(creature);
                break;
            }
        }
        return CreaturesReported;
    }
    
    public void RemoveCreature(GameObject creature) {
        string room = creature.name.Split('-')[1].Trim();
        ActiveCreatures.Remove(creature);
        CreaturesPerRoom[room] -= 1;
        Destroy(creature);
        TotalCreatures -= 1;
    }

    void Start() {
        Instance = this;

        if(SceneManager.GetActiveScene().name == "Cabin"  || SceneManager.GetActiveScene().name == "Apartment") {
            specialCreatures.Add(lurkerPrefab);
        }

        CreatureSpawnpoints = GameObject.FindGameObjectsWithTag("CreatureSpawnPoint").ToList();

        specialCreatures.Add(chaserPrefab);
        setCreatureSettings();
    }

    private void setCreatureSettings() {
        if(GameSystem.InEditor()) {
            return;
        }
        creatureSpawnRate = PlayerPrefs.GetFloat("CreatureSpawnRate", 20f);
        creatureMax = 4;
    }

    void Update() {
        if(GameSystem.Instance.GameOver || PlayerUI.paused) {
            return;
        }

        timeSinceLastCreature += Time.deltaTime;
        if(timeSinceLastCreature > creatureSpawnRate) {
            timeSinceLastCreature = 0;
            doCreatureCheck();
        }
    }
}
