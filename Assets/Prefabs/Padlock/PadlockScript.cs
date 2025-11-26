using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PadlockScript : MonoBehaviour {
    GameObject upperPart;
    public bool locked = true;

    void Start() {
        upperPart = transform.GetChild(0).gameObject;
    } 

    public void Unlock() {
        upperPart.transform.localPosition = new Vector3(upperPart.transform.localPosition.x, 0.068f, 0);
        locked = false;
    }
    public void Lock() {
        upperPart.transform.localPosition -= new Vector3(upperPart.transform.localPosition.x, 0.0547f, 0);
        locked = true;
    }
}
