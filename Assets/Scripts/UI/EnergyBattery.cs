using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class EnergyBattery : MonoBehaviour
{

    private int currentlyBlinking = -1;
    public List<Image> batteryImages = new List<Image>();
    private float energy;
    void Update() {
        if(GameSystem.Instance.CurrentEnergy < energy-2.5f) {
            StartCoroutine(RedBattery());
        }
        energy = GameSystem.Instance.CurrentEnergy;
        UpdateBattery();
    }

    private void UpdateBattery() {
        if(energy >= 95) {
            foreach(Image i in batteryImages) {
                i.enabled = true;
            }
            return;
        }

        int batteriesEnabled = 4;
        if(energy >= 90) {
            batteriesEnabled = 4;
        } else if(energy >= 65) {
            batteriesEnabled = 3;
        } else if(energy >= 40) {
            batteriesEnabled = 2;
        } else if(energy >= 15) {
            batteriesEnabled = 1;
        }
        else {
            batteriesEnabled = 0;
        }

        for(int i = 0; i < batteryImages.Count; i++) {
            if(i < batteriesEnabled) {
                batteryImages[i].enabled = true;
            } else {
                batteryImages[i].enabled = false;
            }
            
        }
    }

    private IEnumerator RedBattery() {
        //change each battery image to red if energy has gone down more than 1 last frame
        for(int i = 0; i < batteryImages.Count; i++) {
            batteryImages[i].color = Color.red;
        }
        yield return new WaitForSeconds(2);
        for(int i = 0; i < batteryImages.Count; i++) {
            batteryImages[i].color = Color.white;
        }
        
    }

}
