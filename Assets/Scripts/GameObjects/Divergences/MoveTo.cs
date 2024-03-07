using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTo : CustomDivergence
{
    
    public Vector3 target;
    public Vector3 originalPosition;

    public override void DoDivergenceAction(bool activate, DynamicObject gameObject) {
        if (activate) {
            originalPosition = transform.localPosition;
            transform.localPosition = target;
        }
        else {
            transform.localPosition = originalPosition;
        }
    }
}
