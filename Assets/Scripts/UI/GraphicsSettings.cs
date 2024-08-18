using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GraphicsSettings : MonoBehaviour
{
    public GameObject Resolution;
    public GameObject Brightness;
    public GameObject AspectRatio;
    public GameObject DisplayMode;
    public GameObject Quality;

    public Image selectedOptionOutline;
    private GameObject selectedOption;
    public TMP_Text selectedOptionDescription;

    public List<String> resolutions = new List<String> {
        "1280x720",
        "1280x800",
        "1366x768",
        "1440x900",
        "1600x900",
        "1680x1050",
        "1920x1080",
        "1920x1200",
        "2560x1440",
        "2560x1600",
        "3840x2160",
        "5120x1440"
    };

    public List<String> qualities = new List<String> {
        "High",
        "Medium",
        "Low",
    };

    public List<String> displayModes = new List<String> {
        "Windowed",
        "Borderless Window",
        "Fullscreen"
    };

    public List<String> aspectRatios = new List<String> {
        "4:3",
        "16:9",
        "16:10",
        "21:9"
    };

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
        if(!PlayerPrefs.HasKey("Quality")) {
            PlayerPrefs.SetString("Quality", "High");
        }

        ChangeSelection(Brightness);

        EventTrigger trigger = Resolution.AddComponent<EventTrigger>();
        // Create a new entry for the click event
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerEnter;
        // Add a callback to the entry
        entry.callback.AddListener(delegate { ChangeSelection(Resolution); });
        // Add the entry to the trigger
        trigger.triggers.Add(entry);

        trigger = Brightness.AddComponent<EventTrigger>();
        entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerEnter;
        entry.callback.AddListener(delegate { ChangeSelection(Brightness); });
        trigger.triggers.Add(entry);


        trigger = AspectRatio.AddComponent<EventTrigger>();
        entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerEnter;
        entry.callback.AddListener(delegate { ChangeSelection(AspectRatio); });
        trigger.triggers.Add(entry);

        trigger = DisplayMode.AddComponent<EventTrigger>();
        entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerEnter;
        entry.callback.AddListener(delegate { ChangeSelection(DisplayMode); });
        trigger.triggers.Add(entry);

        trigger = Quality.AddComponent<EventTrigger>();
        entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerEnter;
        entry.callback.AddListener(delegate { ChangeSelection(Quality); });
        trigger.triggers.Add(entry);
    }

    private void ChangeSelection(GameObject selectedOption) {
        this.selectedOption = selectedOption;
        selectedOptionOutline.transform.position = new Vector3(selectedOption.transform.position.x, selectedOption.transform.position.y, selectedOption.transform.position.z);
        if(selectedOption == Resolution) {
            selectedOptionDescription.text = "Change the resolution of the game window";
        } else if(selectedOption == Brightness) {
            selectedOptionDescription.text = "Change the brightness of the game";
        } else if(selectedOption == AspectRatio) {
            selectedOptionDescription.text = "Change the aspect ratio";
        } else if(selectedOption == DisplayMode) {
            selectedOptionDescription.text = "Change the display mode";
        } else if(selectedOption == Quality) {
            selectedOptionDescription.text = "Change the graphics quality of the game. This can affect performance";
        }
        
    }

    public void SetValues() {
        Resolution.GetComponentInChildren<Dropdown>().InitDropdown(resolutions, PlayerPrefs.GetString("Resolution"));
        Brightness.GetComponentInChildren<BarSlider>().SetValue(PlayerPrefs.GetInt("Brightness"));
        AspectRatio.GetComponentInChildren<Dropdown>().InitDropdown(aspectRatios,PlayerPrefs.GetString("AspectRatio"));
        DisplayMode.GetComponentInChildren<Dropdown>().InitDropdown(displayModes, PlayerPrefs.GetString("DisplayMode"));
        Quality.GetComponentInChildren<Dropdown>().InitDropdown(qualities, PlayerPrefs.GetString("Quality"));
    }

    void Start() {
        SetValues();
    }

    public void SaveSettings() {
        PlayerPrefs.SetString("Resolution", Resolution.GetComponentInChildren<TMP_Dropdown>().captionText.text);
        PlayerPrefs.SetInt("Brightness", Brightness.GetComponentInChildren<BarSlider>().GetIntValue());
        PlayerPrefs.SetString("AspectRatio", AspectRatio.GetComponentInChildren<TMP_Dropdown>().captionText.text);
        PlayerPrefs.SetString("DisplayMode", DisplayMode.GetComponentInChildren<TMP_Dropdown>().captionText.text);
        PlayerPrefs.SetString("Quality", Quality.GetComponentInChildren<TMP_Dropdown>().captionText.text);

        FullScreenMode fsMode = FullScreenMode.ExclusiveFullScreen;
        
        if(PlayerPrefs.GetString("DisplayMode") == "Fullscreen") {
            Screen.fullScreen = true;
            Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
            fsMode = FullScreenMode.ExclusiveFullScreen;
        }
        else if(PlayerPrefs.GetString("DisplayMode") == "Borderless Window") {
            Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
            fsMode = FullScreenMode.FullScreenWindow;
        } 
        else if (PlayerPrefs.GetString("DisplayMode") == "Windowed") {
            Screen.fullScreenMode = FullScreenMode.Windowed;
            fsMode = FullScreenMode.Windowed;
        }

        QualitySettings.SetQualityLevel(qualities.IndexOf(PlayerPrefs.GetString("Quality")));

        string[] res = PlayerPrefs.GetString("Resolution").Split('x');
        Screen.SetResolution(int.Parse(res[0]), int.Parse(res[1]), fsMode);
    }

    public void RevertChanges() {
        Awake();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
