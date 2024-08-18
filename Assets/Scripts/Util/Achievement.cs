using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Achievement {
    public string Name;
    public string Description;
    public bool Unlocked;
    public int Progress;
    public int Goal;
    public String dateEarned;
    public String steamID;

    public Achievement(string name, string description, bool unlocked, int goal, string steamID) {
        Name = name;
        Description = description;
        Unlocked = false;
        Progress = 0;
        Goal = goal;
        dateEarned = "";
        this.steamID = steamID;
    }
}
