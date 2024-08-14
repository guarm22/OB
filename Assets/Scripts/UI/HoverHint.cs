using System;
using TMPro;
using UnityEngine;

public class HoverHint : MonoBehaviour {
    
    public TMP_Text text;
    public String hintText;

    void Awake() {
        text = GetComponentInChildren<TMP_Text>();
        text.text = hintText;
        this.transform.localPosition = new Vector3(50, -50, 0);
    }

    public void SetText(string newText) {
        text.text = newText;
    }

    public void SetParent(Transform newParent) {
        transform.SetParent(newParent);
        transform.localPosition = Vector3.zero;
    }

}
