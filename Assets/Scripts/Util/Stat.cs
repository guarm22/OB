using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Stat {
    public string name;
    public String stringRep;
    public int value;

    public Stat(string name, int value, string s = "") {
        this.name = name;
        this.value = value;
        stringRep = s;
    }
}
