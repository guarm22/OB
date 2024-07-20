using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie : CreatureBase {

    private bool amTouchingPlayer() {
        if(Vector3.Distance(transform.position, player.transform.position) < 1.3f) {
                return true;
            }
        return false;
    }

    protected override void Awake() {
        base.Awake();
    }

    // Update is called once per frame
    protected override void Update() {
        base.Update();
        if(amTouchingPlayer()) {
            StartCoroutine(GameSystem.Instance.EndGame("zombie"));
        }
    }
}
