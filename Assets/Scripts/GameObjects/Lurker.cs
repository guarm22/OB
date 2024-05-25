using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lurker : CreatureBase
{
    public AudioClip yellSound;
    private float timeSinceYell = -1f;
    private float yellCooldown = 5f;
    private Vector3 spawnPos;

    public void runTowardsPlayer() {
        agent.SetDestination(player.transform.position);

        if(timeSinceYell < 0) {
            AudioSource.PlayClipAtPoint(yellSound, transform.position);
            timeSinceYell = yellCooldown;
        }
    }

    private bool amTouchingPlayer() {
        if(Vector3.Distance(transform.position, player.transform.position) < 1.3f) {
                return true;
            }
        return false;
    }

    protected override void Start() {
        spawnPos = transform.position;
        base.Start();
    }

    // Update is called once per frame
    protected override void Update() {
        base.CreatureSounds();
        if(canSeePlayer()) {
            runTowardsPlayer();
        }
        else {
            agent.SetDestination(spawnPos);
            base.FacePlayer();
        }
        if(amTouchingPlayer()) {
            SC_FPSController.Instance.Debuff("Energy", 10f);
            CreatureControl.Instance.RemoveCreature(gameObject);
        }
        timeSinceYell -= Time.deltaTime;
    }
}
