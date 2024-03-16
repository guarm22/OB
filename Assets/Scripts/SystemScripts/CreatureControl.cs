using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class CreatureControl : MonoBehaviour
{
    public static CreatureControl Instance;
    public GameObject zombiePrefab;
    public GameObject endCreaturePrefab;
    public int maxCreaturesPerRoom = 1;
    public Dictionary<string, int> CreaturesPerRoom = new Dictionary<string, int>();
    public int creatureMax;
    public GameObject jumpscareZombie;
    public bool IsJumpscareFinished = false;
    public AudioClip yippie;


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
        audioSource.clip = yippie;
        audioSource.transform.position = jumpscareZombie.transform.position;
        audioSource.Play();

        yield return new WaitForSeconds(1);
        GameObject.Find("JumpscareLight").GetComponent<Light>().intensity = originalLightIntensity;
    }

    public void CreatureSpawn() {
        //If we have less than the maximum, we come back later
        if(GameSystem.Instance.Anomalies.Count <= GameSystem.Instance.MaxDivergences) {
            return;
        }
        int divergencesAboveMax = GameSystem.Instance.Anomalies.Count - GameSystem.Instance.MaxDivergences;
        int spawnChance = UnityEngine.Random.Range(0,33*divergencesAboveMax);
        //print("Spawn chance: " + spawnChance + "     Minimum #:" + (spawnChance > 20-divergencesAboveMax*2));
        string room = GameSystem.Instance.Rooms.ElementAt(UnityEngine.Random.Range(0, GameSystem.Instance.Rooms.Count)).Key;

        if(!CreaturesPerRoom.TryGetValue(room, out int v)) {
            CreaturesPerRoom.Add(room, 0);
        } 
        if(spawnChance > 15-divergencesAboveMax*2) {
            if(GameSystem.Instance.Anomalies.Where(d => d.data.type.Equals(ANOMALY_TYPE.Creature)).ToList().Count >= creatureMax) {
                GameObject ender = Instantiate(endCreaturePrefab);
                createCreature(ender, room, "Ender");
            }
            if(CreaturesPerRoom[room] >= maxCreaturesPerRoom) {
                return;
            }
            GameObject zombie = Instantiate(zombiePrefab);
            createCreature(zombie, room, "Zombie");
        }
    }

    private void createCreature(GameObject creature, string room, string type = "Zombie") {
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
        GameObject zombie = Instantiate(zombiePrefab);
        createCreature(zombie, room, "Zombie");
    }

    void Start() {
        Instance = this;   
    }

    void Update() {
        
    }
}
