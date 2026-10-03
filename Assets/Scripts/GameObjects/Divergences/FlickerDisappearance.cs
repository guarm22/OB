using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlickerDisappearance : CustomDivergence {

    [SerializeField]
    private float minFlickerFrequency = 0.05f;

    [SerializeField]
    private float maxFlickerFrequency = 1.2f;

    private List<MeshRenderer> meshRenderers = new List<MeshRenderer>();
    private List<Light> lights = new List<Light>();


    void Start() {
        //get all mesh renderers in this object and its children
        meshRenderers.AddRange(GetComponentsInChildren<MeshRenderer>());
        meshRenderers.AddRange(GetComponents<MeshRenderer>());
        lights.AddRange(GetComponentsInChildren<Light>());
    }

    public override void DoDivergenceAction(bool activate, DynamicObject gameObject) {
        if(activate) {
            StartCoroutine(Flicker(gameObject));
        }
        else {
            StopAllCoroutines();
            foreach(MeshRenderer mr in meshRenderers) {
                mr.enabled = true;
            }
            foreach(Light l in lights) {
                l.enabled = true;
            }
        }
    }

    public IEnumerator Flicker(DynamicObject gameObject) {
        while(true) {
            if(PlayerUI.paused) { 
                yield return null; 
                continue; 
            }

            float flickerDuration = Random.Range(minFlickerFrequency, maxFlickerFrequency);

            //if flickerduration is above 0.9, give a 50% chance to reduce by half
            if(flickerDuration > 0.9f) {
                if(Random.value > 0.5f) {
                    flickerDuration /= 2f;
                }
            }

            foreach(MeshRenderer mr in meshRenderers) {
                mr.enabled = !mr.enabled;
            }
            foreach(Light l in lights) {
                l.enabled = !l.enabled;
            }

            yield return new WaitForSeconds(flickerDuration);

            foreach(MeshRenderer mr in meshRenderers) {
                mr.enabled = !mr.enabled;
            }
            foreach(Light l in lights) {
                l.enabled = !l.enabled;
            }

            yield return new WaitForSeconds(flickerDuration);
        }
    }
}
