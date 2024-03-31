using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class CreatureControl : MonoBehaviour
{
    public static CreatureControl Instance;
    public GameObject zombiePrefab;
    public GameObject endCreaturePrefab;
    public GameObject chaserPrefab;
    public int maxCreaturesPerRoom = 1;
    public Dictionary<string, int> CreaturesPerRoom = new Dictionary<string, int>();
    public int creatureMax = 3;
    public GameObject jumpscareZombie;
    [HideInInspector]
    public bool IsJumpscareFinished = false;
    public AudioClip jumpscareSound;
    public float creatureSpawnRate = 20f;
    private float timeSinceLastCreature = 0f;
    private List<GameObject> specialCreatures = new List<GameObject>();
    public float zombieSpawnChance = 66.6667f;
    public float specialSpawnChance = 60f;
    [HideInInspector]
    public int TotalCreatures;


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
        GameObject creature = Instantiate(prefab);
        Vector3 spawnPos = FindSpawnPoint(room);
        GameObject roomObj = GameObject.Find(room);
        creature.transform.position = spawnPos;
        creature.transform.SetParent(roomObj.transform);
        creature.name = type + " - " + room;
        CreaturesPerRoom[room] += 1;

        BoxCollider roomCollider = roomObj.GetComponent<BoxCollider>();
        if (roomCollider.bounds.Contains(creature.transform.position)) {
        }
        else {
            creature.transform.position = roomObj.transform.position;
        }
        TotalCreatures += 1;
        GameSystem.Instance.CreateDivergence(creature);
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
        createCreature(zombiePrefab, room, "Zombie");
    }

    public void ManuallySpawnEnder(string room) {
        createCreature(endCreaturePrefab, room, "Ender");
    }

    private void doCreatureCheck() {
        string room = GameSystem.Instance.Rooms.ElementAt(UnityEngine.Random.Range(0, GameSystem.Instance.Rooms.Count)).Key;
        if(!CreaturesPerRoom.TryGetValue(room, out int v)) {
            CreaturesPerRoom.Add(room, 0);
        } 

        //lose condition - all rooms have max anomalies
        if(GameSystem.Instance.TotalAnomalies >= GameSystem.Instance.AnomaliesPerRoom*GameSystem.Instance.Rooms.Count) {
            if(!GameSystem.InEditor()) {
                ManuallySpawnEnder(GameSystem.Instance.getRandomRoom());
            }
        }

        if(GameSystem.Instance.TotalAnomalies >= GameSystem.Instance.MaxDivergences) {
            int spawnChance = UnityEngine.Random.Range(0,100);
            if(spawnChance < zombieSpawnChance) {
                createCreature(zombiePrefab, room);
                return;
            }
        }
        if(GameSystem.Instance.TotalAnomalies >= GameSystem.Instance.MaxDivergences/2) {
            int spawnChance = UnityEngine.Random.Range(0,100);
            int randomIndex = UnityEngine.Random.Range(0, specialCreatures.Count);
            if(spawnChance < specialSpawnChance) {
                createCreature(specialCreatures[randomIndex], room, specialCreatures[randomIndex].name);
            }
        }
    }

    public void RemoveCreature(GameObject creature) {
        string room = creature.name.Split('-')[1].Trim();
        CreaturesPerRoom[room] -= 1;
        Destroy(creature);
        TotalCreatures -= 1;
    }

    void Start() {
        Instance = this;
        specialCreatures.Add(chaserPrefab);
        setCreatureSettings();
    }

    private void setCreatureSettings() {
        if(GameSystem.InEditor()) {
            return;
        }
        creatureSpawnRate = PlayerPrefs.GetFloat("CreatureSpawnRate", 20f);
        creatureMax = 3;
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
