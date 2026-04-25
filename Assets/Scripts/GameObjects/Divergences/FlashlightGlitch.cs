using System.Collections;
using UnityEngine;

public class FlashlightGlitch : CustomDivergence {


    public float glitchFrequency = 0.1f;
    private float glitchTimer = 0f;

    private float resetTime = 8f;

    private Color originalLightColor;
    private float originalLightIntensity;

    private string room;

    void Awake() { 
    }


    public override void DoDivergenceAction(bool enable, DynamicObject obj) {
        originalLightColor = Flashlight.Instance.beam.color;
        room = obj.Room;
        originalLightIntensity = Flashlight.Instance.beam.intensity;

        if(enable) {
            StartCoroutine(GlitchFlashlight());
        }
        else {
            StopAllCoroutines();
            Flashlight.Instance.beam.color = originalLightColor;
            Flashlight.Instance.beam.intensity = originalLightIntensity;
        }
    }

    private IEnumerator GlitchFlashlight() {
        //every glitchFrequency seconds, turn the flashlight on or off randomly
        float elapsedTime = glitchFrequency;
        float resetTimer = 0f;
        while(true) {
            if(PlayerUI.paused || GameSystem.Instance.GameOver) {
                yield return null;
                continue;
            }

            if(PlayerUI.Instance.GetCurrentRoom() != room) {
                yield return null;
                continue;
            }

            resetTimer += Time.deltaTime;
            if(resetTimer <= resetTime) {
                yield return null;
                continue;
            }

            elapsedTime += Time.deltaTime;
            if(elapsedTime >= glitchFrequency) {
                elapsedTime = 0f;

                if(Random.Range(0, 100) > 2 && !Flashlight.Instance.isOn) {
                    Flashlight.Instance.TurnOnLight();
                }

                //random color
                Flashlight.Instance.beam.color = new Color(Random.value, Random.value, Random.value);
                Flashlight.Instance.beam.intensity = Random.Range(5f, 100f);
            }

            glitchTimer += Time.deltaTime;
            resetTimer += Time.deltaTime;
            if(glitchTimer >= 0.35f) {
                Flashlight.Instance.beam.color = originalLightColor;
                Flashlight.Instance.beam.intensity = originalLightIntensity;
                glitchTimer = 0f;
                resetTimer = 0f;
            }

            yield return null;
        }
        
    }
}
