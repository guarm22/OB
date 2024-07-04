using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GraphicsSettings : MonoBehaviour
{
    public GameObject Resolution;
    public GameObject Brightness;
    public GameObject AspectRatio;
    public GameObject DisplayMode;

    void Awake() {
        if(!PlayerPrefs.HasKey("Resolution")) {
            PlayerPrefs.SetString("Resolution", Display.main.systemWidth + "x" + Display.main.systemHeight);
        }
        if(!PlayerPrefs.HasKey("Brightness")) {
            PlayerPrefs.SetInt("Brightness", 50);
        }
        if(!PlayerPrefs.HasKey("AspectRatio")) {
            PlayerPrefs.SetString("AspectRatio", "16:9");
        }
        if(!PlayerPrefs.HasKey("DisplayMode")) {
            PlayerPrefs.SetString("DisplayMode", "Borderless Window");
        }
    }

    public void SetValues() {
        Resolution.GetComponentInChildren<Dropdown>().SetOption(PlayerPrefs.GetString("Resolution"));
        Brightness.GetComponentInChildren<BarSlider>().SetValue(PlayerPrefs.GetInt("Brightness"));
        AspectRatio.GetComponentInChildren<Dropdown>().SetOption(PlayerPrefs.GetString("AspectRatio"));
        DisplayMode.GetComponentInChildren<Dropdown>().SetOption(PlayerPrefs.GetString("DisplayMode"));
    }

    void Start() {
        SetValues();
    }

    public void SaveSettings() {
        PlayerPrefs.SetString("Resolution", Resolution.GetComponentInChildren<Dropdown>().GetCurrentOption());
        PlayerPrefs.SetInt("Brightness", Brightness.GetComponentInChildren<BarSlider>().GetIntValue());
        PlayerPrefs.SetString("AspectRatio", AspectRatio.GetComponentInChildren<Dropdown>().GetCurrentOption());
        PlayerPrefs.SetString("DisplayMode", DisplayMode.GetComponentInChildren<Dropdown>().GetCurrentOption());
    }

    public void RevertChanges() {
        Awake();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
