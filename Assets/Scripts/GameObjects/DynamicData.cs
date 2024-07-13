using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicData : MonoBehaviour
{
    public ANOMALY_TYPE type;
    public int energyCost = 25;
    [HideInInspector]
    public CustomDivergence customDivergence;
}
