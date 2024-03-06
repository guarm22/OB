using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAudio : MonoBehaviour
{
    public AudioClip sound;
    public AudioClip distantSound;
    public float soundRepeatTimer;
    private float timeSinceLastSound;
    private GameObject player;
    private AudioClip currentSound;
    private float clipLength;
    private bool isPlaying;
    private bool justPaused;
    // Start is called before the first frame update
    void Start() {
        player = GameObject.Find("Player");
        
        if(Vector3.Distance(this.transform.position, player.transform.position) > 10 && distantSound != null) {
            currentSound = distantSound;
        }
        else {
            currentSound = sound;
        }
        clipLength = currentSound.length;
        isPlaying = true;
        AudioSource.PlayClipAtPoint(currentSound, this.transform.position);
    }

    // Update is called once per frame
    void Update() {
        if(GameSystem.Instance.GameOver || SC_FPSController.paused) {
            justPaused = true;
            foreach (var audioSource in FindObjectsOfType<AudioSource>()) {
                if(audioSource.clip == currentSound) {
                    audioSource.Pause();
                }
            }
            return;
        }
        else if(justPaused) {
            justPaused = false;
            foreach (var audioSource in FindObjectsOfType<AudioSource>()) {
                if(audioSource.clip == currentSound) {
                    audioSource.UnPause();
                }
            }
        }

        timeSinceLastSound += Time.deltaTime;
        isPlaying = timeSinceLastSound >= currentSound.length;
        if(!isPlaying) {
            if(Vector3.Distance(this.transform.position, player.transform.position) > 10 && distantSound != null) {
                currentSound = distantSound;
            }
            else {
                currentSound = sound;
            }
        }

        if(timeSinceLastSound > soundRepeatTimer) {
            timeSinceLastSound = 0f;
            AudioSource.PlayClipAtPoint(currentSound, this.transform.position);
        }
    }
}
