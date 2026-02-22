using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class PlayAudio : CustomDivergence {
    public AudioClip sound;
    public float soundRepeatTimer;
    public float audioRange = 100f;
    public float soundDelay = 1.5f;
    private AudioSource audioSource;
    private GameObject player;

    private AudioMixerGroup mixerGroup;
    // Start is called before the first frame update
    void Awake() {
        if(PlayerPrefs.GetString("AudioDivergences", "YES") == "NO"){
            Destroy(this.GetComponent<DynamicData>());
            Destroy(this.GetComponent<PlayAudio>());
        }

        player= GameObject.Find("Player");
        mixerGroup = Resources.Load<AudioMixer>("Sounds/Mixers/PlayAudioMixer").FindMatchingGroups("Master")[0];
        if(this.GetComponent<AudioSource>() == null) {
            audioSource = this.gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 1f;
            audioSource.maxDistance = audioRange;
            audioSource.outputAudioMixerGroup = mixerGroup;
        }
        else {
            audioSource = this.GetComponent<AudioSource>();
        }
        audioSource.clip = sound;
    }

    private void ChangeLowPass(float distance = 0.5f) {
        if (distance == 0) {
            mixerGroup.audioMixer.SetFloat("lowpass", 15000);
            return;
        }
        float endValue = 5000;
        if(distance < 5) {
            endValue = Random.Range(2000, 2500);
        }
        else if (distance > 5 && distance < 10) {
            endValue = Random.Range(1500, 2000);
        }
        else if (distance > 10 && distance < 20) {
            endValue = Random.Range(1000, 2000);
        }
        else if (distance > 20) {
            endValue = Random.Range(500, 1000);
        }

        mixerGroup.audioMixer.SetFloat("lowpass", endValue);
    }

    void Update() {
        //raycast from the object to the player
        RaycastHit hit;
        Vector3 objectLoc = new Vector3(audioSource.transform.position.x, audioSource.transform.position.y, audioSource.transform.position.z);
        LayerMask layerMask = LayerMask.GetMask("Floor", "Player");

        if(!audioSource.isPlaying) { return;}
        if(Physics.Raycast(objectLoc, (player.transform.position-transform.position).normalized, out hit, audioRange, layerMask)) {
            float distance = Vector3.Distance(player.transform.position, audioSource.transform.position);
            if(hit.collider.gameObject == player.gameObject) {
                ChangeLowPass(0);
            }
            else {
                ChangeLowPass(distance);
            }
        }
        
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
        float elapsedTime = soundRepeatTimer-soundDelay;
        while(true) {
            if(PlayerUI.paused || GameSystem.Instance.GameOver) {
                yield return null;
                continue;
            }

            elapsedTime += Time.deltaTime;
            if(elapsedTime >= soundRepeatTimer) {
                audioSource.Play();
                elapsedTime = 2f;
            }
            yield return null;
        }
    }
}
