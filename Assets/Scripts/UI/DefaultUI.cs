using System.Collections;
using TMPro;
using UnityEngine;
using DG.Tweening;
using System;
using System.Linq;
using UnityEngine.SceneManagement;


public class DefaultUI : MonoBehaviour {
    public TMP_Text label;
    public TMP_Text flashlight;
    
    public GameObject timer;
    public GameObject crosshair;

    public AudioClip writeBoop;

    public TMP_Text startingText;

    private AudioSource audioSource;
    private String originalText;
    public bool isTimerAnimFinished = false;

    public TMP_Text prevReport;

    Vector3 originalPos;
    Vector3 originalScale;

    void Start() {
        originalText = startingText.text;
        originalPos = timer.transform.position;
        originalScale = timer.transform.localScale;
        startingText.text = "";
        audioSource = this.gameObject.AddComponent<AudioSource>();
        StartCoroutine(TimerAnimation());
    }

    public IEnumerator TimerAnimation() {
        if(SceneManager.GetActiveScene().name == "Tutorial") {
            forceFinishAnim();
            yield break;
        }
        //increase timer size
        timer.transform.localScale = new Vector3(originalScale.x * 2.5f, originalScale.y * 2.5f, originalScale.z * 2.5f);
        //move timer to the center of the screen
        timer.transform.position = crosshair.transform.position + new Vector3(0, 50, 0);

        String text = originalText;
        startingText.text = "";
        yield return new WaitForSeconds(1.2f);
        //have text appear in typewriter animation
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

        yield return new WaitForSeconds(3f);
        isTimerAnimFinished = true;
    }

    private void forceFinishAnim() {
        timer.transform.DOMove(originalPos, 0.1f);
        timer.transform.DOScale(originalScale, 0.1f);
        //remove text
        startingText.transform.DOScale(Vector3.zero, 0.1f);
        isTimerAnimFinished = true;
    }

    void SetText() {
        float reportTime = DivergenceControl.Instance.TimeOfLastreport;
        float lockout = DivergenceControl.Instance.ReportLockout;
        Report lastReport = DivergenceControl.Instance.reports.LastOrDefault();

        //If in the first 5 seconds of the level, none of the if statements will be true
        //this causes the report text to be stuck on "Verifying..." if a report was made right at the start

        if(!DivergenceControl.Instance.PendingReport && Time.time - reportTime > lockout+5) {
            prevReport.text = "";
            label.color = Color.white;
            label.text = "Report";
        }
        else if(Time.time - reportTime < lockout && DivergenceControl.Instance.PendingReport) {
            label.text = "Verifying...";

            String types = "";
            foreach(String type in lastReport.reportTypes) {
                types += type;
                if(type != lastReport.reportTypes.Last()) {
                    types += ", ";
                }
            }

            prevReport.text = "Reported " + types + " at " + lastReport.room;
        }
        else if(Time.time - reportTime > lockout && DivergenceControl.Instance.WasMostRecentReportCorrect && !DivergenceControl.Instance.PendingReport) {
            label.color = Color.green;
            label.text = "CORRECT";
        }
        else if(Time.time - reportTime > lockout && !DivergenceControl.Instance.WasMostRecentReportCorrect && !DivergenceControl.Instance.PendingReport) {
            label.text = "WRONG";
            label.color = Color.red;

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
        if(PlayerUI.Instance.havePausedAtleastOnce && isTimerAnimFinished==false) {
            forceFinishAnim();
        }
        SetText();
    }
}
