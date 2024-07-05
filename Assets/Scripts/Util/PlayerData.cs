using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
[System.Serializable]
public class PlayerData {
    public Stat ReportsMade;
    public Stat DivergencesReported;
    public Stat CreaturesReported;

    public PlayerData() {
        ReportsMade = new Stat("Reports Made", 0);
        DivergencesReported = new Stat("Divergences Reported", 0);
        CreaturesReported = new Stat("Creatures Reported", 0);
    }
}
