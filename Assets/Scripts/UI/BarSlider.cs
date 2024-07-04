using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BarSlider : MonoBehaviour
{
    
    float value;
    public TMP_Text valueText;
    public bool integer = true;


    public void SetValue(float newValue) {
        this.GetComponentInChildren<Slider>().value = newValue;
        value = newValue;
        valueText.text = newValue.ToString();
    }

    public float GetValue() {
        return value;
    }

    public int GetIntValue() {
        return (int)value;
    }

    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        float val = this.GetComponentInChildren<Slider>().value;
        //cast to int if integer is true
        if (integer) {
            val = Mathf.Round(val);
        }
        value = val;
        valueText.text = val.ToString();
    }
}
