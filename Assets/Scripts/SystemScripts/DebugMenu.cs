using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DebugMenu : MonoBehaviour
{
    public TMP_Text chivoPrefab;
    public GameObject spawnPoint;

    List<String> debugTexts = new List<String>();
    List<TMP_Text> debugTextObjects = new List<TMP_Text>();

    // Start is called before the first frame update
    void Start() {
        debugTexts.Add("Divergence Rate"); //0
        debugTexts.Add("Time Until Next Divergence"); //1
        debugTexts.Add("Current Divergences"); //2
        debugTexts.Add("Creature Rate"); //3
        debugTexts.Add("Time Until Next Creature"); //4
        debugTexts.Add("Current Creatures"); //5

        for(int i = 0; i < debugTexts.Count; i++) {
            TMP_Text text = Instantiate(chivoPrefab, spawnPoint.transform.position, Quaternion.identity);
            text.text = debugTexts[i];
            text.transform.SetParent(spawnPoint.transform);
            text.transform.position = new Vector3(spawnPoint.transform.position.x, spawnPoint.transform.position.y - (i * 50), spawnPoint.transform.position.z);
            debugTextObjects.Add(text);
        }
    }

    // Update is called once per frame
    void Update() {
        debugTextObjects[0].text = "Divergence Rate: " + DivergenceControl.Instance.DivergenceInterval;
        debugTextObjects[1].text = "Time Until Next Divergence: " + 
        (DivergenceControl.Instance.DivergenceInterval - 
        (Time.time - DivergenceControl.Instance.lastDivergenceTime +DivergenceControl.Instance.currentRandomness))%10;

        debugTextObjects[2].text = "Current Divergences: " + DivergenceControl.Instance.DivergenceList.Count;
        debugTextObjects[3].text = "Creature Rate: " + CreatureControl.Instance.creatureSpawnRate;
        debugTextObjects[4].text = "Time Until Next Creature: " + (CreatureControl.Instance.creatureSpawnRate - CreatureControl.Instance.timeSinceLastCreature);
        debugTextObjects[5].text = "Current Creatures: " + CreatureControl.Instance.TotalCreatures;

    }
}