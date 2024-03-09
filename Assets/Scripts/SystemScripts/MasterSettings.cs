using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

public class MasterSettings : MonoBehaviour
{
    public float volume;
    public float originalVolume;
    public static MasterSettings instance;
    public Slider volumeSlider;
    public Button volumeTest;
    public AudioClip testClip;

    public Button applySettings;
    public Button backButton;

    void Start()
    {
        instance = this;
        LoadSettings();
        SetUI();
        originalVolume = volume;

        volumeTest.onClick.AddListener(VolumeTest);
        applySettings.onClick.AddListener(ApplySettings);
        backButton.onClick.AddListener(Back);
    }

    private void SetUI() {
        volumeSlider.value = volume;
    }

    public void LoadSettings() {
        volume = PlayerPrefs.GetFloat("Volume", 0.5f);
        AudioListener.volume = volume;
        volumeSlider.value = volume;
    }

    void VolumeTest() {
        AudioListener.volume = volume;
        AudioSource.PlayClipAtPoint(testClip, Camera.main.transform.position);
    }

    void ApplySettings() {
        volume = volumeSlider.value;
        AudioListener.volume = volume;
        PlayerPrefs.SetFloat("Volume", volume);
        originalVolume = volume;
    }

    void Back() {
        volume = originalVolume;
        volumeSlider.value = volume;
        AudioListener.volume = volume;
    }

   
    void Update()
    {
        volume = volumeSlider.value;
    }
}
