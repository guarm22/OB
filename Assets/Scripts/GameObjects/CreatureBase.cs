using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;

public class CreatureBase : MonoBehaviour
{
    public GameObject player;
    public float sightRange = 15;
    private LayerMask playerLayer;

    private bool playerInSight;
    private NavMeshAgent agent;
    public float radius = 20;

    public Action reachedLocation;

    private void CreatureMove() {
        playerInSight = Physics.CheckSphere(transform.position, sightRange, playerLayer);

        if(playerInSight) {
            agent.SetDestination(player.transform.position);
        }

        else if(!agent.hasPath) {
            agent.SetDestination(GetPoint.Instance.GetRandomPoint(transform, radius));
        }
    }

    private void StopCreature() {
        agent.SetDestination(transform.position);
    }
    // Start is called before the first frame update
    void Start()
    {
        playerLayer = LayerMask.GetMask("Player");
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        if(SC_FPSController.paused || GameSystem.Instance.GameOver) {
            StopCreature();
            return;
        }
        CreatureMove();
    }


    #if UNITY_EDITOR
    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }
    #endif
}
