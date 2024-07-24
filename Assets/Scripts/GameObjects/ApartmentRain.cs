using System.Collections;
using System.Collections.Generic;
using SojaExiles;
using UnityEngine;
using UnityEngine.Audio;

public class ApartmentRain : MonoBehaviour {

    public AudioSource rainSound;
    public AudioMixerGroup mixerGroup;

    [HideInInspector]
    public GameObject player;
    public List<GameObject> windows;
    private float distance = 5;
    
    public float insideRainVolume = 2500f;
    public float outsideRainVolume = 10000f;
    void Start() {
        player = GameObject.Find("Player");
        rainSound = GetComponent<AudioSource>();
    }

    private IEnumerator ChangeLowPass(float endValue = 22000, float duration = 0.5f) {        
        float elapsedTime = 0;
        float startValue = mixerGroup.audioMixer.GetFloat("lowpass", out float val) ? val : 22000;

        if(startValue == endValue) {
            yield break;
        }

        while(elapsedTime < duration) {
            elapsedTime += Time.deltaTime;
            mixerGroup.audioMixer.SetFloat("lowpass", Mathf.Lerp(startValue, endValue, elapsedTime / duration));
            yield return null;
        }
    }

    void Update() {
        
        bool isCloseToWindow = false;
    
        //if player is close to open window, set lowpass filter to 22000
        foreach(GameObject window in windows) {
            if(Vector3.Distance(player.transform.position, window.transform.position) < distance 
            && window.GetComponent<opencloseWindowApt>().open) {
                isCloseToWindow = true;
                StartCoroutine(ChangeLowPass(outsideRainVolume, 0.5f));
                return;
            }
        }
        if(!isCloseToWindow) {
            StartCoroutine(ChangeLowPass(insideRainVolume, 0.5f));
            isCloseToWindow = false;
        }
    }
}
