using UnityEngine;

public class ReplaceMaterial : CustomDivergence {

    private Material originalMaterial;

    [SerializeField]
    private Material replacementMaterial;

    public override void DoDivergenceAction(bool activate, DynamicObject gameObject) {
        if(activate) {
            originalMaterial = gameObject.Obj.GetComponent<Renderer>().material;
            gameObject.Obj.GetComponent<Renderer>().material = replacementMaterial;
        }
        else {
            gameObject.Obj.GetComponent<Renderer>().material = originalMaterial;
        }
    }
}
