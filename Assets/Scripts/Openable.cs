using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Openable: MonoBehaviour {

    public float openDistance = 4.0f;

    private Animator animator;
    private Transform player;

    // Start is called before the first frame update
    void Start() {
        animator = gameObject.GetComponent<Animator>();
        player = GameObject.FindWithTag("Player").GetComponent<Transform>();
        Debug.Log(player);
    }

    // Update is called once per frame
    void Update() {
        try {
            float dist = Vector3.Distance(player.position, gameObject.transform.position);
            animator.SetBool("isOpen", dist < openDistance);
        } catch (Exception e) {
            //Debug.Log(e);
        }
    }
}
