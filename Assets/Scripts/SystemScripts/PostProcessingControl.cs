using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PostProcessingControl : MonoBehaviour
{
    public static PostProcessingControl Instance;
    public DepthOfField depthOfField;
    public ChromaticAberration chromaticAberration;
    private VolumeProfile profile;
    public Bloom bloom;
    public LiftGammaGain liftGammaGain;
    public Vignette vignette;
    public ColorAdjustments colorAdjustments;

    private int divergenceCount = 0;

    public void ActivateDepthOfField(bool active) {
        //Debug.Log("Depth of Field: " + active);
        if(active) {
            depthOfField.focalLength.Override(300);
            depthOfField.focusDistance.Override(1);
        } else {
            depthOfField.focalLength.Override(1);
            depthOfField.focusDistance.Override(1);
        }
    }
    public void ActivateDepthOfField(bool active, float focalLength=1, float focusDistance=1) {
        //Debug.Log("Depth of Field: " + active);
        if(active) {
            depthOfField.focalLength.Override(focalLength);
            depthOfField.focusDistance.Override(focusDistance);
        } else {
            depthOfField.focalLength.Override(focalLength);
            depthOfField.focusDistance.Override(focusDistance);
        }
    }

    void Awake() {
        Instance = this;
        profile = GetComponent<Volume>().profile;
        profile.TryGet<DepthOfField>(out depthOfField);
        profile.TryGet<ChromaticAberration>(out chromaticAberration);
        profile.TryGet<LiftGammaGain>(out liftGammaGain);
        profile.TryGet<Bloom>(out bloom);
        profile.TryGet<Vignette>(out vignette);
        profile.TryGet<ColorAdjustments>(out colorAdjustments);
    }

    void Start() {
        SetGammaAlpha(PlayerPrefs.GetInt("Brightness"));
    }

    private IEnumerator AddChromaticAbberation(float intensity, float duration=4f) {
        float elapsed = 0f;

        while (elapsed < duration) {
            chromaticAberration.intensity.Override(intensity * (elapsed / duration));
            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    // Update is called once per frame
    void Update() {
        if(divergenceCount != DivergenceControl.Instance.DivergenceList.Count) {
            divergenceCount = DivergenceControl.Instance.DivergenceList.Count;
            if(DivergenceControl.Instance.DivergenceList.Count >= DivergenceControl.Instance.Rooms.Count) {
                StartCoroutine(AddChromaticAbberation(1f));
            }
            else if(DivergenceControl.Instance.DivergenceList.Count >= DivergenceControl.Instance.Rooms.Count-1) {
                StartCoroutine(AddChromaticAbberation(0.5f));
            }
            else {
                StartCoroutine(AddChromaticAbberation(0.0000000001f));
            }
        }
    }

    public void SetGammaAlpha(float gammaAlpha) {
        //gamma goes from -1 to 1, 0 is default
        gammaAlpha = (gammaAlpha - 50) / 50;
        
        liftGammaGain.gamma.Override(new Vector4(1f, 1f, 1f, gammaAlpha));
    }

    public void EndGameScreenPostProcessing() {
        float duration = 4f;

        //bloom intensity over time
        float maxBloomIntensity = 350f;  //350
        float maxBloomThreshold = 1f;   //1
        StartCoroutine(BloomIntensityOverTime(maxBloomIntensity, maxBloomThreshold, duration));       

        float targetVignetteIntensity = 0.3f;
        float targetVignetteSmoothness = 0.7f;
        StartCoroutine(ChangeVignette(targetVignetteIntensity, targetVignetteSmoothness, duration));


        //contrast = 40
        //intensity = 5
        //post exposure .2
        float intensityTarget = 15f;
        Color colorFilterTarget = new Color(70,30,30)/255f;
        float contrastTarget = 62f;
        float saturationTarget = 100f;
        float postExposureTarget = 0.2f;
        StartCoroutine(ChangeColorAdjustments(colorFilterTarget, contrastTarget, saturationTarget, postExposureTarget, intensityTarget, duration));

        //film grain = 0.5


        //gamma = 0.6
    }

    private IEnumerator ChangeColorAdjustments(Color colorFilter, float contrast, float saturation, float postExposure, float intensity, float duration) {
        float elapsed = 0f;
        Color startingColorFilter = colorAdjustments.colorFilter.value;
        float startingContrast = colorAdjustments.contrast.value;
        float startingSaturation = colorAdjustments.saturation.value;
        float startingPostExposure = colorAdjustments.postExposure.value;
        float startingIntensity = 0f;

        while (elapsed < duration) {
            float currentIntensity = Mathf.Lerp(startingIntensity, intensity, elapsed / duration);
            Color currentColor = Color.Lerp(startingColorFilter, colorFilter, elapsed / duration) * 1.0f;
            colorAdjustments.colorFilter.Override(currentColor*currentIntensity);
            colorAdjustments.contrast.Override(Mathf.Lerp(startingContrast, contrast, elapsed / duration));
            colorAdjustments.saturation.Override(Mathf.Lerp(startingSaturation, saturation, elapsed / duration));
            colorAdjustments.postExposure.Override(Mathf.Lerp(startingPostExposure, postExposure, elapsed / duration));
            elapsed += Time.deltaTime;
            yield return null;
        }
        colorAdjustments.colorFilter.Override(colorFilter*intensity);
        colorAdjustments.contrast.Override(contrast);
        colorAdjustments.saturation.Override(saturation);
        colorAdjustments.postExposure.Override(postExposure);
    }

    private IEnumerator ChangeVignette(float intensity, float smoothness, float duration){
        float elapsed = 0f;
        float startingIntensity = vignette.intensity.value;
        float startingSmoothness = vignette.smoothness.value;

        while (elapsed < duration) {
            vignette.intensity.Override(Mathf.Lerp(startingIntensity, intensity, elapsed / duration));
            vignette.smoothness.Override(Mathf.Lerp(startingSmoothness, smoothness, elapsed / duration));
            elapsed += Time.deltaTime;
            yield return null;
        }
        vignette.intensity.Override(intensity);
        vignette.smoothness.Override(smoothness);
        
    }

    public IEnumerator BloomIntensityOverTime(float targetIntensity, float targetThreshold, float duration) {
        float elapsed = 0f;
        float startingIntensity = bloom.intensity.value;
        float startingThreshold = bloom.threshold.value;

        while (elapsed < duration) {
            bloom.intensity.Override(Mathf.Lerp(startingIntensity, targetIntensity, elapsed / duration));
            bloom.threshold.Override(Mathf.Lerp(startingThreshold, targetThreshold, elapsed / duration));
            elapsed += Time.deltaTime;
            yield return null;
        }
        bloom.intensity.Override(targetIntensity);
    }
}
