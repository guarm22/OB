using System.Collections;
using UnityEngine;
using DG.Tweening;

public class MoveContinous : CustomDivergence {

    public Vector3 target;
    public float moveTime = 2f;
    private Vector3 originalPosition;
    public override void DoDivergenceAction(bool activate, DynamicObject dynamic) {
        if(activate) {
            originalPosition = gameObject.transform.localPosition;
            StartCoroutine(Move(activate));
        }
        else {
            StopAllCoroutines();
            gameObject.transform.DOLocalMove(originalPosition, 0.5f);
        }
    }

    IEnumerator Move(bool activate)
    {
    while(activate) { 
        gameObject.transform.DOLocalMove(target, moveTime);
        yield return new WaitForSeconds(moveTime + 1f);

        gameObject.transform.DOLocalMove(originalPosition, moveTime);
        yield return new WaitForSeconds(moveTime);    
        }                                
    }
}

