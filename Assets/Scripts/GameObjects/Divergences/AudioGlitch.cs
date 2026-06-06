using UnityEngine;
using System.Collections;

public class AudioGlitch : CustomDivergence {

    private string roomName;
    private bool active = false;
    private float originalVolume; 

    void Awake() {
        originalVolume = AudioListener.volume; // Store original volume at the start
    }

    public override void DoDivergenceAction(bool enable, DynamicObject obj) {
        roomName = obj.Room;
        if(enable) {
            active = true;
            StartCoroutine(GlitchAudio());
        }
        else {
            StopAllCoroutines();
            active = false;
            AudioListener.volume = originalVolume; // Restore original volume
        }
    }

    private IEnumerator GlitchAudio() {
        while(true) {
            if(roomName != PlayerUI.Instance.GetCurrentRoom()) {
                //quickly restore the volume to original if player leaves the room
                AudioListener.volume = Mathf.Lerp(AudioListener.volume, originalVolume, Time.deltaTime * 8f);
                yield return null;
                continue;
            }
            //slowly decrease the volume to 0
            AudioListener.volume = Mathf.Lerp(AudioListener.volume, 0f, Time.deltaTime);
            yield return null;
        }
    }
}
