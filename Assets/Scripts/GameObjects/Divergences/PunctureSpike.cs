using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunctureSpike : CustomDivergence {
    private Vector3 startLoc;
    public List<UnityEngine.Vector3> endLoc;
    public float moveTime = 1f;
    public float warningTime = 5f;
    private Vector3 originalScale;

    public GameObject testsphere;

    public AudioSource audioSource;
    public AudioClip warningSound;
    public AudioClip moveSound;

    private GameObject player;

    private float elapsedTime = 0f;

    void Start() {
        startLoc = transform.localPosition;
        originalScale = new Vector3(.25f, .25f, .25f);
        player = GameObject.Find("Player");
    }


    public override void DoDivergenceAction(bool activate, DynamicObject obj) {
        if(activate) {
            // Move the spike to the end location over the specified move time
            this.GetComponent<MeshCollider>().enabled = true;
            StartCoroutine(MoveToLocation());
        } else {
            // Move the spike back to the start location over the specified move time
            this.GetComponent<MeshCollider>().enabled = false;
            elapsedTime = 0f;
            audioSource.Stop();
            transform.localScale = new Vector3(0f, 0f, 0f);
            StopAllCoroutines();
        }
    }

    private IEnumerator MoveToLocation() {
        bool move = false;
        float movingTime = 0f;
        bool resetting = true;
        Vector3 randomEndLoc = endLoc[Random.Range(0, endLoc.Count)];
        RotateSpike(randomEndLoc);
        while(true) {
            if(PlayerUI.paused){yield return null; continue;}

            if (Vector3.Distance(player.transform.position, this.transform.position) < 1 && !resetting) {
                GameSystem.Instance.EndGame("puncture");
                yield return null;
                break;
            }

            if (resetting) {
                if(transform.localScale.x >= originalScale.x) {
                    movingTime = 0f;
                    resetting = false;
                } else {
                    transform.localScale = Vector3.Lerp(new Vector3(0f, 0f, 0f), originalScale, movingTime / warningTime);
                    movingTime += Time.deltaTime;
                }
                yield return null;
                continue;
            }

            if(elapsedTime == 0){
                audioSource.clip = warningSound;
                audioSource.Play();
            }
            elapsedTime += Time.deltaTime;
            if(elapsedTime >= warningTime) {
                move = true;
            }
            if(move) {
                // Move the spike to the end location
                if(transform.localPosition == startLoc) {
                    audioSource.clip = moveSound;
                    audioSource.Play();
                }
                transform.localPosition = Vector3.Lerp(startLoc, randomEndLoc, movingTime / moveTime);
                movingTime += Time.deltaTime;
                if(transform.localPosition == randomEndLoc) {
                    
                    move = false;
                    movingTime = 0f;
                    elapsedTime = 0f;
                    transform.localScale = new Vector3(0f, 0f, 0f);
                    transform.localPosition = startLoc;
                    resetting = true;
                    randomEndLoc = endLoc[Random.Range(0, endLoc.Count)];

                    RotateSpike(randomEndLoc);
                }
            }
            yield return null;
        }
    }

    private void RotateSpike(Vector3 loc) {
        transform.LookAt(loc);
        transform.Rotate(new Vector3(0, -169, 0));
    }

    void Update() {
        
    }
}
