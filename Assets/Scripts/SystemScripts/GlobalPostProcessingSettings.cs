using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class GlobalPostProcessingSettings : MonoBehaviour
{
    public static GlobalPostProcessingSettings Instance;
    LiftGammaGain liftGammaGain;
 
    private void Awake() {
        Instance = this;
        Volume renderingVolume = GetComponent<Volume>();
        if (!renderingVolume.profile.TryGet(out liftGammaGain)) throw new System.NullReferenceException(nameof(liftGammaGain));

        //gamma goes from 0-2, 1 is default
        SetGammaAlpha(PlayerPrefs.GetFloat("Brightness"));

        //set master volume
        AudioListener.volume = PlayerPrefs.GetInt("MasterVolume") / 100f;
    }

    public void SetGammaAlpha(float gammaAlpha) {
        //gamma goes from -1 to 1, 0 is default
        gammaAlpha = (gammaAlpha - 50) / 50;
        
        liftGammaGain.gamma.Override(new Vector4(1f, 1f, 1f, gammaAlpha));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
