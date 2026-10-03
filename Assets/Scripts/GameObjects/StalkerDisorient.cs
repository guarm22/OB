using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class StalkerDisorient : MonoBehaviour {
    public float duration = 12f;

    public Volume vol;
    
    private ColorAdjustments colorAdjustments;

    void Start() {
        vol = GetComponent<Volume>();

        colorAdjustments = vol.profile.TryGet<ColorAdjustments>(out var ca) ? ca : null;
        StartCoroutine(Distort());
    }


    private IEnumerator Distort() {
        float elapsed = 0f;
        colorAdjustments.saturation.Override(-60f);
        colorAdjustments.contrast.Override(100f);
        colorAdjustments.postExposure.Override(5f);
        colorAdjustments.hueShift.Override(90f);

        yield return new WaitForSeconds(1f);

        //return to normal
        elapsed = 0f;
        while (elapsed < duration * 0.4f) {
            if(PlayerUI.paused) {
                yield return null;
            }
            //slowly return each value to 0
            colorAdjustments.saturation.Override(Mathf.Lerp(-60f, 0f, elapsed / (duration * 0.4f)));
            colorAdjustments.contrast.Override(Mathf.Lerp(100f, 0f, elapsed / (duration * 0.4f)));
            colorAdjustments.postExposure.Override(Mathf.Lerp(5f, 0f, elapsed / (duration * 0.4f)));
            colorAdjustments.hueShift.Override(Mathf.Lerp(90f, 0f, elapsed / (duration * 0.4f)));
            
            elapsed += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(0.25f);
        Destroy(this.gameObject);
    }
}
