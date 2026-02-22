using System;
using UnityEngine;
using DG.Tweening;

public class CameraFocusSphere : CustomDivergence{
    public String room;
    private bool on = false;
    public GameObject player;
    public AudioClip creepySound;
    public AudioSource audioSource;

    private Material originalMaterial;
    private Material overrideMaterial;

    void Start() {
        player = GameObject.Find("Player");
        if(GetComponent<AudioSource>() != null) {
            GetComponent<AudioSource>().clip = creepySound;
        }
        else {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.clip = creepySound;
            audioSource.playOnAwake = false;
            audioSource.loop = false;
            audioSource.spatialBlend = 1.0f;
            GetComponent<AudioSource>().clip = Resources.Load("Sounds/creepy-whistles-66703") as AudioClip;
        }
        originalMaterial = GetComponent<Renderer>().material;
        overrideMaterial = Resources.Load("Materials/Puncture_Shader") as Material;
    }

    public override void DoDivergenceAction(bool enable, DynamicObject obj) {
        room = obj.Room;
        if(enable) {
            on = true;
            //change object material to puncture shader
            GetComponent<Renderer>().material = overrideMaterial;
        }
        else {
            on = false;
            audioSource.Stop();
            audioSource.time = 0f;
            //change object material back to original
            GetComponent<Renderer>().material = originalMaterial;
        }
    }

    
    void Update() {
        if(PlayerUI.paused) {
            audioSource.Pause();
        }
        if(on && PlayerUI.Instance.GetCurrentRoom() == room) {
            if(audioSource != null && !audioSource.isPlaying) {
                audioSource.Play();
            }
            SC_FPSController.Instance.LookAtObject(this.gameObject);
        }
        else if (on && PlayerUI.Instance.GetCurrentRoom() != room) {
            audioSource.Pause();
        }
    }
}
