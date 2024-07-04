using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MainMenu : MonoBehaviour
{
    public GameObject DefaultMenu;

    public GameObject LevelSelectionMenu;

    public GameObject Settings;

    public GameObject Stats;
    public GameObject GameSetting;
    public GameObject AdvancedSettings;

    public Button ExitGame;
    public Button ChooseLevel;
    public Button SettingsButton;
    public Button StatsButton;
    public Button AdvancedDifficulty;
    public Button AdvancedBack;

    public AudioClip menuTheme;

    public List<GameObject> ObjToBeInitialized;

    // Start is called before the first frame update
    void Start()
    {
        //change brightness
       // GlobalPostProcessingSettings.Instance.SetGammaAlpha(PlayerPrefs.GetInt("Brightness"));
        //change audio settings
        AudioListener.volume = PlayerPrefs.GetInt("MasterVolume") / 100f;

        //play menu theme
        this.GetComponent<AudioSource>().clip = menuTheme;
        this.GetComponent<AudioSource>().loop = true;
        //change volume
        this.GetComponent<AudioSource>().volume = PlayerPrefs.GetInt("MusicVolume") / 100f;
        this.GetComponent<AudioSource>().Play();
        ExitGame.onClick.AddListener(ExitButtonEvent);
        ChooseLevel.onClick.AddListener(LevelSelectionEvent);
        SettingsButton.onClick.AddListener(SettingsButtonEvent);
        StatsButton.onClick.AddListener(StatsButtonEvent);

        AdvancedDifficulty.onClick.AddListener(AdvancedDifficultyEvent);
        AdvancedBack.onClick.AddListener(AdvancedBackEvent);

        //initialize all objects
        foreach(GameObject obj in ObjToBeInitialized) {
            obj.SetActive(true);
        }

        foreach(GameObject obj in ObjToBeInitialized) {
            obj.SetActive(false);
        }

        DefaultMenu.SetActive(true);
    }

    private void AdvancedBackEvent() {
        AdvancedSettings.SetActive(false);
        AdvancedBack.gameObject.SetActive(false);
        AdvancedDifficulty.gameObject.SetActive(true);
    }
    private void AdvancedDifficultyEvent() {
        AdvancedSettings.SetActive(true);
        AdvancedBack.gameObject.SetActive(true);
        AdvancedDifficulty.gameObject.SetActive(false);
    }
    private void LevelSelectionEvent() {
        DefaultMenu.SetActive(false);
        LevelSelectionMenu.SetActive(true);
    }

    private void ExitButtonEvent() {
        Application.Quit();
    }
    private void SettingsButtonEvent() {
        DefaultMenu.SetActive(false);
        Settings.gameObject.SetActive(true);
        Settings.GetComponent<SettingsMenu>().Open();
    }
    private void StatsButtonEvent() {
        DefaultMenu.SetActive(false);
        Stats.SetActive(true);
    }

}
