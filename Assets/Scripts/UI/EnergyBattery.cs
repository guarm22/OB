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

        int currentTicks = (int) Mathf.Floor(energy/25);
        int blinkingTick = (int) Mathf.Floor(energy/25);
        for(int i = 0; i < batteryImages.Count; i++) {
            if(i < currentTicks) {
                batteryImages[i].enabled = true;
            } else {
                batteryImages[i].enabled = false;
            }
            
        }
    }

    private IEnumerator BlinkImage(Image i) {
        i.enabled = true;
        yield return new WaitForSeconds(1f);
        i.enabled = false;
        yield return new WaitForSeconds(1f);
    }
}
