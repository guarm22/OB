using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpandingSphere : CustomDivergence {
    public float expansionSpeed;
    private Vector3 expandScale;
    private GameObject player;

    public AudioSource ads;
    public AudioClip spawnSound;

    public ParticleSystem spawnParticles;

    void Awake() {
        expandScale = new Vector3(expansionSpeed, expansionSpeed, expansionSpeed);
        player = GameObject.Find("Player");
        ExpandSphere(this.gameObject);
    }

    public override void DoDivergenceAction(bool activate, DynamicObject gameObject) {
        if (activate) {
            StartCoroutine(ExpandSphere(this.gameObject));
        }
        else {
            StopAllCoroutines();
            transform.localScale = Vector3.zero;
        }
    }

    public void ManualActivation(float speed = 0.3f) {
        expansionSpeed = speed;
        expandScale = new Vector3(expansionSpeed, expansionSpeed, expansionSpeed);
        StartCoroutine(ExpandSphere(this.gameObject));
    }

    public IEnumerator ExpandSphere(GameObject obj) {
        this.GetComponent<Collider>().enabled = false;

        ads.pitch = Random.Range(1.1f, 1.2f);
        ads.volume = 0.6f;
        ads.PlayOneShot(spawnSound);

        float time = 0;
        //wait until particle system is done playing
        if(spawnParticles != null) {
            float duration = spawnParticles.main.duration;
            spawnParticles.Play();
            while(time < duration-0.3f) {
                time += Time.deltaTime;
                yield return null;
            }
        }
        ads.volume = 1f;
        ads.pitch = 1;
        this.GetComponent<Collider>().enabled = true;

        time = 0;
        while (time < 100) {
            if(PlayerUI.paused) {
                yield return null;
            }
            //if the distance between the player and the expanding sphere is less than the radius of the sphere, end the game
            if (Vector3.Distance(player.transform.position, obj.transform.position) < obj.transform.localScale.x/2) {
                StartCoroutine(GameSystem.Instance.EndGame("puncture"));
                break;
            }            

            time += Time.deltaTime;
            obj.transform.localScale = expandScale * time;
            yield return null;
        }
    }
}
