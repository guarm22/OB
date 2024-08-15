using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ControlSettings : MonoBehaviour {
    public Button keybindButton;
    public GameObject MouseSens;
    public GameObject MouseAccel;
    public GameObject InvertMouse;
    public GameObject KeybindParent;
    public GameObject KeybindsMenu;
    public List<GameObject> otherMenus;

    public Image selectedOptionOutline;
    private GameObject selectedOption;
    public TMP_Text selectedOptionDescription;

    void Awake() {
        if(!PlayerPrefs.HasKey("MouseSens")) {
            PlayerPrefs.SetFloat("MouseSens", 2);
        }

        if(!PlayerPrefs.HasKey("MouseAccel")) {
            PlayerPrefs.SetString("MouseAccel", "OFF");
        }

        if(!PlayerPrefs.HasKey("InvertMouse")) {
            PlayerPrefs.SetString("InvertMouse", "OFF");
        }

        keybindButton.onClick.AddListener(() => {
            otherMenus.ForEach(m => m.SetActive(false));
            KeybindsMenu.SetActive(true);
            KeybindMenu.Instance.isOpen = true;
            gameObject.SetActive(false);
        });

        SetValues();

        ChangeSelection(KeybindParent);

        EventTrigger trigger = MouseSens.AddComponent<EventTrigger>();
        // Create a new entry for the click event
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerEnter;
        // Add a callback to the entry
        entry.callback.AddListener(delegate { ChangeSelection(MouseSens); });
        // Add the entry to the trigger
        trigger.triggers.Add(entry);

        trigger = MouseAccel.AddComponent<EventTrigger>();
        entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerEnter;
        entry.callback.AddListener(delegate { ChangeSelection(MouseAccel); });
        trigger.triggers.Add(entry);


        trigger = InvertMouse.AddComponent<EventTrigger>();
        entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerEnter;
        entry.callback.AddListener(delegate { ChangeSelection(InvertMouse); });
        trigger.triggers.Add(entry);

        trigger = KeybindParent.AddComponent<EventTrigger>();
        entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerEnter;
        entry.callback.AddListener(delegate { ChangeSelection(KeybindParent); });
        trigger.triggers.Add(entry);
    }

    private void ChangeSelection(GameObject selectedOption) {
        this.selectedOption = selectedOption;
        selectedOptionOutline.transform.position = new Vector3(selectedOption.transform.position.x, selectedOption.transform.position.y, selectedOption.transform.position.z);
        if(selectedOption == MouseSens) {
            selectedOptionDescription.text = "Controls the sensitivity of the mouse";
        } else if(selectedOption == MouseAccel) {
            selectedOptionDescription.text = "Controls mouse acceleration";
        } else if(selectedOption == InvertMouse) {
            selectedOptionDescription.text = "Invert Mouse";
        } else if(selectedOption == KeybindParent) {
            selectedOptionDescription.text = "Click to open the keybind menu";
        }
    }

    public void SetValues() {
        MouseSens.GetComponentInChildren<BarSlider>().SetValue(PlayerPrefs.GetFloat("MouseSens"));
        MouseAccel.GetComponentInChildren<SingleChoiceSection>().SetChoice(PlayerPrefs.GetString("MouseAccel"));
        InvertMouse.GetComponentInChildren<SingleChoiceSection>().SetChoice(PlayerPrefs.GetString("InvertMouse"));
    }

    public void SaveSettings() {
        PlayerPrefs.SetFloat("MouseSens", MouseSens.GetComponentInChildren<BarSlider>().GetValue());
        PlayerPrefs.SetString("MouseAccel", MouseAccel.GetComponentInChildren<SingleChoiceSection>().GetCurrentChoice());
        PlayerPrefs.SetString("InvertMouse", InvertMouse.GetComponentInChildren<SingleChoiceSection>().GetCurrentChoice());
    }

    public void RevertChanges() {
        Awake();
    }
}
