using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
[System.Serializable]
public class PlayerData {
    public int ReportsMade;
    public int DivergencesReported;
    public int CreaturesReported;

    public PlayerData() {
        ReportsMade = 0;
        DivergencesReported = 0;
        CreaturesReported = 0;
    }
}
