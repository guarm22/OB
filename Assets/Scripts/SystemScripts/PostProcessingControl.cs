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

    void Start() {
        Instance = this;
        profile = GetComponent<Volume>().profile;
        profile.TryGet<DepthOfField>(out depthOfField);
        profile.TryGet<ChromaticAberration>(out chromaticAberration);
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
}
