using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicDifficulty : MonoBehaviour
{
    private float TimeSinceLastReport=0;

    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        TimeSinceLastReport += Time.deltaTime;
        if(DivergenceControl.Instance.reports.Count <= 0) {
            return;
        }

        Report lastReport = DivergenceControl.Instance.reports[DivergenceControl.Instance.reports.Count - 1];
        //check if report made recently
        if(lastReport.timeMade - GameSystem.Instance.TimeInLevel < 1) {
            TimeSinceLastReport = 0;
        }
    }
}
