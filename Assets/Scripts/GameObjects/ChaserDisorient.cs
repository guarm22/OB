using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ChaserDisorient : MonoBehaviour {
    public Volume vol;
    private LensDistortion lensDistortion;
    private Vignette vignette;

    public float vignetteStart = 0.5f;
    public float lensDistortionStart = 0.5f;

    public float duration = 15f;

    void Start() {
        vol = GetComponent<Volume>();
        vol.profile.TryGet(out lensDistortion);
        vol.profile.TryGet(out vignette);

        vignette.intensity.Override(vignetteStart);
        lensDistortion.intensity.Override(lensDistortionStart);

        StartCoroutine(Distort());
    }

    public IEnumerator Distort() {
        SC_FPSController.Instance.Debuff("Slow", 0.3f, duration-0.75f);
        float elapsed = 0f;
        float lensTarget = 1f;
        float vignetteTarget = 1f;

        //slowly increase distortion to max
        while (elapsed < duration*0.4f) {
            if(PlayerUI.paused) {
                yield return null;
            }
            lensDistortion.intensity.Override(Mathf.Lerp(lensDistortionStart, lensTarget, elapsed / (duration*0.4f)));
            vignette.intensity.Override(Mathf.Lerp(vignetteStart, vignetteTarget, elapsed / (duration*0.4f)));
            elapsed += Time.deltaTime;
            yield return null;
        }

        if(PlayerUI.paused) {
                yield return null;
            }

        yield return new WaitForSeconds(duration*0.2f);

        //return to normal
        elapsed = 0f;
        while (elapsed < duration *0.4f) {
            if(PlayerUI.paused) {
                yield return null;
            }
            lensDistortion.intensity.Override(Mathf.Lerp(lensTarget, 0, elapsed / (duration*0.4f)));
            vignette.intensity.Override(Mathf.Lerp(vignetteTarget, 0, elapsed / (duration*0.4f)));
            elapsed += Time.deltaTime;
            yield return null;
        }

        Destroy(this.gameObject);
    }
}
