using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;

public enum CREATURE_TYPE {
    NONE,
    DEFAULT,
}

public class CreatureBase : MonoBehaviour
{
    public Vector3 roomMax;
    public Vector3 roomMin;

    public float movementSpeed;
    private Vector3 movementLocation;
    public GameObject player;
    public float sightRange;
    public LayerMask playerLayer;

    private bool playerInSight;

    private NavMeshAgent agent;

    public CREATURE_TYPE type;

    public float radius;

    public Action reachedLocation;
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        playerInSight = Physics.CheckSphere(transform.position, sightRange, playerLayer);

        if(playerInSight) {
            agent.SetDestination(player.transform.position);
        }

        else if(!agent.hasPath) {
            agent.SetDestination(GetPoint.Instance.GetRandomPoint(transform, radius));
        }
    }


    #if UNITY_EDITOR
    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }
    #endif
}
