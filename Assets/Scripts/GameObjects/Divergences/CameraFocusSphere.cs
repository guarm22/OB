using System;
using UnityEngine;
using DG.Tweening;

public class CameraFocusSphere : CustomDivergence{
    public String room;
    private bool on = false;
    public GameObject player;
    public AudioClip creepySound;
    public AudioSource audioSource;

    private bool changedSpeed = false;

    void Start() {
        player = GameObject.Find("Player");
        if(GetComponent<AudioSource>() != null) {
            GetComponent<AudioSource>().clip = creepySound;
        }
    }

    public override void DoDivergenceAction(bool enable, DynamicObject obj) {
        room = obj.Room;
        if(enable) {
            on = true;
            obj.Obj.transform.DOScale(new Vector3(1,1,1), 2.5f);
        }
        else {
            on = false;
            audioSource.Stop();
            audioSource.time = 0f;
            obj.Obj.transform.DOScale(new Vector3(0f, 0f, 0f), 2.5f);
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
