using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplaySettings : MonoBehaviour
{
    
    public GameObject VisualHints;
    public GameObject FOV;

    void Start() {
        if(!PlayerPrefs.HasKey("VisualHints")) {
            PlayerPrefs.SetString("VisualHints", "YES");
        }

        if(!PlayerPrefs.HasKey("FOV")) {
            PlayerPrefs.SetInt("FOV", 75);
        }

        VisualHints.GetComponentInChildren<SingleChoiceSection>().SetChoice(PlayerPrefs.GetString("VisualHints"));
        FOV.GetComponentInChildren<BarSlider>().SetValue(PlayerPrefs.GetInt("FOV"));
    }

    public void RevertChanges() {
        Start();
    }

    public void SaveSettings() {
        PlayerPrefs.SetString("VisualHints", VisualHints.GetComponentInChildren<SingleChoiceSection>().GetCurrentChoice());
        PlayerPrefs.SetInt("FOV", (int)FOV.GetComponentInChildren<BarSlider>().GetValue());
    }

    // Update is called once per frame
    void Update() {

    }
}
