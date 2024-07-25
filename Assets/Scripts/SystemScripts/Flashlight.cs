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
        staticEnergyDrainPerSecond = eps * 1.25f;
        totalEnergyDrainPerSecond = staticEnergyDrainPerSecond * energyDrainModifier;
        Instance = this;
    }

    // Update is called once per frame
    void Update() {
        if(beam.enabled) {
            timer += Time.deltaTime;
            if(timer >= 1f) {
                timer = 0;
                GameSystem.Instance.ChangeEnergy(-totalEnergyDrainPerSecond);
            }
        }
        if(GameSystem.Instance.GameOver || PlayerUI.paused || PlayerUI.Instance.inMenu) {
            return;
        }

        if(GameSystem.Instance.CurrentEnergy <= 0) {
            beam.enabled = false;
        }

        if(Input.GetKeyDown(KeybindManager.instance.GetKeybind("Flashlight"))) {
            beam.enabled = !beam.enabled;
        }

        //get the x rotation of the camera
        float x = Camera.main.transform.eulerAngles.x;
        //set the x rotation of the flashlight to the y rotation of the camera
        transform.eulerAngles = Camera.main.transform.eulerAngles;
    }
}
