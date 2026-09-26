using System.Collections;
using UnityEngine;

public class FlickerDisappearance : CustomDivergence {

    [SerializeField]
    private float minFlickerFrequency = 0.05f;

    [SerializeField]
    private float maxFlickerFrequency = 1.2f;

    private MeshRenderer meshRenderer;


    void Start() {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    public override void DoDivergenceAction(bool activate, DynamicObject gameObject) {
        if(activate) {
            StartCoroutine(Flicker(gameObject));
        }
        else {
            StopAllCoroutines();
            meshRenderer.enabled = true;
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

            meshRenderer.enabled = !meshRenderer.enabled;
            yield return new WaitForSeconds(flickerDuration);
            meshRenderer.enabled = !meshRenderer.enabled;
            yield return new WaitForSeconds(flickerDuration);
        }
    }
}
