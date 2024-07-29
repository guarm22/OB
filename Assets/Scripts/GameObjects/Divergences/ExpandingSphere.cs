using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpandingSphere : CustomDivergence {
    public float expansionSpeed;
    private Vector3 expandScale;
    private GameObject player;

    void Start() {
        expandScale = new Vector3(expansionSpeed, expansionSpeed, expansionSpeed);
        player = GameObject.Find("Player");
        ExpandSphere(this.gameObject);
    }

    public override void DoDivergenceAction(bool activate, DynamicObject gameObject) {
        if (activate) {
            StartCoroutine(ExpandSphere(this.gameObject));
        }
        else {
            StopAllCoroutines();
            transform.localScale = Vector3.zero;
        }
    }

    public IEnumerator ExpandSphere(GameObject obj) {
        float time = 0;
        while (time < 100) {
            //if colliding with player, kill them (end game)
            if (Vector3.Distance(player.transform.position, obj.transform.position) < 1) {
                StartCoroutine(GameSystem.Instance.EndGame("puncture"));
                break;
            }            

            time += Time.deltaTime;
            obj.transform.localScale = expandScale * time;
            yield return null;
        }
    }
}
