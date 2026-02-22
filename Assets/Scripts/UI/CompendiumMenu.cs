using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CompendiumMenu : MonoBehaviour {
    public GameObject buttonPrefab;
    public GameObject initialLocation;
    public GameObject description;
    private List<string> descriptions = new List<string> {
        "Zombies are a common sight in areas affected by The Puncture. They move quickly and attack with their claws, causing matter to be warped.",
        "Lurkers attempt to get the jump on their targets by hiding around corners or objects. They attempt to disorient the target with a loud sound and then attack.",
        "Hiders like to stalk their prey from the shadows. They are known to be able to siphon energy from afar.",
        "Chasers are known for their speed and agility. With their small frame, they are hard to spot. However, their sounds give them away.",
        "Enders are the most dangerous type of creature. They do not adhere to any known laws of physics, and their attacks are always lethal. However, it seems they only appear in areas with massive energy fluctuations."
    };

    private List<string> names = new List<string> {"Zombie", "Lurker", "Hider", "Chaser", "Ender"};

    void Start() {
        Init();
    }

    private void Init() {
        string firstShown = null;
        float y = 0f;
        int index = 0;
        foreach(string c in names) {

            GameObject entry = Instantiate(buttonPrefab, initialLocation.transform);
            entry.name = c + " Button";
            TMP_Text t = entry.GetComponentInChildren<TMP_Text>();
            entry.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, y);
            t.text = c;

            if(firstShown == null) {firstShown = c;}
            t.color = Color.white;
            Button rb = entry.GetComponent<Button>();
            entry.GetComponent<Button>().onClick.AddListener(() => {
                SetEntry(descriptions[names.IndexOf(c)]);
            });
                
            y -= 100;
        }
        if(firstShown != null) {SetEntry(descriptions[names.IndexOf(firstShown)]);}
    }

    private void SetEntry(String entry)
    {
        description.GetComponent<TMPro.TextMeshProUGUI>().text = entry;
    }
}
