using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeLight : CustomDivergence {
    private float originalLevel;
    public float changePos = 3f;
    public float changeNeg = -3f;
    public override void DoDivergenceAction(bool enable, DynamicObject obj) {
        if(enable) {
            originalLevel = obj.Obj.GetComponent<Light>().intensity;
            float random = Random.Range(-1f, 1f);
            float amt;
            if (random < 0) {
                amt = changeNeg;
            }
            else {
                amt = changePos;
            }
            obj.Obj.GetComponent<Light>().intensity += amt;
        }
        else {
            obj.Obj.GetComponent<Light>().intensity = originalLevel;
        }
    }

}

