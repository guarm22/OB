using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class LurkerDisorient : MonoBehaviour {

    public AudioClip disorientSound;
    public float duration = 12f;

    public Volume vol;
    private LensDistortion lensDistortion;
    private Vignette vignette;

    public float vignetteStart = 0.5f;
    public float lensDistortionStart = 0.5f;

    void Start() {
         vol = GetComponent<Volume>();
        vol.profile.TryGet(out lensDistortion);
        vol.profile.TryGet(out vignette);

        vignette.intensity.Override(vignetteStart);
        lensDistortion.intensity.Override(lensDistortionStart);

        StartCoroutine(DistortSound());
        StartCoroutine(Distort());
    }

    private IEnumerator DistortSound() {
        AudioSource a = this.gameObject.AddComponent<AudioSource>();
        this.gameObject.GetComponent<AudioSource>().clip = disorientSound;
        this.gameObject.GetComponent<AudioSource>().Play();
        float targetVol = SoundControl.Instance.MasterVolume;

        AudioListener.volume = 0f;

        float elapsed = 0f;
        while(elapsed < duration-1f) {
            if(PlayerUI.paused) {
                yield return null;
            }
            elapsed += Time.deltaTime;
            a.volume = Mathf.Lerp(0f, 1f, 1-(elapsed/duration));
            AudioListener.volume = Mathf.Lerp(0f, targetVol, elapsed/duration);
            yield return null;
        }
        AudioListener.volume = targetVol;
    }

    private IEnumerator Distort() {
        float elapsed = 0f;
        float lensTarget = 1f;
        float vignetteTarget = 1f;

        //slowly increase distortion to max
        while (elapsed < duration * 0.4f) {
            if(PlayerUI.paused) {
                yield return null;
            }
            lensDistortion.intensity.Override(Mathf.Lerp(lensDistortionStart, lensTarget, elapsed / duration));
            vignette.intensity.Override(Mathf.Lerp(vignetteStart, vignetteTarget, elapsed / duration));
            elapsed += Time.deltaTime;
            yield return null;
        }

        if(PlayerUI.paused) {
                yield return null;
            }

        yield return new WaitForSeconds(duration * 0.2f);

        //return to normal
        elapsed = 0f;
        while (elapsed < duration * 0.4f) {
            if(PlayerUI.paused) {
                yield return null;
            }
            lensDistortion.intensity.Override(Mathf.Lerp(lensTarget, 0, elapsed / duration));
            vignette.intensity.Override(Mathf.Lerp(vignetteTarget, 0, elapsed / duration));
            elapsed += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(0.25f);
        Debug.Log("Destroying LurkerDisorient");
        Destroy(this.gameObject);
    }
}
