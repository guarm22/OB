using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreepAnimController : MonoBehaviour
{
    public Animator anim;
    public Chaser chaser;
    
    void Start() {
        anim = GetComponentInChildren<Animator>();
        chaser = GetComponent<Chaser>();
    }

    void Update() {
        //if paused, freeze animation
        if(PlayerUI.paused) {
            anim.enabled = false;
            return;
        }
        else {
            anim.enabled = true;
        }

        if(anim.isActiveAndEnabled) {
            if(chaser.isPlayerSeen) {
                anim.SetBool("playerSeen", true);
            }
            else {
                anim.SetBool("playerSeen", false);
            }
        }
    }
}
