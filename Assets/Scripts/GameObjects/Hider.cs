using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Hider : CreatureBase {
    public float EnergyDrainAmount = 2f;
    private float energyDrainCooldown = 1f;
    private float timer = 0f;
    private float timeLookingAtPlayer = 0f;

    public AudioSource ads;

    public AudioClip scareSound;
    public AudioClip firstLookSound;
    public GameObject eyeball;

    private bool firstLook = false;

    public float lookRange = 45f;

    public bool currentlySeeingPlayer = false;

    // Start is called before the first frame update
    protected override void Awake() {
        player = GameObject.Find("Player");
        ads = this.gameObject.AddComponent<AudioSource>();
        ads.volume = PlayerPrefs.GetInt("CreatureVolume", 50)/100f;
        StartCoroutine(MoveBody());
    }

    private IEnumerator MoveBody() {
        float moveDuration = 3f;
        float movDirection = 0.2f;
        Vector3 currentPos = eyeball.transform.position;
        while (true) {
            eyeball.transform.DOMove(new Vector3(currentPos.x, currentPos.y+movDirection, currentPos.z), moveDuration);
            yield return new WaitForSeconds(moveDuration);
            movDirection*=-1;
        }
    }

    // Update is called once per frame
    protected override void Update() {
        if(PlayerUI.paused) { return; }

        timer += Time.deltaTime;
        //if we can see the player, remove 1 energy per second
        if(canSeePlayer(lookRange)) {
            base.FacePlayer();
            currentlySeeingPlayer = true;
            if(!firstLook) {
                ads.pitch = 1f;
                ads.PlayOneShot(this.firstLookSound);
                firstLook = true;
            }
            timeLookingAtPlayer += Time.deltaTime;
            if(timer >= energyDrainCooldown) {
                if(PlayerPrefs.GetInt("EnergySapped", 0) == 1) {
                    PlayerDebuffs.Instance.Energy(EnergyDrainAmount);
                }
                SC_FPSController.Instance.Debuff("Energy", EnergyDrainAmount);
                timer = 0f;
            }
        }
        else if(!canSeePlayer() && currentlySeeingPlayer) {
            currentlySeeingPlayer = false;
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
