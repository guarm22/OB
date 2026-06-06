using UnityEngine;
using System.Collections.Generic;

public class FootstepControl : MonoBehaviour {

    public GameObject walkingSound;
    private AudioSource audioSource;
    private GameObject player;

    public string defaultSurfaceTag = "CarpetFloor";

    private string currentSurfaceTag;
    private string playingSurfaceTag = "null";


    public AudioClip TileFootstep;
    public AudioClip CarpetFootstep;

    void Start() {
        player = GameObject.Find("Player");
        audioSource = walkingSound.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update() {
        walking();
        DetectGround();
    }

    private void DetectGround() {
        RaycastHit hit;
        //only detect ground layer
        if (Physics.Raycast(player.transform.position, Vector3.down, out hit, 1.5f, LayerMask.GetMask("Floor"))) {
            currentSurfaceTag = hit.collider.tag;
            Debug.Log("Current surface: " + currentSurfaceTag);
        }

        if(currentSurfaceTag == playingSurfaceTag) {
            return;
        }

        playingSurfaceTag = currentSurfaceTag;
        if(currentSurfaceTag == "Untagged") {
            currentSurfaceTag = defaultSurfaceTag;
        }

        if(currentSurfaceTag == "TileFloor" && TileFootstep != null) {
            ChangeFootstepSound(TileFootstep);
        }
        else if(currentSurfaceTag == "CarpetFloor" && CarpetFootstep != null) {
            ChangeFootstepSound(CarpetFootstep);
        }
        //defaiult to carpet footstep if the surface is not tagged
        else {
            ChangeFootstepSound(CarpetFootstep);
        }
    }

    private void ChangeFootstepSound(AudioClip newFootstep) {
        //reset audiosorce time to 0 to prevent the sound from being cut off when changing footstep sounds
        audioSource.time = 0f;
        audioSource.clip = newFootstep;
        if(walkingSound.activeSelf) {
            audioSource.Play();
        }
    }

    private void walking() {
        if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D)) {
            if(PlayerUI.paused) {walkingSound.SetActive(false); return;}
            walkingSound.SetActive(true);
            if(SC_FPSController.Instance.isRunning) {
                audioSource.pitch = 1.5f;
            }
            else {
                audioSource.pitch = 1f;
            }
        }
        else {
            walkingSound.SetActive(false);
        }
    }
}
