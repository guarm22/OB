using UnityEngine;
using System.Collections.Generic;

public class Fireflies : MonoBehaviour {

    private ParticleSystem fireflies;

    private GameObject player;

    private float updateRate = 0.5f;
    private float nextUpdate = 0f;

    private bool isPlaying = true;

    [Tooltip("The distance at which the fireflies will start playing")]
    public float minSightDistance = 25f;

    [Tooltip("The rate at which the fireflies will emit. Same as the 'emission rate' under 'emission' in the particle system")]
    public float emissionRate = 0.5f;

    [Tooltip("The speed at which the fireflies will move. Same as the 'simulation speed' in the particle system")]
    public float speed = 0.6f;

    [Tooltip("The size of the fireflies. Same as the 'start size' in the particle system")]
    public float size = 0.2f;

    [Tooltip("The lifetime of the fireflies. Same as the 'duration' in the particle system")]
    public float lifetime = 2.5f;

    void Awake() {
        fireflies = GetComponent<ParticleSystem>();
        player = GameObject.Find("Player");
        fireflies.Stop();
        
        var emission = fireflies.emission;
        emission.rateOverTime = emissionRate;

        var main = fireflies.main;
        main.simulationSpeed = speed;

        main.startSize = size;

        main.duration = lifetime;
        fireflies.Play();
    }

    void Update() {
        nextUpdate += Time.deltaTime;
        if(nextUpdate >= updateRate) {
            nextUpdate = 0f;
            CheckPlayerDistance();
        }
    }

    private void CheckPlayerDistance() {
        if(Vector3.Distance(transform.position, player.transform.position) > minSightDistance) {
            if(isPlaying) {
                return;
            }
            fireflies.Play();
            isPlaying = true;
        } 
        else {
            isPlaying = false;
            fireflies.Stop();
        }
    }

}
