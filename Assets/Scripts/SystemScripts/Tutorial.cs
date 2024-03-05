using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{

    public GameObject player;
    public LayerMask triggerLayer;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void ActivateTrigger(GameObject hit) {
        if(hit.name.Equals("Trigger1")) {
            Popup.Instance.OpenPopup("Welcome to The Puncture\nPress TAB to open the reporting menu.");
        }

        if(hit.name.Equals("Trigger2")) {
            Popup.Instance.OpenPopup("See that box in front of you? If you walk a bit more forward it'll disappear.\nUse the reporting menu to report that it's disappeared!");
        }

        if(hit.name.Equals("DivTrigger1")) {
            GameSystem.Instance.ManuallyCreateDivergence("TutorialBox");
        }

        /*if(hit.name.Equals("ZombieTrigger")) {
            GameSystem.Instance.ManuallySpawnCreature("Main");
        }*/

        //will stop the player from hitting a trigger twice
        //can add a return somewhere if you dont want to destroy it
        Destroy(hit);
    }
}
