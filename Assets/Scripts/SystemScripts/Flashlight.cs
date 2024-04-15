using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class Flashlight : MonoBehaviour
{

    public Light beam;
    private float staticEnergyDrainPerSecond;
    private float totalEnergyDrainPerSecond;
    public float energyDrainModifier = 1f;
    private float timer;
    public static Flashlight Instance;
    // Start is called before the first frame update
    void Start() {
        beam.enabled = false;   
        float eps = PlayerPrefs.GetFloat("EPS");
        staticEnergyDrainPerSecond = eps * 1.5f;
        totalEnergyDrainPerSecond = staticEnergyDrainPerSecond * energyDrainModifier;
        Instance = this;
    }

    // Update is called once per frame
    void Update() {
        if(GameSystem.Instance.GameOver || PlayerUI.paused) {
            return;
        }

        if(GameSystem.Instance.CurrentEnergy <= 0) {
            beam.enabled = false;
        }

        if(Input.GetKeyDown(KeyCode.F)) {
            beam.enabled = !beam.enabled;
        }

        if(beam.enabled) {
            timer += Time.deltaTime;
            if(timer >= 1f) {
                timer = 0;
                GameSystem.Instance.ChangeEnergy(-totalEnergyDrainPerSecond);
            }
        }
    }
}
