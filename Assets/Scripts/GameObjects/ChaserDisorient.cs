using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ChaserDisorient : MonoBehaviour
{
    public Volume vol;
    private LensDistortion lensDistortion;
    private Vignette vignette;

    public float vignetteStart = 0.5f;
    public float lensDistortionStart = 0.5f;

    public float duration = 5f;

    void Start() {
        vol = GetComponent<Volume>();
        vol.profile.TryGet(out lensDistortion);
        vol.profile.TryGet(out vignette);

        vignette.intensity.Override(vignetteStart);
        lensDistortion.intensity.Override(lensDistortionStart);

        StartCoroutine(Distort());
    }

    public IEnumerator Distort() {
        SC_FPSController.Instance.Debuff("Slow", 0.3f, (duration*2) + 2);
        float elapsed = 0f;
        float lensTarget = 1f;
        float vignetteTarget = 1f;

        //slowly increase distortion to max
        while (elapsed < duration) {
            lensDistortion.intensity.Override(Mathf.Lerp(lensDistortionStart, lensTarget, elapsed / duration));
            vignette.intensity.Override(Mathf.Lerp(vignetteStart, vignetteTarget, elapsed / duration));
            elapsed += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(2f);

        //return to normal
        elapsed = 0f;
        while (elapsed < duration) {
            lensDistortion.intensity.Override(Mathf.Lerp(lensTarget, 0, elapsed / duration));
            vignette.intensity.Override(Mathf.Lerp(vignetteTarget, 0, elapsed / duration));
            elapsed += Time.deltaTime;
            yield return null;
        }

        Destroy(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
