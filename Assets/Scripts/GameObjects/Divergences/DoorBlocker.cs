using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorBlocker : CustomDivergence {
    public Vector3 OriginalScale;
    private GameObject player;

    void Start() {
        player = GameObject.Find("Player");
    }

    void Update() {
        if(transform.localScale == Vector3.zero) {
            return;
        }
        if(Vector3.Distance(player.transform.position, transform.position) < 1) {
            StartCoroutine(GameSystem.Instance.EndGame("puncture"));
        }
    }

    public override void DoDivergenceAction(bool activate, DynamicObject gameObject) {
        if (activate) {
            transform.localScale = OriginalScale;
        }
        else {
            transform.localScale = Vector3.zero;
        }
    }
}
