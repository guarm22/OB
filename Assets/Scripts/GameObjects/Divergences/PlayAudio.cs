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
    private bool isPlaying;
    // Start is called before the first frame update
    void Start() {
        player = GameObject.Find("Player");
        
        if(Vector3.Distance(this.transform.position, player.transform.position) > 10 && distantSound != null) {
            currentSound = distantSound;
        }
        else {
            currentSound = sound;
        }
        isPlaying = true;
        AudioSource.PlayClipAtPoint(currentSound, this.transform.position);
    }

    // Update is called once per frame
    void Update() {
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
