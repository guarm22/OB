using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class JumpDivergence : CustomDivergence {

    public float jumpHeight = 1f;
    public override void DoDivergenceAction(bool activate, DynamicObject dynamic) {
        if(activate) {
            StartCoroutine(waiter(activate));
        }
        else {
            StopAllCoroutines();
            gameObject.transform.DOMove(dynamic.originalPosition, 0.5f);
        }
    }

    IEnumerator waiter(bool activate)
    {
    while(activate) { 
        float waitTime = 0.5f;
        Debug.Log(activate);
        gameObject.transform.DOMove(new Vector3(gameObject.transform.position.x,
                                                gameObject.transform.position.y + jumpHeight,
                                                gameObject.transform.position.z),
                                                waitTime);
        yield return new WaitForSeconds(waitTime);

        gameObject.transform.DOMove(new Vector3(gameObject.transform.position.x,
                                                gameObject.transform.position.y - jumpHeight,
                                                gameObject.transform.position.z),
                                                waitTime);
        yield return new WaitForSeconds(waitTime);    
        }                                
    }
}

