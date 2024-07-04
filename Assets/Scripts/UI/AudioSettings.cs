using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSettings : MonoBehaviour
{
    public GameObject masterVolume;
    public GameObject musicVolume;
    public GameObject sfxVolume;
    public GameObject AmbienceVolume;
    public GameObject AlertVolume;
    public GameObject CreatureVolume;
    public GameObject WeatherVolume;

    void Awake() {
        if(!PlayerPrefs.HasKey("MasterVolume")) {
            PlayerPrefs.SetInt("MasterVolume", 100);
        }
        if(!PlayerPrefs.HasKey("MusicVolume")) {
            PlayerPrefs.SetInt("MusicVolume", 100);
        }

        if(!PlayerPrefs.HasKey("SFXVolume")) {
            PlayerPrefs.SetInt("SFXVolume", 100);
        }
        if(!PlayerPrefs.HasKey("AmbienceVolume")) {
            PlayerPrefs.SetInt("AmbienceVolume", 100);
        }
        if(!PlayerPrefs.HasKey("AlertVolume")) {
            PlayerPrefs.SetInt("AlertVolume", 100);
        }
        if(!PlayerPrefs.HasKey("CreatureVolume")) {
            PlayerPrefs.SetInt("CreatureVolume", 100);
        }
        if(!PlayerPrefs.HasKey("WeatherVolume")) {
            PlayerPrefs.SetInt("WeatherVolume", 100);
        }

        SetValues();
    }

    public void SetValues() {
        masterVolume.GetComponentInChildren<BarSlider>().SetValue(PlayerPrefs.GetInt("MasterVolume"));
        musicVolume.GetComponentInChildren<BarSlider>().SetValue(PlayerPrefs.GetInt("MusicVolume"));
        sfxVolume.GetComponentInChildren<BarSlider>().SetValue(PlayerPrefs.GetInt("SFXVolume"));
        AmbienceVolume.GetComponentInChildren<BarSlider>().SetValue(PlayerPrefs.GetInt("AmbienceVolume"));
        AlertVolume.GetComponentInChildren<BarSlider>().SetValue(PlayerPrefs.GetInt("AlertVolume"));
        CreatureVolume.GetComponentInChildren<BarSlider>().SetValue(PlayerPrefs.GetInt("CreatureVolume"));
        WeatherVolume.GetComponentInChildren<BarSlider>().SetValue(PlayerPrefs.GetInt("WeatherVolume"));
    }

    public void SaveSettings() {
        PlayerPrefs.SetInt("MasterVolume", masterVolume.GetComponentInChildren<BarSlider>().GetIntValue());
        PlayerPrefs.SetInt("MusicVolume", musicVolume.GetComponentInChildren<BarSlider>().GetIntValue());
        PlayerPrefs.SetInt("SFXVolume", sfxVolume.GetComponentInChildren<BarSlider>().GetIntValue());
        PlayerPrefs.SetInt("AmbienceVolume", AmbienceVolume.GetComponentInChildren<BarSlider>().GetIntValue());
        PlayerPrefs.SetInt("AlertVolume", AlertVolume.GetComponentInChildren<BarSlider>().GetIntValue());
        PlayerPrefs.SetInt("CreatureVolume", CreatureVolume.GetComponentInChildren<BarSlider>().GetIntValue());
        PlayerPrefs.SetInt("WeatherVolume", WeatherVolume.GetComponentInChildren<BarSlider>().GetIntValue());

    }

    public void RevertChanges() {
        Awake();
    }
}
