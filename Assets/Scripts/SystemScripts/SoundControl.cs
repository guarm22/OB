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
    private bool justPaused = false;

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

        //set master volume
        AudioListener.volume = PlayerPrefs.GetInt("MasterVolume") / 100f;
    }

    void Update() {
        walking();
    }

    private void walking() {
        if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D)) {
            walkingSound.SetActive(true);
            if(SC_FPSController.Instance.isRunning) {
                walkingSound.GetComponent<AudioSource>().pitch = 1.5f;
            }
            else {
                walkingSound.GetComponent<AudioSource>().pitch = 1f;
            }
        }
        else {
            walkingSound.SetActive(false);
        }
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

    public IEnumerator guessFeedbackSound(bool correct) {
        GameObject player = GameObject.Find("Player");
        player.AddComponent<AudioSource>();
        AudioSource audioSource = player.GetComponent<AudioSource>();
        if(correct) {
            audioSource.clip = correctGuess;
        }
        else {
            audioSource.volume = 0.5f;
            audioSource.clip = incorrectGuess;
        }
        audioSource.Play();
        yield return new WaitForSeconds(audioSource.clip.length);
        Destroy(audioSource);
    }
}
