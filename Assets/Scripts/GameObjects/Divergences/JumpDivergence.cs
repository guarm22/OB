using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpDivergence : CustomDivergence {
    public override void DoDivergenceAction(bool activate, GameObject gameObject) {
        Debug.Log("Custom jump here!" + "..." + activate);
    }
}

