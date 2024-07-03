using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hider : CreatureBase {
    public float EnergyDrainAmount = 2f;
    private float energyDrainCooldown = 1f;
    private float timer = 0f;
    private float timeLookingAtPlayer = 0f;

    public AudioClip scareSound;

    // Start is called before the first frame update
    protected override void Start() {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update() {
        timer += Time.deltaTime;
        //if we can see the player, remove 1 energy per second
        if(canSeePlayer(25f)) {
            base.FacePlayer();
            timeLookingAtPlayer += Time.deltaTime;
            if(timer >= energyDrainCooldown) {
                SC_FPSController.Instance.Debuff("Energy", EnergyDrainAmount);
                timer = 0f;
            }
        }
        else {
            timeLookingAtPlayer = 0f;
        }

        if(timeLookingAtPlayer >= 6.5f) {
            AudioSource.PlayClipAtPoint(scareSound, transform.position);
            SC_FPSController.Instance.Debuff("Energy", EnergyDrainAmount*2.5f);
            timeLookingAtPlayer = 0f;
        }
    }
}
