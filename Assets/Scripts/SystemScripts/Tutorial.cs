using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{

    public GameObject player;
    public LayerMask triggerLayer;
    private bool blocker1 = true;
    private bool blocker3 = true;
    private bool timer1 = true;
    private bool endTrigger = false;

    public static Tutorial Instance;
    
    // Start is called before the first frame update
    void Start() {
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {

        if(GameSystem.Instance.AnomaliesSuccesfullyReportedThisGame > 0) {
            if(Instance.blocker1) {
                Instance.blocker1 = false;
                Destroy(GameObject.Find("DoorBlocker"));
                Popup.Instance.OpenPopup("You successfully reported the divergence, and the area has returned to normal.\n\nYou can now open the door by left clicking or pressing E.\n\nYou can replay this tutorial at any time if you forget the controls.");

            }
        }

         if(GameSystem.Instance.AnomaliesSuccesfullyReportedThisGame > 2) {
            if(Instance.blocker3) {
                Instance.blocker3 = false;
                Destroy(GameObject.Find("FinalBlocker"));
            }
        }

        if(GameSystem.Instance.gameTime < 180f && Instance.timer1) {
            Instance.timer1 = false;
            DivergenceControl.Instance.ManuallyActivateDivergence("DivCan");
        }

        if(GameSystem.Instance.AnomaliesSuccesfullyReportedThisGame > 3 && Instance.endTrigger) {
            endTrigger = false;
            GameSystem.Instance.SetGameTime(5f);
            Popup.Instance.OpenPopup("Congratulations! You have completed the tutorial.\n\n");
        }
    }

    public void ActivateTrigger(GameObject hit) {

        if(hit.name.Equals("ControlsTrigger")) {
            Popup.Instance.OpenPopup("Welcome to The Puncture\n\nControls:\nTAB: Opens and closes reporting menu.\nQ: Pauses game\nWASD: Movement\nSpace: Jump\nInteract: Left Click or E\nFlashlight:F\nCrouch: Left Ctrl");
        }

        if(hit.name.Equals("DivTrigger1")) {
            Popup.Instance.OpenPopup("Moving forward will create a DIVERGENCE, these can be reported by pressing TAB.\n\nTo report, you need to select a room and divergence type (the current room is shown on the top right, and you can also see which room you are in on the report screen). ");
        }

        if(hit.name.Equals("CreateFirstDiv")) {
            DivergenceControl.Instance.ManuallyActivateDivergence("TutorialBox");
        }

        if(hit.name.Equals("Trigger5")) {
            Popup.Instance.OpenPopup("If you let too many divergences appear without reporting them, creatures eventually become attracted to the area.\n\nYou can report these for a reduced energy cost.\n\n");
        }

        if(hit.name.Equals("Trigger6")) {
            CreatureControl.Instance.ManuallySpawnCreature("Main Room");
        }

        if(hit.name.Equals("Trigger7")) {
            Popup.Instance.OpenPopup("You can see your energy on the bottom right. If you are running low on energy, you wont be able to report anything\n\nIf you are confident in your energy usage, you can also report multiple divergence types at once.\n\n");
        }

        if(hit.name.Equals("Trigger8")) {
            Popup.Instance.OpenPopup("Each level has different divergences, make sure you are always watching AND listening for anything suspicious!\n\nSometimes you will get hints for divergences that have been around for a long time, look out for FLASHING LIGHTS.\n\nReport the divergence in this room to enter the final room.");
        }
        if(hit.name.Equals("TriggerToilet")) {
            DivergenceControl.Instance.ManuallyActivateDivergence("tutorialToilet");
        }
        if(hit.name.Equals("Trigger9")) {
            Popup.Instance.OpenPopup("The divergence in this room will appear in 20 seconds.\n\nFamiliarize yourself with the room and report it when it appears.\n\n");
            GameSystem.Instance.SetGameTime(200f);
        }

        //will stop the player from hitting a trigger twice
        //can add a return somewhere if you dont want to destroy it
        Destroy(hit);
    }
}
