using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hider : CreatureBase {
    public float EnergyDrainAmount = 2f;
    private float energyDrainCooldown = 1f;
    private float timer = 0f;
    private float timeLookingAtPlayer = 0f;

    public AudioSource ads;

    public AudioClip scareSound;
    public AudioClip firstLookSound;

    private bool firstLook = false;

    // Start is called before the first frame update
    protected override void Awake() {
        player = GameObject.Find("Player");
        ads = this.gameObject.AddComponent<AudioSource>();
        ads.volume = PlayerPrefs.GetInt("CreatureVolume", 50)/100f;
    }

    // Update is called once per frame
    protected override void Update() {
        timer += Time.deltaTime;
        //if we can see the player, remove 1 energy per second
        if(canSeePlayer(25f)) {
            base.FacePlayer();
            if(!firstLook) {
                ads.pitch = 1f;
                ads.PlayOneShot(this.firstLookSound);
                firstLook = true;
            }
            timeLookingAtPlayer += Time.deltaTime;
            if(timer >= energyDrainCooldown) {
                SC_FPSController.Instance.Debuff("Energy", EnergyDrainAmount);
                timer = 0f;
            }
        }
        else {
            firstLook = false;
            timeLookingAtPlayer = 0f;
        }

        if(timeLookingAtPlayer >= 6.5f) {
            ads.pitch = Random.Range(1.65f, 1.8f);
            ads.PlayOneShot(this.scareSound);
            SC_FPSController.Instance.Debuff("Energy", EnergyDrainAmount*2.5f);
            timeLookingAtPlayer = 1f;
        }
    }
}
