using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundControl : MonoBehaviour
{
    [SerializeField]
    public AudioClip correctGuess;
    [SerializeField]
    public AudioClip incorrectGuess;
    public GameObject walkingSound;
    public static SoundControl Instance;
    private Dictionary<AudioSource, float> pausedAudioSources = new Dictionary<AudioSource, float>();
    public AudioSource audioSource;

    public float MasterVolume {
        get {
            return AudioListener.volume;
        }
        set {
            AudioListener.volume = value;
        }
    }

    void Awake() {
        Instance = this;
        audioSource = this.gameObject.AddComponent<AudioSource>();
        //set master volume
        AudioListener.volume = PlayerPrefs.GetInt("MasterVolume") / 100f;
    }

    public void PauseSound(bool pausing) {
        if(pausing) {
            AudioSource[] a = FindObjectsOfType<AudioSource>();
            foreach (AudioSource audioSource in a) {

                if (!audioSource.isPlaying || audioSource.gameObject.name.Equals("JumpscareAudioSource")) continue;
                pausedAudioSources.Add(audioSource, audioSource.time);
                audioSource.Pause();
            }
        }
        else {
            foreach (var audioSource in pausedAudioSources.Keys) {
                if(audioSource == null) {
                    continue;
                }
                audioSource.Play();
            }
            pausedAudioSources.Clear();
        }
    }

    public void guessFeedbackSound(bool correct) {
        if(correct) {
            audioSource.pitch = 1f;
            audioSource.volume = 1f;
            audioSource.clip = correctGuess;
        }
        else {
            audioSource.pitch = 0.8f;
            audioSource.volume = 0.8f;
            audioSource.clip = incorrectGuess;
        }
        audioSource.Play();
    }
}
