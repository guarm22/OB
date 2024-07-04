using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplaySettings : MonoBehaviour
{
    
    public GameObject VisualHints;
    public GameObject FOV;

    void Awake() {
        if(!PlayerPrefs.HasKey("VisualHints")) {
            PlayerPrefs.SetString("VisualHints", "YES");
        }

        if(!PlayerPrefs.HasKey("FOV")) {
            PlayerPrefs.SetInt("FOV", 75);
        }

        SetValues();
    }

    public void SetValues() {
        VisualHints.GetComponentInChildren<SingleChoiceSection>().SetChoice(PlayerPrefs.GetString("VisualHints"));
        FOV.GetComponentInChildren<BarSlider>().SetValue(PlayerPrefs.GetInt("FOV"));
    }

    public void RevertChanges() {
        Awake();
    }

    public void SaveSettings() {
        PlayerPrefs.SetString("VisualHints", VisualHints.GetComponentInChildren<SingleChoiceSection>().GetCurrentChoice());
        PlayerPrefs.SetInt("FOV", (int)FOV.GetComponentInChildren<BarSlider>().GetValue());
    }
}
