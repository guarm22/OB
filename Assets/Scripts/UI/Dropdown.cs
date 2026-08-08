using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Dropdown : MonoBehaviour {
    
    public List<String> options;
    public TMP_Dropdown dropdown;

    public bool interactableDefault = true;

    public void Start() {
        dropdown = GetComponent<TMP_Dropdown>();
        
    }

    public void SetInteractable(bool interactable) {
        interactableDefault = interactable;
    }

    public void InitDropdown(List<String> options, string currentOption) {
        this.options = options;
        TMP_Dropdown dropdown = GetComponent<TMP_Dropdown>();
        dropdown.ClearOptions();
        dropdown.AddOptions(options);
        dropdown.value = options.IndexOf(currentOption);
        dropdown.interactable = interactableDefault;
        if(!interactableDefault) {
            dropdown.GetComponentInChildren<TMP_Text>().color = new Color(172/255f, 187/255f, 207/255f, 0.5f);
        }
        else {
            dropdown.GetComponentInChildren<TMP_Text>().color = new Color(172/255f, 187/255f, 207/255f, 1);
        }
    }
}
