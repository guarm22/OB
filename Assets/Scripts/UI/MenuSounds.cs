using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class MenuSounds : MonoBehaviour {
    [HideInInspector]
    public AudioSource audioSource;
    public AudioClip clickSound;
    public static MenuSounds Instance;
    
    void Awake() {
        Instance = this;
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = 0.4f;
    }

    public void PlayClickSound(float volume = 0.4f, float pitch = 1) {
        audioSource.clip = clickSound;
        audioSource.volume = volume;
        audioSource.pitch = pitch;
        audioSource.Play();

    }
}
