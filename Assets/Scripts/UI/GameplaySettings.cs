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
            selectedOptionDescription.text = "In game field of view";
        } else if(selectedOption == VisualHints) {
            selectedOptionDescription.text = "Determines if lights will flicker in game as a hint";
        } else if(selectedOption == profile) {
            selectedOptionDescription.text = "Profile to save stats, achievements, and settings to";
        }
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
