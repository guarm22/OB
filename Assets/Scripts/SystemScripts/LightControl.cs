using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class LightControl : MonoBehaviour
{
    private bool killedLights = false;
    private Dictionary<Light, Coroutine> flickeringLights = new Dictionary<Light, Coroutine>();

    public static LightControl Instance;
    void Start() {
        Instance = this;
    }
    void Update() {
        
    }

    public IEnumerator FlickerLight(Light light, float minIntensity, float maxIntensity, float duration) {
        float elapsedTime = 0f;
        while (elapsedTime < duration) {
            // Randomly change the light's intensity
            light.intensity = UnityEngine.Random.Range(minIntensity, maxIntensity);

            // Wait for a short amount of time
            yield return new WaitForSeconds(UnityEngine.Random.Range(0.05f, 0.1f));

            elapsedTime += Time.deltaTime;
        }
        flickeringLights.Remove(light);
        // Ensure the light's intensity is reset to its original state
        light.intensity = maxIntensity;
}

    public void FlickerLights()
    {
        foreach (GameObject lightObject in GameObject.FindGameObjectsWithTag("Light"))
        {
            Light light = lightObject.GetComponent<Light>();
            flickeringLights.Add(light, StartCoroutine(FlickerLight(light, 2f, light.intensity, .1f)));
        }
    }

    public void KillLights(float amt) {
        if(killedLights) return;

        killedLights = true;
        int amtOfLightsInScene = GameObject.FindGameObjectsWithTag("Light").Length;
        int lightsToKill = (int)System.Math.Ceiling(amtOfLightsInScene * amt);
        for(int i = 0; i < lightsToKill; i++) {
            GameObject light = GameObject.FindGameObjectsWithTag("Light")[UnityEngine.Random.Range(0, amtOfLightsInScene-i)];
            light.tag = "Untagged";
            light.GetComponent<Light>().intensity = 2f;
            if(flickeringLights.ContainsKey(light.GetComponent<Light>()) && flickeringLights[light.GetComponent<Light>()] != null) {
                StopCoroutine(flickeringLights[light.GetComponent<Light>()]);
            }
        }
    }
}
