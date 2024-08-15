using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AudioSettings : MonoBehaviour
{
    public GameObject masterVolume;
    public GameObject musicVolume;
    public GameObject sfxVolume;
    public GameObject AmbienceVolume;
    public GameObject AlertVolume;
    public GameObject CreatureVolume;

    public Image selectedOptionOutline;
    private GameObject selectedOption;
    public TMP_Text selectedOptionDescription;

    void Awake() {
        if(!PlayerPrefs.HasKey("MasterVolume")) {
            PlayerPrefs.SetInt("MasterVolume", 50);
        }
        if(!PlayerPrefs.HasKey("MusicVolume")) {
            PlayerPrefs.SetInt("MusicVolume", 50);
        }

        if(!PlayerPrefs.HasKey("SFXVolume")) {
            PlayerPrefs.SetInt("SFXVolume", 50);
        }
        if(!PlayerPrefs.HasKey("AmbienceVolume")) {
            PlayerPrefs.SetInt("AmbienceVolume", 50);
        }
        if(!PlayerPrefs.HasKey("AlertVolume")) {
            PlayerPrefs.SetInt("AlertVolume", 50);
        }
        if(!PlayerPrefs.HasKey("CreatureVolume")) {
            PlayerPrefs.SetInt("CreatureVolume", 50);
        }
        SetValues();

        ChangeSelection(masterVolume);

        EventTrigger trigger = masterVolume.AddComponent<EventTrigger>();
        // Create a new entry for the click event
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerEnter;
        // Add a callback to the entry
        entry.callback.AddListener(delegate { ChangeSelection(masterVolume); });
        // Add the entry to the trigger
        trigger.triggers.Add(entry);

        trigger = musicVolume.AddComponent<EventTrigger>();
        entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerEnter;
        entry.callback.AddListener(delegate { ChangeSelection(musicVolume); });
        trigger.triggers.Add(entry);


        trigger = sfxVolume.AddComponent<EventTrigger>();
        entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerEnter;
        entry.callback.AddListener(delegate { ChangeSelection(sfxVolume); });
        trigger.triggers.Add(entry);

        trigger = AmbienceVolume.AddComponent<EventTrigger>();
        entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerEnter;
        entry.callback.AddListener(delegate { ChangeSelection(AmbienceVolume); });
        trigger.triggers.Add(entry);

        trigger = AlertVolume.AddComponent<EventTrigger>();
        entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerEnter;
        entry.callback.AddListener(delegate { ChangeSelection(AlertVolume); });
        trigger.triggers.Add(entry);

        trigger = CreatureVolume.AddComponent<EventTrigger>();
        entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerEnter;
        entry.callback.AddListener(delegate { ChangeSelection(CreatureVolume); });
        trigger.triggers.Add(entry);
    }

    private void ChangeSelection(GameObject selectedOption) {
        this.selectedOption = selectedOption;
        selectedOptionOutline.transform.position = new Vector3(selectedOption.transform.position.x, selectedOption.transform.position.y, selectedOption.transform.position.z);
        if(selectedOption == masterVolume) {
            selectedOptionDescription.text = "Overall volume of all game sounds";
        } else if(selectedOption == musicVolume) {
            selectedOptionDescription.text = "Controls the volume of any music (not including divergences)";
        } else if(selectedOption == sfxVolume) {
            selectedOptionDescription.text = "Controls the volume of sound effects";
        } else if(selectedOption == AmbienceVolume) {
            selectedOptionDescription.text = "Controls the volume of ambient sounds";
        }
        else if(selectedOption == AlertVolume) {
            selectedOptionDescription.text = "Controls the volume of the alert that plays when a divergence appears";
        }
        else if(selectedOption == CreatureVolume) {
            selectedOptionDescription.text = "Controls the volume of creature sounds";
        }
    }

    public void SetValues() {
        masterVolume.GetComponentInChildren<BarSlider>().SetValue(PlayerPrefs.GetInt("MasterVolume"));
        musicVolume.GetComponentInChildren<BarSlider>().SetValue(PlayerPrefs.GetInt("MusicVolume"));
        sfxVolume.GetComponentInChildren<BarSlider>().SetValue(PlayerPrefs.GetInt("SFXVolume"));
        AmbienceVolume.GetComponentInChildren<BarSlider>().SetValue(PlayerPrefs.GetInt("AmbienceVolume"));
        AlertVolume.GetComponentInChildren<BarSlider>().SetValue(PlayerPrefs.GetInt("AlertVolume"));
        CreatureVolume.GetComponentInChildren<BarSlider>().SetValue(PlayerPrefs.GetInt("CreatureVolume"));
    }

    public void SaveSettings() {
        PlayerPrefs.SetInt("MasterVolume", masterVolume.GetComponentInChildren<BarSlider>().GetIntValue());
        PlayerPrefs.SetInt("MusicVolume", musicVolume.GetComponentInChildren<BarSlider>().GetIntValue());
        PlayerPrefs.SetInt("SFXVolume", sfxVolume.GetComponentInChildren<BarSlider>().GetIntValue());
        PlayerPrefs.SetInt("AmbienceVolume", AmbienceVolume.GetComponentInChildren<BarSlider>().GetIntValue());
        PlayerPrefs.SetInt("AlertVolume", AlertVolume.GetComponentInChildren<BarSlider>().GetIntValue());
        PlayerPrefs.SetInt("CreatureVolume", CreatureVolume.GetComponentInChildren<BarSlider>().GetIntValue());
    }

    public void RevertChanges() {
        Awake();
    }
}
