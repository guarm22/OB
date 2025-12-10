using System.Collections;
using System.Collections.Generic;
using SojaExiles;
using TMPro;
using UnityEngine;

public class Tutorial : MonoBehaviour {

    public static Tutorial Instance;
    public GameObject tutorialReminder;

    public GameObject RoomNameTutorialTrigger;
    public GameObject FlashlightTrigger;
    public GameObject KeyTrigger;
    public GameObject RockfallTrigger;
    public GameObject FinalPathTrigger;
    public GameObject ShortPathTrigger;
    public GameObject EndTutorialTrigger;
    public GameObject InteractTrigger;

    public GameObject key1;

    public GameObject gate1;
    public GameObject gate2;

    public GameObject invisibleWall1;
    public GameObject invisibleWall2;
    public GameObject invisibleWall3;

    private bool manualTrigger1 = false;
    private bool manualTrigger2 = false;
    private bool manualTrigger3 = false;
    private bool JustLoaded = true;
    
    // Start is called before the first frame update
    void Start() {
        Instance = this;
        tutorialReminder.SetActive(false);
        
    }

    // Update is called once per frame
    void Update() {
        
        if(JustLoaded) {
            JustLoaded = false;
            Popup.Instance.OpenPopup("Welcome to The Puncture.\nYou can use the WASD keys to move around, and the mouse to look around.\n\nYou can also sprint by holding down the left shift key.\n\nYou may pause the game at any time by pressing ESC or Q.");
        }

        //picnic report
        if(GameSystem.Instance.AnomaliesSuccesfullyReportedThisGame > 0 && manualTrigger1 == false) {
            manualTrigger1 = true;
            StartCoroutine(CorrectReport1());
        }

        //rockfall report
        if(GameSystem.Instance.AnomaliesSuccesfullyReportedThisGame > 1 && manualTrigger2 == false) {
            UnlockGate2();
            tutorialReminder.GetComponentInChildren<TMP_Text>().text = "The gate is now open, you may proceed through.";
            manualTrigger2 = true;
        }

        //final path report
        if(GameSystem.Instance.AnomaliesSuccesfullyReportedThisGame > 2 && manualTrigger3 == false) {
            tutorialReminder.SetActive(false);
            tutorialReminder.GetComponentInChildren<TMP_Text>().text = "";
            manualTrigger3 = true;
        }

        if(Input.GetKeyDown(KeybindManager.instance.GetKeybind("Interact"))) {
            if(key1.activeInHierarchy && key1.GetComponent<Outliner>().hovering) {
                key1.SetActive(false);
                UnlockGate1();
                tutorialReminder.GetComponentInChildren<TMP_Text>().text = "";
                tutorialReminder.SetActive(false);
            }
        }
    }

    public void ActivateTrigger(GameObject trigger) {
        if(trigger == InteractTrigger) {
            Popup.Instance.OpenPopup("You can interact with objects in the world by looking at them and pressing E or Left Click.\n\n"+
            "Interactables are objects such as doors, gates, and collectibles. Collectibles will be highlighted when you hover over them.");
            InteractTrigger.SetActive(false);
        }

        if(trigger == RoomNameTutorialTrigger) {
            Popup.Instance.OpenPopup("When you enter a new room, you can see it's name on the top right of the HUD as long as you're inside of that room.\n\n"+
            "You can press tab anytime to see the map, showing how rooms are connected.");
            trigger.SetActive(false);
            invisibleWall1.SetActive(true);
            tutorialReminder.GetComponentInChildren<TMP_Text>().text = "";
            tutorialReminder.SetActive(false);
        }

        if(trigger == FlashlightTrigger) {
            tutorialReminder.GetComponentInChildren<TMP_Text>().text = 
            "Press F to activate your flashlight. This will cost some energy over time.\n\n"+
            "You can also right click to zoom in to see far away objects more clearly.";

            tutorialReminder.SetActive(true);
        }

        if(trigger == KeyTrigger) {
            StartCoroutine(KeyReminder());
            KeyTrigger.SetActive(false);
        }

        if(trigger == RockfallTrigger) {
            StartCoroutine(RockfallSequence());
            RockfallTrigger.SetActive(false);
        }

        if(trigger == FinalPathTrigger) {
            invisibleWall3.SetActive(true);
            StartCoroutine(FinalPathSequence());
            FinalPathTrigger.SetActive(false);
        }

        if(trigger == ShortPathTrigger) {
            DivergenceControl.Instance.ManuallyActivateDivergence("RockFall Div");
            invisibleWall2.SetActive(true);
            ShortPathTrigger.SetActive(false);
        }

        if(trigger == EndTutorialTrigger) {
            GameSystem.Instance.SetGameTime(1f);
        }
    }

    private void UnlockGate1() {
        Debug.Log("Unlocking gate 1 from tutorial");
        gate1.SetActive(false);
    }

    private void UnlockGate2() {
        Debug.Log("Unlocking gate 2 from tutorial");
        gate2.SetActive(false);
    }

    private IEnumerator CorrectReport1() {
        yield return new WaitForSeconds(2f);
        key1.SetActive(true);
        Popup.Instance.OpenPopup("You have successfully reported the divergence. Allowing divergences to stay around for too long can lead to bad things.\n\nKeep an eye on your energy in the TAB menu, as reporting divergences consumes a large amount of it.");
        tutorialReminder.GetComponentInChildren<TMP_Text>().text = "The key is behind the rock next to the gate. Press E or Left Click to collect it.";
    }

    private IEnumerator RockfallSequence() {
        yield return new WaitForSeconds(0.2f);
        SpawnCreature1();
        Popup.Instance.OpenPopup("When multiple divergences are around, Creatures may spawn to attack you.\n\nCreatures cannot outright harm you, but they will impede your attempts to report divergences.\n\nYou can report them for a reduced energy cost. But be careful, in certain instances, they can leave the room they originated from.");
        tutorialReminder.SetActive(true);
        tutorialReminder.GetComponentInChildren<TMP_Text>().text = "Report the divergence in Rock Fall while dealing with the creature.\n\nIf a creature attacks you successfully, it no longer needs to be reported. ";
    }

    private IEnumerator FinalPathSequence() {
        DivergenceControl.Instance.ManuallyActivateDivergence("FinalPath Addition");
        yield return new WaitForSeconds(1f);
        tutorialReminder.SetActive(true);
        tutorialReminder.GetComponentInChildren<TMP_Text>().text = "Another divergence has appeared. Report it to continue down the path.";
    }

    private void SpawnCreature1() {
        CreatureControl.Instance.ManuallySpawnCreature("Rock Fall");
    }

    private IEnumerator KeyReminder() {
        yield return new WaitForSeconds(1.5f);
        tutorialReminder.SetActive(true);
        tutorialReminder.GetComponentInChildren<TMP_Text>().text = "There should be a key somewhere in this area to open the gate.";
        yield return new WaitForSeconds(12f);
        DivergenceControl.Instance.ManuallyActivateDivergence("BenchDiv");
        yield return new WaitForSeconds(1.5f);
        Popup.Instance.OpenPopup("A divergence has appeared in the area.\n\n"+
        "Divergences are physical anomalies that cause strange effects. Find the divergence in this room and report it.\n\nPress TAB to open the report menu and report it. Each report requires a type and a room.");

        tutorialReminder.SetActive(true);
        tutorialReminder.GetComponentInChildren<TMP_Text>().text = "Press TAB to open the report menu and report the divergence.\n\nEach report requires a divergence type and a room. You may report multiple types, but only 1 room.";
    }
}
