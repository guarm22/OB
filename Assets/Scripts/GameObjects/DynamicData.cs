using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicData : MonoBehaviour
{
    public ANOMALY_TYPE type;
    public float cooldown = 0f;

    public float cooldownTime = 10f;
    public float killedTime = 0f;

    public bool beenKilled;

    public CustomDivergence customDivergence;
    
    void Update() {
        if(cooldown > 0f) {
            cooldown -= Time.deltaTime;
        }
    }
}
