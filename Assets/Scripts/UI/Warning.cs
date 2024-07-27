using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Warning : MonoBehaviour {

    public TMP_Text warningText;

    public String warning = "!!";
    public Color warningColor = Color.yellow;
    public AudioClip warningSound;

    public String danger = "!!!!";
    public Color dangerColor = Color.red;
    public AudioClip dangerSound;
    
    private AudioSource audioSource;

    public float warningTime = 5.0f;

    // The threshold (amount of divergences before max) at which the warning will be displayed
    public static float warningThreshold = 0.5f;

    public static float dangerThreshold = 0.8f;

    private bool didWarning = false;
    private bool didDanger = false;

    private bool inCouroutine = false;

    public static Warning Instance;

    void Awake() {
        Instance = this;
    }

    void Start() {

        warningText = GetComponent<TMP_Text>();
        audioSource = GetComponent<AudioSource>();
    }

    public void TurnOffAlert() {
        audioSource.Stop();
        audioSource.clip = null;
        inCouroutine = false;
        warningText.text = "";
        StopAllCoroutines();
    }

    private IEnumerator flashWarning(string txt, Color color, AudioClip sound, float flashSpeed = 1.0f) {
        yield return new WaitForSeconds(1f);
        float elapsed = 0.0f;
        inCouroutine = true;
        audioSource.clip = sound;
        audioSource.Play();

        warningText.color = color;
        while(elapsed < warningTime) {
            elapsed += Time.deltaTime;
            if(elapsed % flashSpeed < flashSpeed/2) {
                warningText.text = txt;
            } else {
                warningText.text = "";
            }

            yield return null;
        }

        inCouroutine = false;
    }

    // Update is called once per frame
    void Update() {
        if(didDanger && didWarning) {
            return;
        }

        int divergences = DivergenceControl.Instance.DivergenceList.Count;
        int maxDivergences = DivergenceControl.Instance.Rooms.Count;

        float divergenceRatio = (float) divergences / maxDivergences;

        if(divergenceRatio > dangerThreshold && !didDanger) {
            didDanger = true;
            StartCoroutine(flashWarning(danger, dangerColor, dangerSound, 0.5f));

        } 
        else if(divergenceRatio > warningThreshold && !didWarning) {
            didWarning = true;
            StartCoroutine(flashWarning(warning,warningColor, warningSound));
        } 
        else if(!inCouroutine) {
            warningText.text = "";
        }

        //remove comments and return at the top to make warnings play multiple times

        /*if(didDanger && divergenceRatio <= dangerThreshold) {
            didDanger = false;
        }

        if(didWarning && divergenceRatio <= warningThreshold) {
            didWarning = false;
        }*/


    }
}
