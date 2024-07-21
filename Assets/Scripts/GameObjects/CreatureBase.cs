using System;
using UnityEngine;
using UnityEngine.AI;
public class CreatureBase : MonoBehaviour {
    [HideInInspector]
    public GameObject player;
    [HideInInspector]
    public float sightRange = 15;
    [HideInInspector]
    public NavMeshAgent agent;
    [SerializeField] public LayerMask playerLayer;
    [SerializeField] public LayerMask floorLayer;
    public float stuckTimer = 3f;
    [HideInInspector]
    public Vector3 posCheck;
    [HideInInspector]
    public float timeSinceLastStuckCheck;
    [HideInInspector]
    public bool playerInView = false;
    [HideInInspector]
    Vector3 dest;
    [HideInInspector]
    public bool isDestSet;
    [SerializeField] float walkRange;

    public AudioClip closeSound;
    public AudioClip farSound;
    public float soundTimerStart = 13.8f;
    public float soundTimerMax = 14f;
    public float creatureSpeed = 5f;
    public float closeSoundRange = 10f;
    [HideInInspector]
    public bool isPlayerSeen = false;

    public void CreaturePatrol() {
        CreatureSounds();
        //if the creature can see the player, chasing the player takes priority over everything else
        isPlayerSeen = canSeePlayer();
        
        Vector3 direction = dest - transform.position;
        Debug.DrawRay(transform.position, direction, Color.red);

        if(isPlayerSeen) {
            dest = player.transform.position;
            agent.SetDestination(player.transform.position);
            
            return;
        }

        //every 3 seconds it checks if the creature has been in the same spot, if so then choose another place to move
        if(stuck()) {
            isDestSet = false;
        }
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

    public bool amCloseToPlayer(float distance = 3.5f) {
        if(Vector3.Distance(transform.position, player.transform.position) < distance) {
            return true;
        }
        return false;
    }

    protected bool canSeePlayer(float lookRange = 15f) {
        RaycastHit hit;
        Vector3 direction = player.transform.position - transform.position;
        if (Physics.Raycast(transform.position, direction, out hit, lookRange, playerLayer)) {
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
    
    protected virtual void CreatureSounds() {
        soundTimerStart += Time.deltaTime;

        if(soundTimerStart >= soundTimerMax) {
            if(Vector3.Distance(player.transform.position, transform.position) < closeSoundRange) {
                AudioSource.PlayClipAtPoint(closeSound, transform.position);
            }
            else {
                AudioSource.PlayClipAtPoint(farSound, transform.position);
            }
            soundTimerStart = 0f;
        }
    }

    protected virtual void FacePlayer() {
        // Calculate the direction vector from the GameObject to the player
        Vector3 direction = player.transform.position - transform.position;
        // Set the GameObject's forward direction to this direction
        transform.forward = direction;
    }


    protected virtual void Awake() {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.Find("Player");
        //hacky fix to stop navmeshagent from getting stuck
        agent.Warp(transform.position);
        timeSinceLastStuckCheck = 0f;
        posCheck = transform.position;
        agent.speed = creatureSpeed;
        agent.angularSpeed=3000;
    }

    protected virtual void Update() {
        if(PlayerUI.paused || GameSystem.Instance.GameOver) {
            StopCreature();
            return;
        }
        timeSinceLastStuckCheck += Time.deltaTime;
        CreaturePatrol();
    }


    #if UNITY_EDITOR
    void OnDrawGizmos() {
        //Gizmos.DrawWireSphere(transform.position, radius);
    }
    #endif
}
