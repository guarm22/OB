using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnergyUI : MonoBehaviour
{
    public Text EnergyText;
    // Update is called once per frame
    void Update() {
        EnergyText.text = "Energy = " + Mathf.RoundToInt(GameSystem.Instance.CurrentEnergy).ToString() + "%";
    }
}
