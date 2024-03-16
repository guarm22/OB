using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chaser : CreatureBase
{
    private bool amTouchingPlayer() {
        if(Vector3.Distance(transform.position, player.transform.position) < 1.3f) {
                return true;
            }
        return false;
    }
    protected override void Start() {
        base.Start();

    }

    // Update is called once per frame
    protected override void Update() {
        base.Update();
        if(amTouchingPlayer()) {
            SC_FPSController.Instance.Debuff("Slow");
            CreatureControl.Instance.RemoveCreature(gameObject);
        }
    }
}
