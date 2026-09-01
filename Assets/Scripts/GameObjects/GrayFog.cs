using UnityEngine;
using System.Collections.Generic;

public class GrayFog : MonoBehaviour {

    private List<ParticleSystem> ps;

    private bool paused;

    void Start() {
        ps = new List<ParticleSystem>(GetComponentsInChildren<ParticleSystem>());
    }

    // Update is called once per frame
    void Update() {
        if(PlayerUI.paused && !paused) {
            paused = true;
            foreach(ParticleSystem p in ps) {
                p.Pause();
            }
        }
        else if(!PlayerUI.paused && paused)
        {
            paused = false;
            foreach(ParticleSystem p in ps)
            {
                p.Play();
            }
        }
    }
}
