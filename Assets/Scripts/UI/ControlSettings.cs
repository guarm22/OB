using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControlSettings : MonoBehaviour {
    public Button keybindButton;
    public GameObject MouseSens;
    public GameObject MouseAccel;
    public GameObject InvertMouse;
    public GameObject KeybindsMenu;

    public List<GameObject> otherMenus;

    void Awake() {
        if(!PlayerPrefs.HasKey("MouseSens")) {
            PlayerPrefs.SetFloat("MouseSens", 2);
        }

        if(!PlayerPrefs.HasKey("MouseAccel")) {
            PlayerPrefs.SetString("MouseAccel", "OFF");
        }

        if(!PlayerPrefs.HasKey("InvertMouse")) {
            PlayerPrefs.SetString("InvertMouse", "OFF");
        }

        keybindButton.onClick.AddListener(() => {
            otherMenus.ForEach(m => m.SetActive(false));
            KeybindsMenu.SetActive(true);
            KeybindMenu.Instance.isOpen = true;
            gameObject.SetActive(false);
        });

        SetValues();
    }

    public void SetValues() {
        MouseSens.GetComponentInChildren<BarSlider>().SetValue(PlayerPrefs.GetFloat("MouseSens"));
        MouseAccel.GetComponentInChildren<SingleChoiceSection>().SetChoice(PlayerPrefs.GetString("MouseAccel"));
        InvertMouse.GetComponentInChildren<SingleChoiceSection>().SetChoice(PlayerPrefs.GetString("InvertMouse"));
    }

    public void SaveSettings() {
        PlayerPrefs.SetFloat("MouseSens", MouseSens.GetComponentInChildren<BarSlider>().GetValue());
        PlayerPrefs.SetString("MouseAccel", MouseAccel.GetComponentInChildren<SingleChoiceSection>().GetCurrentChoice());
        PlayerPrefs.SetString("InvertMouse", InvertMouse.GetComponentInChildren<SingleChoiceSection>().GetCurrentChoice());
    }

    public void RevertChanges() {
        Awake();
    }
}
