using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayer : MonoBehaviour {

    public AudioClip testClip;
    public AudioPlayer Instance;
    private AudioSource audioSource;
    void Start() {
        audioSource = this.GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        Instance = this;
    }

    public void PlaySound(AudioClip clip, String subtitle = "") {
        audioSource.clip = clip;
        audioSource.Play();
        if(subtitle != "") {
            SubtitleControl.Instance.ShowSubtitle(subtitle, clip.length * 1.3f);
        }
    }


    void Update() {
        //test play sound
        if(Input.GetKeyDown(KeyCode.U)) {
            TranscriptEntry entry = Transcript.instance.GetEntry("test_01");
            PlaySound(getFileFromName(entry.filename), entry.subtitle);
        } 
    }

    private AudioClip getFileFromName(string filename) {
        return Resources.Load<AudioClip>("Sounds/Voice Lines/" + filename);
    }
}
