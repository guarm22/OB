using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Puncture : MonoBehaviour {

    private float count = 1;

    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        count += Time.deltaTime * 0.6f;
        Shader.SetGlobalFloat("_angle", count);
        Debug.Log(count);
    }
}
