using System;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions.Must;

public class CreatureBase : MonoBehaviour
{
    public GameObject player;
    public float sightRange = 15;
    private NavMeshAgent agent;
    [SerializeField] public LayerMask playerLayer;
    [SerializeField] public LayerMask floorLayer;
    public float stuckTimer = 3f;
    public Vector3 posCheck;
    public float timeSinceLastStuckCheck;
    public bool playerInView = false;
    Vector3 dest;
    public bool isDestSet;
    [SerializeField] float walkRange;

    public AudioClip closeSound;
    public AudioClip farSound;
    private float soundTimer = 13.8f;
    public float soundTimerMax = 14f;

    private void CreaturePatrol() {
        soundTimer += Time.deltaTime;

        if(soundTimer >= soundTimerMax) {
            if(Vector3.Distance(player.transform.position, transform.position) < 10f) {
                AudioSource.PlayClipAtPoint(closeSound, transform.position);
            }
            else {
                AudioSource.PlayClipAtPoint(farSound, transform.position);
            }
            soundTimer = 0f;
        }

        //if the creature can see the player, chasing the player takes priority over everything else
        if(canSeePlayer()) {
            agent.SetDestination(player.transform.position);

            if(amTouchingPlayer()) {
                StartCoroutine(GameSystem.Instance.EndGame("zombie"));
            }
            return;
        }

        //every 3 seconds it checks if the creature has been in the same spot, if so then choose another place to move
        if(stuck()) {
            isDestSet = false;
        }

        Vector3 direction = dest - transform.position;
        // Perform the raycast
        RaycastHit hit;
        if (Physics.Raycast(transform.position, direction, out hit, 5f, floorLayer)) {
            isDestSet = false;
        }
        if(!isDestSet) {
            findDest();
        }
        if(isDestSet) {
            agent.SetDestination(dest);
        }
        if(Vector3.Distance(transform.position, dest) < 1) {
            isDestSet = false;
        }

    }

    private bool stuck() {
        if(timeSinceLastStuckCheck > stuckTimer) {
            timeSinceLastStuckCheck = 0f;
            if(Vector3.Distance(posCheck, transform.position) > 1f) {
                return true;
            }
            else {
                return false;
            }
        }
        else {
            return false;
        }
    }

    private bool canSeePlayer() {
        RaycastHit hit;
        Vector3 direction = player.transform.position - transform.position;
        if (Physics.Raycast(transform.position, direction, out hit, sightRange, playerLayer)) {
            //then check if there is a wall between
            if(Physics.Raycast(transform.position, direction, out hit, 
            Vector3.Distance(transform.position, player.transform.position), floorLayer)) {
                return false;
            }
            playerInView = true;
            return true;
        }
        return false;   
    }

    private bool amTouchingPlayer() {
        if(Vector3.Distance(transform.position, player.transform.position) < 1.3f) {
                return true;
            }
        return false;
    }

    void findDest() {
        float z = UnityEngine.Random.Range(-walkRange, walkRange);
        float x = UnityEngine.Random.Range(-walkRange, walkRange);

        dest = new Vector3(transform.position.x + x, transform.position.y, transform.position.z + z);

        if(Physics.Raycast(dest, Vector3.down, floorLayer)) {
            isDestSet = true;
        }
        NavMeshHit hit;
        if (!NavMesh.Raycast(transform.position, dest, out hit, NavMesh.AllAreas)) {
            // Destination is not reachable, set a new destination point
            Vector3 newDestination = hit.position;
            agent.SetDestination(newDestination);
        }
    }


    private void StopCreature() {
        agent.SetDestination(transform.position);
    }

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.Find("Player");
        //hacky fix to stop navmeshagent from getting stuck
        agent.Warp(transform.position);
        timeSinceLastStuckCheck = 0f;
        posCheck = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if(SC_FPSController.paused || GameSystem.Instance.GameOver) {
            StopCreature();
            return;
        }
        timeSinceLastStuckCheck += Time.deltaTime;
        CreaturePatrol();
    }


    #if UNITY_EDITOR
    void OnDrawGizmos()
    {
        //Gizmos.DrawWireSphere(transform.position, radius);
    }
    #endif
}
