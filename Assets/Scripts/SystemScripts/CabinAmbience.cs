using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CabinAmbience : MonoBehaviour {
    public List<AudioClip> clips;
    public AudioSource source;

    private List<AudioClip> playedClips = new List<AudioClip>();

    public float soundTimer = 90f;

    private float minSoundRandomness = 120f;
    private float maxSoundRandomnesss = 180f;

    private bool paused = false;

    void Awake() {
        source.volume = (PlayerPrefs.GetInt("AmbienceVolume", 50)/100f) - .2f;
    }

    private void PlayRandomClip() {
        AudioClip clip = clips[Random.Range(0, clips.Count)];
        source.clip = clip;
        source.Play();

        playedClips.Add(clip);
        clips.Remove(clip);
        if(clips.Count == 0) {
            clips.AddRange(playedClips);
            playedClips.Clear();
        }
        
    }

    void Update() {
        if(PlayerUI.paused) {
            source.Pause();
            paused = true;
            return;
        }
        if(paused && !PlayerUI.paused) {
            source.UnPause();
            paused = false;
        }

        if(soundTimer <= 0) {
            PlayRandomClip();
            soundTimer = Random.Range(minSoundRandomness, maxSoundRandomnesss);
        }
        else {
            soundTimer -= Time.deltaTime;
        }
    }
}
