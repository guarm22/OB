using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    ///Default UI that appears on the bottom right of the players screen
    public GameObject defaultUI;
    //UI that appears on the bottom right when the player presses tab

    public GameObject roomText;
    public GameObject escapeMenuUI;
    public GameObject EndGameUI;
    public GameObject debugUI;
    public GameObject defaultBottomRight;
    public TMP_Text tabText;
    public string targetTag = "Room";
    public string tutTag = "Tutorial";
    public bool inMenu = false;
    public static bool paused = false;
    public static PlayerUI Instance;
    public GameObject prompt;
    public bool havePausedAtleastOnce = false;
    public String currentRoom;
    public bool isGlitching = false;

    public AudioClip glitchSound;
    private AudioSource audioSource;
    private bool isReportTextGlitching = false;
    public GameObject crosshair;

    public GameObject reportDevice;
    public bool reportDeviceUp = false;

    public TMP_Text acquisitionText;

    void Start() {
        Instance = this;
        audioSource = this.gameObject.AddComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update() {
        if(Popup.Instance != null) {
            if(Popup.Instance.isPopupOpen ) {
                return;
            }
        }
        //now waiting for jumpscare to finish, if any
        if(GameSystem.Instance.GameOver) {
            EndingGame();
            return;
        }
        EscapeMenuControl();
        if(paused) {
            return;
        }
        SelectionMenu();
    }

    public void ChangePrompt(string text, bool activate) {
        prompt.SetActive(activate);
        prompt.GetComponent<TMP_Text>().text = text;
    }

    private IEnumerator GlitchReportText() {
        //for 0.5 seconds, change the tab text to random characters
        float elapsedTime = 0f;
        String originalText = tabText.text;
        while(elapsedTime < 0.8f) {
            elapsedTime += Time.deltaTime;
            char[] chars = tabText.text.ToCharArray();
            for(int i = 0; i < chars.Length; i++) {
                if(UnityEngine.Random.Range(0, 5) == 0) {
                    chars[i] = (char)UnityEngine.Random.Range(65, 91);
                }
            }
            tabText.text = new string(chars);
            yield return null;
        }
        tabText.text = originalText;
        isReportTextGlitching = false;
    }

    public IEnumerator Acquisition(String itemName, String postString = "View information about it in the Relic menu.", float waitTime = 8f) {
        if(acquisitionText.IsActive()) {yield return new WaitUntil(() =>!acquisitionText.IsActive());}

        acquisitionText.text = "Acquired: " + itemName + ". " + postString;
        acquisitionText.gameObject.SetActive(true);
        yield return new WaitForSeconds(waitTime);
        acquisitionText.gameObject.SetActive(false);
    }

    private void TurnOnPhysicalUI() {
    
        if(DivergenceControl.Instance.PendingReport) {
            return;
        }
        if(Warning.Instance != null) {
            Warning.Instance.TurnOffAlert();
        }
        if(Flashlight.Instance.isOn) {
            Flashlight.Instance.TurnOffLight();
        }  
        crosshair.SetActive(false);
        Vector3 onPos = new Vector3(0.47f, -.108f, .6f);
        reportDeviceUp = true;
        inMenu = true;
        reportDevice.transform.DOLocalMove(onPos, 0.5f);
        reportDevice.transform.DOLocalRotate(new Vector3 (0, -90, 0), 0.9f);

        defaultBottomRight.SetActive(false);
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        ReportUI.Instance.TurnOn();
    }

    private void TurnOffPhysicalUI() {

        Vector3 offPos = new Vector3(0, -5, 0);
        reportDeviceUp = false;
        reportDevice.transform.DOLocalMove(offPos, 0.5f);
        reportDevice.transform.DOLocalRotate(new Vector3 (0, -90, -45), 0.7f);
        crosshair.SetActive(true);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        inMenu = false;
        defaultBottomRight.SetActive(true);
        ReportUI.Instance.TurnOff();
    }

    public void PhysicalUI() {
        if(!inMenu) {TurnOnPhysicalUI();}
        else {TurnOffPhysicalUI();}
    }

    void SelectionMenu() {
        //if the report is glitching, change the tab text to red and do not allow menu interaction
        if(isGlitching) { MenuSelectionGlitch(); return; }
        else { tabText.color = Color.white; }

        //test code for physical report menu
        if(Input.GetKeyDown(KeybindManager.instance.GetKeybind("Report Menu"))) {
            PhysicalUI();
        }
    }

    private void MenuSelectionGlitch() {
        tabText.color = Color.red;
        if(reportDeviceUp) {
            PhysicalUI();
        }
        if(Input.GetKeyDown(KeybindManager.instance.GetKeybind("Report Menu"))) {
            if(!isReportTextGlitching) {
                audioSource.PlayOneShot(glitchSound);
                isReportTextGlitching = true;
                StartCoroutine(GlitchReportText());
            }
        }
    }

    public String GetCurrentRoom() {
        return currentRoom;
    }

    private void openEscape() {
        if(reportDeviceUp) {
            PhysicalUI();
        }
        havePausedAtleastOnce = true;
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        escapeMenuUI.SetActive(true);
        paused = true;
        defaultUI.SetActive(false);
    }

    public void closeEscape() {
        paused = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        EscapeMenu.Instance.CloseEscapeMenu();
        escapeMenuUI.SetActive(false);
        defaultUI.SetActive(true);
    }

    public void PauseControl(String src="") {
        if(src=="EndGame") {
            if (reportDeviceUp){
                PhysicalUI();
                return;
            }
        }

        if(EscapeMenu.Instance != null){          
            if(src=="escape" && EscapeMenu.Instance.inExtrasMenu){
                if(Input.GetKeyDown(KeyCode.Q)) {return;}
                EscapeMenu.Instance.CloseExtras();
                return;
            }
        }
        //lazy copy pasted solution
        if(EscapeMenu.Instance != null){    
            if(src=="escape" && EscapeMenu.Instance.inOptionsMenu){
                if(Input.GetKeyDown(KeyCode.Q)) {return;}
                if(KeybindMenu.Instance != null) { if(KeybindMenu.Instance.isOpen) {return;};}
                if(SettingsMenu.Instance != null) {if(!SettingsMenu.Instance.isPopupOpen) { SettingsMenu.Instance.ShowPopup(); return;};}
                if(SettingsMenu.Instance != null) {if(SettingsMenu.Instance.isPopupOpen) { SettingsMenu.Instance.closePopup(); return;};}
                SettingsMenu.Instance.ShowPopup();
                return;
            }
        }
        

        paused = !paused;
        PostProcessingControl.Instance.ActivateDepthOfField(paused);
        SoundControl.Instance.PauseSound(paused);

        if(src=="escape") {
            if(paused) {
                openEscape();
            }
            else {
                closeEscape();
            }   
        }

        if(src=="popup") {
            if(paused && reportDeviceUp) {
                PhysicalUI();
            }
        }
    }

    private void EscapeMenuControl() {
        //CHANGE TO ESCAPE
        if(Input.GetKeyDown(KeybindManager.instance.GetKeybind("Pause")) || Input.GetKeyDown(KeyCode.Escape)) { 
            PauseControl("escape");
        }
        if(Input.GetKeyDown(KeyCode.P)) {
            //disable all UI
            if(defaultUI.activeSelf) {
                defaultUI.SetActive(false);
            }
            else {
                defaultUI.SetActive(true);
            }
        }
        if(Input.GetKeyDown(KeyCode.O)) {
            debugUI.SetActive(!debugUI.activeSelf);
        }
    }

    private void EndingGame() {
        if(GameSystem.Instance.endReason == "quit") {
            return;
        }
        EndGameUI.SetActive(true);
    }
    //Currently used for figuring out which room the player is in and displaying it on the top right
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals(targetTag) || other.tag.Equals("FakeRoom"))
        {
            // Player is within a GameObject with the specified tag
            //Debug.Log("Player is within a GameObject with the tag: " + targetTag + " with object name: " + other.gameObject.name);
            roomText.GetComponent<TMP_Text>().text = other.gameObject.name;
            currentRoom = other.gameObject.name;
        }

        if (other.tag.Equals(tutTag)) {
            Tutorial.Instance.ActivateTrigger(other.gameObject);
        }
    }
}
