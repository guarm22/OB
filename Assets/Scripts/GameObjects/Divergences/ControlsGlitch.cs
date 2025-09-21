using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlsGlitch : CustomDivergence
{
    private String roomName;
    private float crouchDuration = 2f;
    private float crouchCD = 3f;
    private bool active = false;

    void Awake() {
        
    }
   
    public override void DoDivergenceAction(bool enable, DynamicObject obj) {
        roomName = obj.Room;
        if(enable) {
            active = true;
            //invert the controls
            StartCoroutine(GlitchControls());
            SC_FPSController.Instance.controlsGlitch = true;
        }
        else {
            active = false;
            SC_FPSController.Instance.controlsGlitch = false;
            StopAllCoroutines();
        }
    }

    private IEnumerator GlitchControls() {
        while(true) {
            if(roomName != PlayerUI.Instance.GetCurrentRoom()) {
                yield return null;
                continue;
            }
            //every 2 seconds, crouch
            yield return StartCoroutine(SC_FPSController.Instance.ForceCrouch(crouchDuration));
            yield return new WaitForSeconds(crouchCD);
            
        }
    }

    void Update() {
        if(!active) {
            return;
        }

        if(PlayerUI.Instance.GetCurrentRoom() != roomName) {
            SC_FPSController.Instance.controlsGlitch = false;
            return;
        }
        SC_FPSController.Instance.controlsGlitch = true;
    }


}
