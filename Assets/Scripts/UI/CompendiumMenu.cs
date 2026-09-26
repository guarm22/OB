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
        "Disappearance is a phenomenon where objects vanish without a trace, often associated with areas of high energy fluctuation.\n\nThese vanishing events aren't just the removal of energy or matter, but also the ability to affect how they're perceived by those nearby.\n\n\n ...make sure you remember. Sometimes... I see things at the edge of my vision and they disappear when I look directly at them.",
        "Replacement occurs when an object or being is substituted for another, sometimes leading to confusion and disorientation.\n\nEven the material of an object can be replaced.\n\n\n\n The chair was made of flesh.",
        "Movement divergences are unexplained shifts in position or behavior, often linked to the presence of The Puncture.\n\nWhen you look at an object affected by a movement divergence, you may feel the object was given life. Do not believe that feeling.",
        "Audio disturbances are unusual, repeating, and continual sounds that can be heard in areas affected by The Puncture.\n\n\n\nThe toilet isn't actually flushing itself over and over again.",
        "Glitches are... different than the other divergences. You may feel your body change at the will of the Puncture.\n\nYou may not be able to control certain body parts the same as before, you may lose the ability to read.\n\n\nShe's trying to become one with me by using my body.",
        "The Puncture is a mysterious phenomenon that disrupts the normal flow of space and time, leading to unpredictable consequences.\n\nThey do not obey the normal laws of physics.",
        "It is unknown whether the creatures of the Puncture are living beings or simply matter controlled by the Puncture.\n\nThey don't appear to have a will of their own... simply acting as a distraction from the mission of stabilizing the Puncture.",
        "Addition refers to the unexpected appearance of new objects or beings in areas affected by The Puncture."
    };

    private List<string> names = new List<string> 
    {"Disappearance", "Replacement", "Movement", "Audio", "Glitch", "Puncture", "Creature", "Addition"};

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
