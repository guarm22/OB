using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleData : MonoBehaviour {
    public string description;
    
    [HideInInspector]
    public GameObject obj;

    void Awake() {
        obj = this.gameObject;
    }
}
