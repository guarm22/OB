using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraveyardAmbience : MonoBehaviour {
    
    List<ParticleSystem> ps;
    private bool paused = false;

    public List<AudioSource> audioSources = new List<AudioSource>();
    public List<AudioClip> audioClips = new List<AudioClip>();
    private List<AudioClip> playedClips = new List<AudioClip>();
    public float audioTimer = 60f;

    public float audioTimerMax = 90f;

    public float audioRandomOffset = 10f;

    void Awake() {
        ps = new List<ParticleSystem>(GameObject.FindObjectsOfType<ParticleSystem>());
        audioTimer = audioTimer + Random.Range(-audioRandomOffset, audioRandomOffset);

        audioSources.ForEach(p => p.volume = (PlayerPrefs.GetInt("AmbienceVolume", 50)/100f) - .2f);
        audioSources.ForEach(p => p.playOnAwake = false);
        audioSources.ForEach(p => p.spatialBlend = 1f);
    }

    private void PlaySound() {
        AudioSource source = audioSources[Random.Range(0, audioSources.Count)];
        AudioClip clip = audioClips[Random.Range(0, audioClips.Count)];

        source.clip = clip;
        source.Play();

        //remove clip from list and add to played clips
        //so we dont have repeating sounds too often
        playedClips.Add(clip);
        audioClips.Remove(clip);
        if(audioClips.Count <= 1) {
            audioClips.AddRange(playedClips);
            playedClips.Clear();
        }
    }

    void Update() {
        if(PlayerUI.paused) {
            paused = true;
            foreach(ParticleSystem p in ps) {
                p.Pause();
            }
            return;
        }
        else if (!PlayerUI.paused && paused) {
            paused = false;
            foreach(ParticleSystem p in ps) {
                p.Play();
            }
        }

        audioTimer += Time.deltaTime;
        if(audioTimer >= audioTimerMax) {
            audioTimer = 0 + Random.Range(-audioRandomOffset/2, audioRandomOffset);
            PlaySound();
        }

    }
}
