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
        ads.volume = PlayerPrefs.GetInt("SFXVolume", 50)/100f;
    }

    public override void DoDivergenceAction(bool activate, DynamicObject gameObject) {
        if (activate) {
            StartCoroutine(ExpandSphere(this.gameObject));
        }
        else {
            StopAllCoroutines();
            this.GetComponent<Collider>().enabled = false;
            StartCoroutine(EndSphere());
        }
    }

    public void ManualActivation(float speed = 0.3f) {
        expansionSpeed = speed;
        expandScale = new Vector3(expansionSpeed, expansionSpeed, expansionSpeed);
        StartCoroutine(ExpandSphere(this.gameObject));
    }

    public IEnumerator EndSphere(float shrinkTime = 0.5f) {
        //slowly shrink the sphere
        float time = 0;
        Vector3 initialScale = this.transform.localScale;
        while (time < shrinkTime) {
            if(PlayerUI.paused) {
                yield return new WaitUntil(() => !PlayerUI.paused);
            }

            time += Time.deltaTime;
            this.transform.localScale = Vector3.Lerp(initialScale, Vector3.zero, time/shrinkTime);
            yield return null;
        }
        
        transform.localScale = Vector3.zero;
    }
    public IEnumerator ExpandSphere(GameObject obj) {
        this.GetComponent<Collider>().enabled = false;
        ads.pitch = Random.Range(1.1f, 1.2f);
        ads.PlayOneShot(spawnSound);
        this.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        float time = 0;
        //wait until particle system is done playing
        if(spawnParticles != null) {
            float duration = spawnParticles.main.duration;
            spawnParticles.Play();
            while(time < duration-0.3f) {
                if(PlayerUI.paused) {
                    yield return new WaitUntil(() => !PlayerUI.paused);
                }

                time += Time.deltaTime;
                yield return null;
            }
        }
        ads.pitch = 1;
        this.GetComponent<Collider>().enabled = true;

        time = 0;
        while (time < 200) {
            if(PlayerUI.paused) {
                yield return new WaitUntil(() => !PlayerUI.paused);
            }
            //if the distance between the player and the expanding sphere is less than the radius of the sphere, end the game
            if (Vector3.Distance(player.transform.position, obj.transform.position) < obj.transform.localScale.x/2) {
                GameSystem.Instance.EndGame("puncture");
                yield return null;
                break;
            }            

            time += Time.deltaTime;
            obj.transform.localScale = expandScale * time;
            yield return null;
        }
    }
}
