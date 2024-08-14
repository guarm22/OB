using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameplaySettings : MonoBehaviour
{
    
    public GameObject VisualHints;
    public GameObject FOV;

    public GameObject profile;
    public Button newProfileButton;

    public GameObject newProfilePanel;
    public TMP_InputField newProfileName;
    public Button createProfileButton;
    public Button cancelProfileButton;

    void Awake() {
        if(!PlayerPrefs.HasKey("VisualHints")) {
            PlayerPrefs.SetString("VisualHints", "YES");
        }

        if(!PlayerPrefs.HasKey("currentProfile")) {
            PlayerPrefs.SetString("currentProfile", "default");
        }

        List<string> profiles = PFileUtil.GetAllProfiles();
        profile.GetComponentInChildren<Dropdown>().InitDropdown(profiles, PlayerPrefs.GetString("currentProfile"));

        newProfileButton.onClick.AddListener(CreateNewProfile);

        createProfileButton.onClick.AddListener(() => {
            PFileUtil.CreateDirectoryForProfile(newProfileName.text);
            profile.GetComponentInChildren<Dropdown>().InitDropdown(PFileUtil.GetAllProfiles(), PlayerPrefs.GetString("currentProfile"));
            newProfilePanel.SetActive(false);
        });

        cancelProfileButton.onClick.AddListener(() => {
            newProfilePanel.SetActive(false);
        });

        if(!PlayerPrefs.HasKey("FOV")) {
            PlayerPrefs.SetInt("FOV", 75);
        }

        SetValues();
    }

    private void CreateNewProfile() {
        newProfilePanel.SetActive(true);
    }

    public void SetValues() {
        VisualHints.GetComponentInChildren<SingleChoiceSection>().SetChoice(PlayerPrefs.GetString("VisualHints"));
        FOV.GetComponentInChildren<BarSlider>().SetValue(PlayerPrefs.GetInt("FOV"));
        profile.GetComponentInChildren<Dropdown>().InitDropdown(PFileUtil.GetAllProfiles(), PlayerPrefs.GetString("currentProfile"));
    }

    public void RevertChanges() {
        Awake();
    }

    public void SaveSettings() {
        if(PlayerPrefs.GetString("currentProfile") != profile.GetComponentInChildren<TMP_Dropdown>().captionText.text) {
            
        }

        PlayerPrefs.SetString("VisualHints", VisualHints.GetComponentInChildren<SingleChoiceSection>().GetCurrentChoice());
        PlayerPrefs.SetInt("FOV", (int)FOV.GetComponentInChildren<BarSlider>().GetValue());
        PlayerPrefs.SetString("currentProfile", profile.GetComponentInChildren<TMP_Dropdown>().captionText.text);
    }

    void Update() {
        if(Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.E)) {
            newProfilePanel.SetActive(false);
        }
    }
}
