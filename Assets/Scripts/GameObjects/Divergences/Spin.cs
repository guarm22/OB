using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Spin : CustomDivergence
{
    public float rotationsPerSecond;
    public Vector3 rotationAxis;
    private Vector3 originalRotation;

    public override void DoDivergenceAction(bool activate, DynamicObject gameObject) {
        if (activate) {
            originalRotation = transform.localEulerAngles;
            StartCoroutine(SpinObject(this.gameObject));
        }
        else {
            StopAllCoroutines();
            DOTween.Kill(this.gameObject);
            transform.localEulerAngles = originalRotation;
        }
    }

    public IEnumerator SpinObject(GameObject obj) {
        while (true) {
            obj.transform.Rotate(rotationAxis, rotationsPerSecond * 360 * Time.deltaTime);
            yield return null;
        }
    }
}
