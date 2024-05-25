using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Collectible {
    public string name;
    public string description; 
    public bool isCollected; 
    public string map; 

    public Collectible(string name, string description, bool isCollected, string map) {
        this.name = name;
        this.description = description;
        this.isCollected = isCollected;
        this.map = map;
    }
}
