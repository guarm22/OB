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
    public GameObject DisplayMode;
    public GameObject Quality;
    public GameObject Monitor;

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

    public List<String> monitors = new List<String>();

    void Awake() {
        if(!PlayerPrefs.HasKey("Resolution")) {
            PlayerPrefs.SetString("Resolution", Display.main.systemWidth + "x" + Display.main.systemHeight);
        }
        if(!PlayerPrefs.HasKey("Brightness")) {
            PlayerPrefs.SetInt("Brightness", 50);
        }
        if(!PlayerPrefs.HasKey("DisplayMode")) {
            PlayerPrefs.SetString("DisplayMode", "Borderless Window");
        }
        if(!PlayerPrefs.HasKey("Quality")) {
            PlayerPrefs.SetString("Quality", "High");
        }
        if(!PlayerPrefs.HasKey("Monitor")) {
            PlayerPrefs.SetString("Monitor", "Monitor 1");
        }

        GetMonitors();

        ChangeSelection(Brightness);
        Brightness.GetComponentInChildren<BarSlider>().SetValue(PlayerPrefs.GetInt("Brightness",50));
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

        trigger = Monitor.AddComponent<EventTrigger>();
        entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerEnter;
        entry.callback.AddListener(delegate { ChangeSelection(Monitor); });
        trigger.triggers.Add(entry);
    }

    private void GetMonitors() {
        int monitorCount = Display.displays.Length;
        monitors.Clear();
        for(int i = 0; i < monitorCount; i++) {
            monitors.Add("Monitor " + (i + 1));
        }
    }

    private void SwitchMonitor(int monitorIndex) {
        if(monitorIndex < Display.displays.Length) {
            Display.displays[monitorIndex].Activate();
            PlayerPrefs.SetString("Monitor", "Monitor " + (monitorIndex + 1));
        }
    }

    private void ChangeSelection(GameObject selectedOption) {
        this.selectedOption = selectedOption;
        selectedOptionOutline.transform.position = new Vector3(selectedOption.transform.position.x, selectedOption.transform.position.y, selectedOption.transform.position.z);
        if(selectedOption == Resolution) {
            selectedOptionDescription.text = "Change the resolution of the game window";
        } else if(selectedOption == Brightness) {
            selectedOptionDescription.text = "Change the brightness of the game";
        } else if(selectedOption == DisplayMode) {
            selectedOptionDescription.text = "Change the display mode";
        } else if(selectedOption == Quality) {
            selectedOptionDescription.text = "Change the graphics quality of the game. This can affect performance";
        } else if(selectedOption == Monitor) {
            selectedOptionDescription.text = "Change the monitor the game is displayed on";
        }
         
        
    }

    public void SetValues() {
        Resolution.GetComponentInChildren<Dropdown>().InitDropdown(resolutions, PlayerPrefs.GetString("Resolution"));
        Brightness.GetComponentInChildren<BarSlider>().SetValue(PlayerPrefs.GetInt("Brightness"));
        DisplayMode.GetComponentInChildren<Dropdown>().InitDropdown(displayModes, PlayerPrefs.GetString("DisplayMode"));
        Quality.GetComponentInChildren<Dropdown>().InitDropdown(qualities, PlayerPrefs.GetString("Quality"));
        Monitor.GetComponentInChildren<Dropdown>().InitDropdown(monitors, PlayerPrefs.GetString("Monitor"));
    }

    void Start() {
        SetValues();
    }

    public void SaveSettings() {
        PlayerPrefs.SetString("Resolution", Resolution.GetComponentInChildren<TMP_Dropdown>().captionText.text);
        PlayerPrefs.SetInt("Brightness", Brightness.GetComponentInChildren<BarSlider>().GetIntValue());
        PlayerPrefs.SetString("DisplayMode", DisplayMode.GetComponentInChildren<TMP_Dropdown>().captionText.text);
        PlayerPrefs.SetString("Quality", Quality.GetComponentInChildren<TMP_Dropdown>().captionText.text);
        PlayerPrefs.SetString("Monitor", Monitor.GetComponentInChildren<TMP_Dropdown>().captionText.text);
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

        //SwitchMonitor(monitors.IndexOf(PlayerPrefs.GetString("Monitor")));
    }

    public void RevertChanges() {
        Awake();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
