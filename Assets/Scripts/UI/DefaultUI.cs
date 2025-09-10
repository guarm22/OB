using System.Collections;
using TMPro;
using UnityEngine;
using DG.Tweening;
using System;


public class DefaultUI : MonoBehaviour {
    public TMP_Text label;
    public TMP_Text flashlight;
    
    public GameObject timer;
    public GameObject crosshair;

    public AudioClip writeBoop;

    public TMP_Text startingText;

    private AudioSource audioSource;

    void Start() {
        audioSource = this.gameObject.AddComponent<AudioSource>();
        StartCoroutine(TimerAnimation());
    }

    private IEnumerator TimerAnimation() {
        Debug.Log("starting timer animation");
        Vector3 originalPos = timer.transform.position;
        Vector3 originalScale = timer.transform.localScale;

        //increase timer size
        timer.transform.localScale = new Vector3(originalScale.x * 2.5f, originalScale.y * 2.5f, originalScale.z * 2.5f);
        //move timer to the center of the screen
        timer.transform.position = crosshair.transform.position + new Vector3(0, 50, 0);

        String text = startingText.text;
        startingText.text = "";
        //have text appear in typewriter animation
        yield return new WaitForSeconds(1.2f);
        int element = 0;
        while(startingText.text.Equals(text) == false) {
            startingText.text += text[element];
            if(char.IsLetterOrDigit(text[element])) {
                audioSource.pitch = UnityEngine.Random.Range(0.9f, 1.05f);
                audioSource.PlayOneShot(writeBoop);
            }
            element += 1;
            yield return new WaitForSeconds(0.15f);
        }

        //starting animations completed, now linger in center for a few seconds
        yield return new WaitForSeconds(3.5f);
        //slowly move timer back to normal position and normal size
        timer.transform.DOMove(originalPos, 3f);
        timer.transform.DOScale(originalScale, 2.5f);

        //remove text
        startingText.transform.DOScale(Vector3.zero, 0.5f);
    }

    void SetText() {
        float reportTime = DivergenceControl.Instance.TimeOfLastreport;
        float lockout = DivergenceControl.Instance.ReportLockout;

        if(!DivergenceControl.Instance.PendingReport && Time.time - reportTime > lockout+5) {
            label.text = "Report";
        }
        else if(Time.time - reportTime < lockout && DivergenceControl.Instance.PendingReport) {
            label.text = "Verifying...";
        }
        else if(Time.time - reportTime > lockout && DivergenceControl.Instance.WasMostRecentReportCorrect && !DivergenceControl.Instance.PendingReport) {
            label.text = "CORRECT";
        }
        else if(Time.time - reportTime > lockout && !DivergenceControl.Instance.WasMostRecentReportCorrect && !DivergenceControl.Instance.PendingReport && GameSystem.Instance.TimeInLevel > 10.5) {
            label.text = "WRONG";
        }

        if(Flashlight.Instance.beam.enabled) {
            flashlight.text = "Flashlight: ON";
        }
        else {
            flashlight.text = "Flashlight: OFF";
        }
    }

    // Update is called once per frame
    void Update() {
        if(PlayerUI.paused || GameSystem.Instance.GameOver) {
            return;
        }
        SetText();
    }
}
