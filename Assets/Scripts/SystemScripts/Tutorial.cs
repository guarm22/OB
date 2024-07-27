using System.Collections;
using System.Collections.Generic;
using SojaExiles;
using UnityEngine;

public class Tutorial : MonoBehaviour
{

    public GameObject player;
    public LayerMask triggerLayer;
    private bool blocker1 = true;
    private bool blocker3 = true;
    private bool timer1 = true;
    private bool timer2 = true;
    private bool endTrigger = true;
    public static Tutorial Instance;

    public GameObject door1;
    public GameObject door2;
    
    // Start is called before the first frame update
    void Start() {
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {

        if(GameSystem.Instance.AnomaliesSuccesfullyReportedThisGame > 0) {
            if(Instance.blocker1) {
                StartCoroutine(TutorialCoroutine1());
            }
        }

         if(GameSystem.Instance.AnomaliesSuccesfullyReportedThisGame > 1) {
            if(Instance.blocker3) {
                Instance.blocker3 = false;
                door2.GetComponent<opencloseDoor>().ChangeLockState(false);
            }
        }

        if(GameSystem.Instance.gameTime < 180f && Instance.timer1) {
            Instance.timer1 = false;
            DivergenceControl.Instance.ManuallyActivateDivergence("DivCan");
        }

        if(GameSystem.Instance.AnomaliesSuccesfullyReportedThisGame > 2 && Instance.endTrigger) {
           StartCoroutine(TutorialCoroutine2());
        }

        if(GameSystem.Instance.gameTime < 899f && timer2) {
            timer2 = false;
            Popup.Instance.OpenPopup("Welcome to The Puncture.\nYou can use the WASD keys to move around, and the mouse to look around.\n\nYou can also sprint by holding down the left shift key.\n\nYou may pause the game at any time by pressing ESC or Q.");
        }
    }
    
    IEnumerator TutorialCoroutine1() {
        Instance.blocker1 = false;
        yield return new WaitForSeconds(1f);
        door1.GetComponent<opencloseDoor>().ChangeLockState(false);
        Popup.Instance.OpenPopup("You successfully reported the divergence, and the area has returned to normal.\n\nYou can now open the door by left clicking or pressing E.\n\nYou can replay this tutorial at any time if you forget the controls.");
    }

    IEnumerator TutorialCoroutine2() {
        endTrigger = false;
        yield return new WaitForSeconds(1f);
        GameSystem.Instance.SetGameTime(3f);
        Popup.Instance.OpenPopup("Congratulations! You have completed the tutorial.\n\n");
    }

    public void ActivateTrigger(GameObject hit) {
        if(hit.name.Equals("ControlsTrigger2")) {
            Popup.Instance.OpenPopup("Some objects can be interacted with by pressing E or left clicking.");
        }

        if(hit.name.Equals("DivTrigger1")) {
            Popup.Instance.OpenPopup("Moving forward will create a 'Disappearence Divergence', these can be reported by pressing TAB."+ 
            "\n\nTo report, you need to select a room and divergence type."+
            "\n\nThe current room is shown on the top right, and you can also see which room you are in on the report screen. ");
        }

        if(hit.name.Equals("ControlsTrigger3")) {
            Popup.Instance.OpenPopup("You can press F to turn on your flashlight. This will help you see in dark areas at the cost of some energy."+
            "\n\nYou can also right click to zoom in.");
        }

        if(hit.name.Equals("CreateFirstDiv")) {
            DivergenceControl.Instance.ManuallyActivateDivergence("TutorialBox");
        }

        if(hit.name.Equals("Trigger5")) {
            Popup.Instance.OpenPopup("If you let too many divergences appear without reporting them, creatures eventually become attracted to the area."+
            "\n\nYou can report these for a reduced energy cost.\n\n"+
            "\n\nNot every creature is the same, some are faster than others, and some are more deadly. Learn more about them in the Compendium.");
        }

        if(hit.name.Equals("Trigger6")) {
            CreatureControl.Instance.ManuallySpawnCreature("Main Room");
        }

        if(hit.name.Equals("Trigger7")) {
            Popup.Instance.OpenPopup("You can see your energy on the top right. If you are running low on energy, you wont be able to report anything.\n\nIf you are confident in your energy usage, you can also report multiple divergence types at once.\n\n");
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
