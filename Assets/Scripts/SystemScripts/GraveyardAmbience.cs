using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraveyardAmbience : MonoBehaviour {
    
    List<ParticleSystem> ps;
    private bool paused = false;
    void Awake() {
        ps = new List<ParticleSystem>(GameObject.FindObjectsOfType<ParticleSystem>());
    }
    void Update() {
        if(PlayerUI.paused) {
            paused = true;
            foreach(ParticleSystem p in ps) {
                p.Pause();
            }
        }
        else if (!PlayerUI.paused && paused) {
            paused = false;
            foreach(ParticleSystem p in ps) {
                p.Play();
            }
        }
    }
}
