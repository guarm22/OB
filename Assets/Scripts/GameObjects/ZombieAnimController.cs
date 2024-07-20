using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ZombieAnimController : MonoBehaviour
{
    private Animator animator;
    private CreatureBase creature;

    void Start() {
        animator = GetComponent<Animator>();  
        creature = GetComponent<CreatureBase>(); 
    }

    void Update() {
        if(PlayerUI.paused) {
            return;
        }
        animator.SetBool("isDestinationSet", creature.isDestSet);
        animator.SetBool("isPlayerSeen", creature.isPlayerSeen);
        animator.SetBool("isPlayerInRange", creature.amCloseToPlayer());

    }
}
