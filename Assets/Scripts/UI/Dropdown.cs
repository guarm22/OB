using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Dropdown : MonoBehaviour {
    
    public List<String> options;
    public TMP_Dropdown dropdown;


    public void InitDropdown(List<String> options, string currentOption) {
        this.options = options;
        TMP_Dropdown dropdown = GetComponent<TMP_Dropdown>();
        dropdown.ClearOptions();
        dropdown.AddOptions(options);
        dropdown.value = options.IndexOf(currentOption);
    }
}
