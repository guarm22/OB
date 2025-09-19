using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraGlitch : CustomDivergence {

    public string roomName;
    public float glitchFrequency = 5f;
    void Awake() {
        
    }

    public override void DoDivergenceAction(bool enable, DynamicObject obj) {
        if(enable) {
            StartCoroutine(GlitchCamera());
        }
        else {
            StopAllCoroutines();
        }
    }

    private IEnumerator GlitchCamera() {
        //every 0.5 seconds, change the camera angle to a random angle
        while(true) {
            if(roomName != PlayerUI.Instance.GetCurrentRoom()) {
                yield return null;
                continue;
            }

            if(PlayerUI.paused || GameSystem.Instance.GameOver) {
                yield return null;
                continue;
            }
            //change camera angle to a random angle
            SC_FPSController.Instance.MoveCamera(new Vector3(Random.Range(-10f, 10f), Random.Range(0f, 360f), 0));
            yield return new WaitForSeconds(glitchFrequency);
        }
    }

}
