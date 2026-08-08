using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
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

    public Image selectedOptionOutline;
    private GameObject selectedOption;
    public TMP_Text selectedOptionDescription;

    public GameObject AudioDivergences;

    void Awake() {
        if(!PlayerPrefs.HasKey("VisualHints")) {
            PlayerPrefs.SetString("VisualHints", "YES");
        }

        if(!PlayerPrefs.HasKey("currentProfile")) {
            PlayerPrefs.SetString("currentProfile", "default");
        }
        if(!PlayerPrefs.HasKey("AudioDivergences")) {
            PlayerPrefs.SetString("AudioDivergences", "YES");
        }

        if(SettingsMenu.Instance != null && SettingsMenu.Instance.inLevel) {
            profile.GetComponentInChildren<Dropdown>().SetInteractable(false);
            newProfileButton.interactable = false;
            newProfileButton.GetComponentInChildren<TMP_Text>().color = new Color(172/255f, 187/255f, 207/255f, 0.5f);
            profile.transform.GetChild(1).GetComponent<TMP_Text>().color = new Color(172/255f, 187/255f, 207/255f, 1);
        }

        List<string> profiles = PFileUtil.GetAllProfiles();
        profile.GetComponentInChildren<Dropdown>().InitDropdown(profiles, PlayerPrefs.GetString("currentProfile", "default"));

        newProfileButton.onClick.AddListener(CreateNewProfile);

        createProfileButton.onClick.AddListener(() => {
            PFileUtil.CreateDirectoryForProfile(newProfileName.text);
            profile.GetComponentInChildren<Dropdown>().InitDropdown(PFileUtil.GetAllProfiles(), PlayerPrefs.GetString("currentProfile", "default"));
            newProfilePanel.SetActive(false);
        });

        cancelProfileButton.onClick.AddListener(() => {
            newProfilePanel.SetActive(false);
        });

        if(!PlayerPrefs.HasKey("FOV")) {
            PlayerPrefs.SetInt("FOV", 85);
        }

        SetValues();
        ChangeSelection(profile);

        EventTrigger trigger = FOV.AddComponent<EventTrigger>();
        // Create a new entry for the click event
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerEnter;
        // Add a callback to the entry
        entry.callback.AddListener(delegate { ChangeSelection(FOV); });
        // Add the entry to the trigger
        trigger.triggers.Add(entry);

        trigger = VisualHints.AddComponent<EventTrigger>();
        entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerEnter;
        entry.callback.AddListener(delegate { ChangeSelection(VisualHints); });
        trigger.triggers.Add(entry);
        if(SettingsMenu.Instance != null && SettingsMenu.Instance.inLevel) {
            VisualHints.GetComponentInChildren<SingleChoiceSection>().SetInteractable(false);
            VisualHints.transform.GetChild(1).GetComponent<TMP_Text>().color = new Color(172/255f, 187/255f, 207/255f, 1);
        }

        trigger = AudioDivergences.AddComponent<EventTrigger>();
        entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerEnter;
        entry.callback.AddListener(delegate { ChangeSelection(AudioDivergences); });
        trigger.triggers.Add(entry);
        if(SettingsMenu.Instance != null && SettingsMenu.Instance.inLevel) {
            AudioDivergences.GetComponentInChildren<SingleChoiceSection>().SetInteractable(false);
            AudioDivergences.transform.GetChild(1).GetComponent<TMP_Text>().color = new Color(172/255f, 187/255f, 207/255f, 1);
        }




        trigger = profile.AddComponent<EventTrigger>();
        entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerEnter;
        entry.callback.AddListener(delegate { ChangeSelection(profile); });
        trigger.triggers.Add(entry);
    }

    private void ChangeSelection(GameObject selectedOption) {
        this.selectedOption = selectedOption;
        selectedOptionOutline.transform.position = new Vector3(selectedOption.transform.position.x, selectedOption.transform.position.y, selectedOption.transform.position.z);
        if(selectedOption == FOV) {
            selectedOptionDescription.text = "In game field of view.";
        } else if(selectedOption == VisualHints) {
            selectedOptionDescription.text = "Determines if lights will flicker in game as a hint";
             if(SettingsMenu.Instance != null && SettingsMenu.Instance.inLevel) {
                selectedOptionDescription.text += ".\n\nCannot be changed in game.";
            }
        } else if(selectedOption == profile) {
            selectedOptionDescription.text = "Profile to save relics, achievements, and keybinds. Will not affect steam achievements/stats.";
            if(SettingsMenu.Instance != null && SettingsMenu.Instance.inLevel) {
                selectedOptionDescription.text += "\n\nCannot be changed in game.";
            }        
        } else if(selectedOption == AudioDivergences) {
            selectedOptionDescription.text = "Determines if the audio divergences will appear in game. For accessibility purposes.";
             if(SettingsMenu.Instance != null && SettingsMenu.Instance.inLevel) {
                selectedOptionDescription.text += "\n\nCannot be changed in game.";
            }
        }

    }

    private void CreateNewProfile() {
        newProfilePanel.SetActive(true);
    }

    public void SetValues() {
        VisualHints.GetComponentInChildren<SingleChoiceSection>().SetChoice(PlayerPrefs.GetString("VisualHints"));
        FOV.GetComponentInChildren<BarSlider>().SetValue(PlayerPrefs.GetInt("FOV"));
        profile.GetComponentInChildren<Dropdown>().InitDropdown(PFileUtil.GetAllProfiles(), PlayerPrefs.GetString("currentProfile"));
        AudioDivergences.GetComponentInChildren<SingleChoiceSection>().SetChoice(PlayerPrefs.GetString("AudioDivergences"));
    }

    public void RevertChanges() {
        Awake();
    }

    public void SaveSettings() {
        PlayerPrefs.SetString("VisualHints", VisualHints.GetComponentInChildren<SingleChoiceSection>().GetCurrentChoice());
        PlayerPrefs.SetInt("FOV", (int)FOV.GetComponentInChildren<BarSlider>().GetValue());
        PlayerPrefs.SetString("currentProfile", profile.GetComponentInChildren<TMP_Dropdown>().captionText.text);
        PlayerPrefs.SetString("AudioDivergences", AudioDivergences.GetComponentInChildren<SingleChoiceSection>().GetCurrentChoice());

        if(SC_FPSController.Instance !=null) {
            SC_FPSController.Instance.ChangeFOV(PlayerPrefs.GetInt("FOV"));
        }
    }

    void Update() {
        if(Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.E)) {
            newProfilePanel.SetActive(false);
        }
    }
}
