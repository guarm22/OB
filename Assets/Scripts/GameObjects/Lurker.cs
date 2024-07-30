using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Lurker : CreatureBase
{
    public AudioClip yellSound;
    [HideInInspector]
    private float timeSinceYell = -1f;
    private float yellCooldown = 5f;
    [HideInInspector]
    private Vector3 spawnPos;
    public Volume disorientPrefab;

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

    protected override void Awake() {
        spawnPos = transform.position;
        base.Awake();
    }

    // Update is called once per frame
    protected override void Update() {
        if(GameSystem.Instance.GameOver || PlayerUI.paused) {
            return;
        }

        base.CreatureSounds();
        if(canSeePlayer()) {
            runTowardsPlayer();
        }
        else {
            agent.SetDestination(spawnPos);
            base.FacePlayer();
        }
        if(amTouchingPlayer()) {
            Instantiate(disorientPrefab, player.transform.position, Quaternion.identity);
            SC_FPSController.Instance.Debuff("Energy", 10f);
            CreatureControl.Instance.RemoveCreature(gameObject);
        }
        timeSinceYell -= Time.deltaTime;
    }
}
