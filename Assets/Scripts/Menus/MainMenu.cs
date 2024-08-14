using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MainMenu : MonoBehaviour
{
    public GameObject DefaultMenu;
    public GameObject LevelSelectionMenu;
    public GameObject Settings;
    public GameObject Stats;

    public Button ExitGame;
    public Button ChooseLevel;
    public Button SettingsButton;
    public Button StatsButton;

    public AudioClip menuTheme;

    public List<GameObject> ObjToBeInitialized;

    // Start is called before the first frame update
    void Start()
    {
        //change brightness
        GlobalPostProcessingSettings.Instance.SetGammaAlpha(PlayerPrefs.GetInt("Brightness", 50));
        //change audio settings
        AudioListener.volume = PlayerPrefs.GetInt("MasterVolume", 50) / 100f;

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

        //initialize all objects
        foreach(GameObject obj in ObjToBeInitialized) {
            obj.SetActive(true);
        }

        foreach(GameObject obj in ObjToBeInitialized) {
            obj.SetActive(false);
        }

        DefaultMenu.SetActive(true);
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
