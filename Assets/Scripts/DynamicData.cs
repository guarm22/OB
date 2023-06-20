using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicData : MonoBehaviour
{
    public ANOMALY_TYPE type;
    public float cooldown = 10f;

    public float timeSinceLastDespawn {get; set;}
    void Start() {
        this.timeSinceLastDespawn = -20f;
    }
}
