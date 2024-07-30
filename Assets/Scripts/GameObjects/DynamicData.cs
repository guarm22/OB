using UnityEngine;

public class DynamicData : MonoBehaviour {
    public ANOMALY_TYPE type;
    public int energyCost = 25;
    [HideInInspector]
    public CustomDivergence customDivergence;
    public string difficulty = "All";
}
