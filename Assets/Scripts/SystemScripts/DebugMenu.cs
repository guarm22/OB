using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DebugMenu : MonoBehaviour
{
    public TMP_Text divRate;
    public TMP_Text amtOfDivs;
    public TMP_Text amtOfCreatures;
    public TMP_Text creatureSpawnrate;
    public TMP_Text creatureThreshold;
    public TMP_Text EPS;
    public TMP_Text gracePeriod;
    public TMP_Text currentRandomness;

    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        divRate.text = "Divergence Rate: " + GameSystem.Instance.GameObjectDisappearanceInterval;
        amtOfDivs.text = "Amount of Divergences: " + GameSystem.Instance.TotalAnomalies;
        amtOfCreatures.text = "Amount of Creatures: " + CreatureControl.Instance.TotalCreatures;
        creatureSpawnrate.text = "Creature Spawnrate: " + CreatureControl.Instance.creatureSpawnRate;
        creatureThreshold.text = "Creature Threshold: " + CreatureControl.Instance.creatureMax;
        EPS.text = "EPS: " + GameSystem.Instance.energyPerSecond;
        gracePeriod.text = "Grace Period: " + GameSystem.Instance.GameStartTime;
        currentRandomness.text = "Current Randomness: " + GameSystem.Instance.currentRandomness;
    }
}
