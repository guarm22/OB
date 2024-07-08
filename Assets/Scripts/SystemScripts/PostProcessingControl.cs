using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PostProcessingControl : MonoBehaviour
{
    public static PostProcessingControl Instance;
    public DepthOfField depthOfField;
    private VolumeProfile profile;

    public void ActivateDepthOfField(bool active) {
        Debug.Log("Depth of Field: " + active);
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
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
