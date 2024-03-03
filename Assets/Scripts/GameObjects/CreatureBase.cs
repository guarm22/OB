using System;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.AI;

public class CreatureBase : MonoBehaviour
{
    public GameObject player;
    public float sightRange = 15;
    private NavMeshAgent agent;
    [SerializeField] public LayerMask playerLayer;
    [SerializeField] public LayerMask floorLayer;
    private bool playerInSight;
    public float radius = 20;

    Vector3 dest;
    bool isDestSet;

    [SerializeField] float walkRange;

    private void CreaturePatrol() {
        if(!isDestSet) {
            findDest();
        }
        if(isDestSet) {
            agent.SetDestination(dest);
        }
        if(!agent.CalculatePath(dest, agent.path)) {
            
        }
        if(Vector3.Distance(transform.position, dest) < 15) {
            isDestSet = false;
        }

    }

    void findDest() {
        float z = UnityEngine.Random.Range(-walkRange, walkRange);
        float x = UnityEngine.Random.Range(-walkRange, walkRange);

        dest = new Vector3(transform.position.x + x, transform.position.y, transform.position.z + z);

        if(Physics.Raycast(dest, Vector3.down, floorLayer)) {
            isDestSet = true;
        }
        NavMeshHit hit;
        if (NavMesh.Raycast(transform.position, dest, out hit, NavMesh.AllAreas)) {
            // Destination is not reachable, set a new destination point
            Vector3 newDestination = hit.position;
            agent.SetDestination(newDestination);
        }
    }


    private void StopCreature() {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if(SC_FPSController.paused || GameSystem.Instance.GameOver) {
            StopCreature();
            return;
        }
        CreaturePatrol();
    }


    #if UNITY_EDITOR
    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }
    #endif
}
