using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{

    public GameObject player;
    public LayerMask triggerLayer;

    private bool blocker1 = true;
    private bool blocker2 = true;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(GameSystem.Instance.AnomaliesSuccesfullyReportedThisGame > 0) {
            if(blocker1) {
                blocker1 = false;
                Destroy(GameObject.Find("Blocker1"));
            }
        }
        if(GameSystem.Instance.AnomaliesSuccesfullyReportedThisGame > 1) {
            if(blocker2) {
                blocker2 = false;
                GameSystem.Instance.SetGameTime(3f);
            }
        }
    }

    public static void ActivateTrigger(GameObject hit) {
        if(hit.name.Equals("Trigger1")) {
            Popup.Instance.OpenPopup("Welcome to The Puncture\n\nControls:\nTAB: Opens and closes reporting menu.\nQ: Pauses game\nWASD: Movement\nSpace: Jump\n");
        }

        if(hit.name.Equals("Trigger2")) {
            Popup.Instance.OpenPopup("See that box in front of you? If you walk a bit more forward it'll disappear - creating a divergence.\n\nUse the reporting menu to report any divergences you discover!\n\nHint: You can see the current room name at the top right.");
        }

        if(hit.name.Equals("DivTrigger1")) {
            GameSystem.Instance.ManuallyCreateDivergence("TutorialBox");
        }

        if(hit.name.Equals("Trigger3")) {
            Popup.Instance.OpenPopup("Reporting takes energy. Run out of energy and you cant report for a while!\n\nYou can also report multiple categories at once, but be careful because that takes more energy.\n\n");
        }

        if(hit.name.Equals("Trigger4")) {
            Popup.Instance.OpenPopup("Left click opens doors.\n\nYou can replay this tutorial at any time if you forget the controls.");
        }

        if(hit.name.Equals("Trigger5")) {
            Popup.Instance.OpenPopup("If you leave too many divergences appear without reporting them, creatures eventually become attracted to the area.\n\nYou can report these for a reduced energy cost, but they can end your game...");
        }

        if(hit.name.Equals("Trigger6")) {
            GameSystem.Instance.ManuallySpawnCreature("Side Room");
        }

        //will stop the player from hitting a trigger twice
        //can add a return somewhere if you dont want to destroy it
        Destroy(hit);
    }
}
