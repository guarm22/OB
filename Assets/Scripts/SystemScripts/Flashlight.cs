using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flashlight : MonoBehaviour
{

    public Light light;
    private float staticEnergyDrainPerSecond;
    private float totalEnergyDrainPerSecond;
    public float energyDrainModifier = 1f;
    private float timer;
    // Start is called before the first frame update
    void Start() {
        light.enabled = false;   
        float eps = PlayerPrefs.GetFloat("EPS");
        staticEnergyDrainPerSecond = eps * 1.5f;
        totalEnergyDrainPerSecond = staticEnergyDrainPerSecond * energyDrainModifier;
    }

    // Update is called once per frame
    void Update() {
        if(Input.GetKeyDown(KeyCode.F)) {
            light.enabled = !light.enabled;
        }

        if(light.enabled) {
            timer += Time.deltaTime;
            if(timer >= 1f) {
                timer = 0;
                GameSystem.Instance.ChangeEnergy(-totalEnergyDrainPerSecond);
            }
        }
    }
}
