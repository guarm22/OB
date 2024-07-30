using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAudio : CustomDivergence {
    public AudioClip sound;
    public float soundRepeatTimer;
    private GameObject player;
    private AudioSource audioSource;
    // Start is called before the first frame update
    void Awake() {
        player = GameObject.Find("Player");
        if(this.GetComponent<AudioSource>() == null) {
            audioSource = this.gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 1f;
        }
        else {
            audioSource = this.GetComponent<AudioSource>();
        }
        audioSource.clip = sound;
    }

    public override void DoDivergenceAction(bool enable, DynamicObject obj) {
        if(enable) {
            StartCoroutine(RepeatSound());
        }
        else {
            StopAllCoroutines();
            audioSource.Stop();
        }
    }

    private IEnumerator RepeatSound() {
        float elapsedTime = soundRepeatTimer-1.5f;
        while(true) {
            if(PlayerUI.paused || GameSystem.Instance.GameOver) {
                yield return null;
                continue;
            }

            elapsedTime += Time.deltaTime;
            if(elapsedTime >= soundRepeatTimer) {
                audioSource.Play();
                elapsedTime = 0f;
            }
            yield return null;
        }
    }
}
