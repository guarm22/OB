using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{

    public GameObject player;
    public LayerMask triggerLayer;

    private bool trigger1 = false;
    private bool trigger6 = false;

    private bool blocker1 = true;
    private bool blocker2 = true;
    private bool blocker3 = true;
    private bool timer1 = true;
    private bool timer2 = true;
    private bool timer3 = true;
    private bool timer4 = true;
    private bool timer5 = true;

    public static Tutorial Instance;
    
    // Start is called before the first frame update
    void Start() {
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if(!Instance.trigger1) {
            GameSystem.Instance.SetGameTime(180f);
        }

        if(GameSystem.Instance.AnomaliesSuccesfullyReportedThisGame > 0) {
            if(Instance.blocker1) {
                Instance.blocker1 = false;
                Destroy(GameObject.Find("Blocker1"));
            }
        }
        if(GameSystem.Instance.AnomaliesSuccesfullyReportedThisGame > 2) {
            if(Instance.blocker3) {
                Instance.blocker3 = false;
                Destroy(GameObject.Find("Blocker3"));
            }
        }
        if(GameSystem.Instance.AnomaliesSuccesfullyReportedThisGame > 2) {
            if(Instance.blocker2) {
                Instance.blocker2 = false;
                GameSystem.Instance.SetGameTime(3f);
            }
        }

        if(GameSystem.Instance.gameTime <= 176f && Instance.timer3) {
            Instance.timer3= false;
            Popup.Instance.OpenPopup("There's a box on the other side of the room, if you wait a bit it'll disappear - creating a divergence.\n\nUse the reporting menu to report any divergences you discover! You wont be able to move forward until you report the box.\n\nHint: You can see the current room name at the top right.");
        }

        if(GameSystem.Instance.gameTime <= 173f && Instance.timer2) {
            Instance.timer2 = false;
            GameSystem.Instance.ManuallyCreateDivergence("TutorialBox");
        }

        if(GameSystem.Instance.gameTime <= 1f && Instance.timer1) {
            Instance.timer1 = false;
            Popup.Instance.OpenPopup("Congratulations! You've completed the tutorial. You can now choose a different level.");
        }
    }

    public void ActivateTrigger(GameObject hit) {

        if(hit.name.Equals("Trigger1")) {
            Instance.trigger1 = true;
            Popup.Instance.OpenPopup("Welcome to The Puncture\n\nControls:\nTAB: Opens and closes reporting menu.\nQ: Pauses game\nWASD: Movement\nSpace: Jump\nInteract: Left Click or E");
        }

        if(hit.name.Equals("Trigger3")) {
            Popup.Instance.OpenPopup("Reporting takes energy. Run out of energy and you wont be able to report until your energy builds back up.\n\nYou can also report multiple categories of divergences at once, but be careful because that takes more energy.\n\nYou cant report multiple rooms however, so be careful!");
        }

        if(hit.name.Equals("Trigger4")) {
            Popup.Instance.OpenPopup("Left click or E opens doors.\n\nYou can replay this tutorial at any time if you forget the controls.");
        }

        if(hit.name.Equals("Trigger5")) {
            Popup.Instance.OpenPopup("If you let too many divergences appear without reporting them, creatures eventually become attracted to the area.\n\nYou can report these for a reduced energy cost.\n\n");
        }

        if(hit.name.Equals("Trigger6")) {
            CreatureControl.Instance.ManuallySpawnCreature("Side Room");
            trigger6 = true;
        }

        if(hit.name.Equals("Trigger7")) {
            Popup.Instance.OpenPopup("Your goal is to survive until the timer at the top right hits zero.\n\nOn certain difficulties, flickering lights will appear in rooms that have divergences, make sure to use this to your advantage!\n\n");
        }

        if(hit.name.Equals("Trigger8")) {
            Popup.Instance.OpenPopup("Each level has different divergences, make sure you watch out for everything!\n\nReport the final divergence and you will complete the tutorial.\n\n");
            GameSystem.Instance.ManuallyCreateDivergence("tutorialToilet");
        }

        //will stop the player from hitting a trigger twice
        //can add a return somewhere if you dont want to destroy it
        Destroy(hit);
    }
}
