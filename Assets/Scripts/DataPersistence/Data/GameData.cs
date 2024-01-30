using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public int AnomaliesSuccesfullyReported;
    public int LevelsFailed;
    public int LevelsWon;

    public GameData() {
        this.AnomaliesSuccesfullyReported = 0;
        this.LevelsFailed = 0;
        this.LevelsWon = 0;
    }
}
