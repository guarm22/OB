using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MasterSettings : MonoBehaviour
{
    public float volume;
    public float originalVolume;
    public float FOV;
    public static MasterSettings instance;
    public Slider volumeSlider;
    public Button volumeTest;
    public TMP_Text volumeText;
    public AudioClip testClip;

    public Slider FOVSlider;
    public TMP_Text FOVText;

    public Slider MouseSensitivitySlider;
    public TMP_Text MouseSensitivityText;
    public float MouseSensitivity;

    public Button applySettings;
    public Button backButton;

    public TMP_Text appliedText;

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
        FOVSlider.value = FOV;
        MouseSensitivitySlider.value = MouseSensitivity;
    }

    private void LoadSettings() {
        volume = PlayerPrefs.GetFloat("Volume", 0.5f);
        FOV = PlayerPrefs.GetFloat("FOV", 60);    
        MouseSensitivity = PlayerPrefs.GetFloat("MouseSensitivity", 2);
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

        FOV = FOVSlider.value;
        PlayerPrefs.SetFloat("FOV", FOV);

        MouseSensitivity = MouseSensitivitySlider.value;
        PlayerPrefs.SetFloat("MouseSensitivity", MouseSensitivity);

        StartCoroutine(AppliedTextFade());
    }

    void Back() {
        volume = originalVolume;
        volumeSlider.value = volume;
        AudioListener.volume = volume;
    }

    private IEnumerator AppliedTextFade() {
        appliedText.gameObject.SetActive(true);
        yield return new WaitForSeconds(3f);
        appliedText.gameObject.SetActive(false);
    }

   
    void Update()
    {
        volume = volumeSlider.value;
        volumeText.text = (int)(volume * 100) + "%";

        FOV = FOVSlider.value;
        FOVText.text = (int)FOV + "";

        MouseSensitivity = MouseSensitivitySlider.value;
        MouseSensitivityText.text = MouseSensitivity.ToString("F1");
    }
}
