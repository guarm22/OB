using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTo : CustomDivergence
{
    
    public Vector3 target;
    private Vector3 originalPosition;

    public Vector3 rotation;

    private Vector3 originalRotation;

    public override void DoDivergenceAction(bool activate, DynamicObject gameObject) {
        if (activate) {
            if(target.Equals(Vector3.zero)) {
                target = transform.localPosition;
            }
            if(rotation.Equals(Vector3.zero)) {
                rotation = transform.localEulerAngles;
            }

            originalRotation = transform.localEulerAngles;
            originalPosition = transform.localPosition;
            transform.localEulerAngles = rotation;
            transform.localPosition = target;
        }
        else {
            transform.localPosition = originalPosition;
            transform.localEulerAngles = originalRotation;
        }
    }
}
