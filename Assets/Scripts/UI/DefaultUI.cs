using System.Collections;
using System.Collections.Generic;
using DG.Tweening.Core.Easing;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DefaultUI : MonoBehaviour
{
    public TMP_Text label;
    public TMP_Text flashlight;
    // Start is called before the first frame update
    void Start() {
        
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
