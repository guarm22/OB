using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CompendiumMenu : MonoBehaviour {
    public List<Button> buttons = new List<Button>();
    public GameObject description;
    private List<string> descriptions = new List<string> {
        "Zombies are a common sight in areas affected by The Puncture. They move quickly and attack with their claws, causing matter to be warped.",
        "Lurkers attempt to get the jump on their targets by hiding around corners or objects. They attempt to disorient the target with a loud sound and then attack.",
        "Hiders like to stalk their prey from the shadows. They are known to be able to siphon energy from afar.",
        "Chasers are known for their speed and agility. With their small frame, they are hard to spot. However, their sounds give them away.",
        "Enders are the most dangerous type of creature. They do not adhere to any known laws of physics, and their attacks are always lethal. However, it seems they only appear in areas with massive energy fluctuations."
    };

    public GameObject outline;

    private void SetDescription(String selection) {
        Button b = buttons.Find(b => b.name == selection);
        description.GetComponent<TMPro.TextMeshProUGUI>().text = descriptions[buttons.FindIndex(b => b.name == selection)];

        //make the parent of the outline this button
        outline.transform.SetParent(b.transform);

        //change color of button text to show it is selected
        foreach(Button button in buttons) {
            if(button == b) {
                //hex code for a light blue color
                button.GetComponentInChildren<TMPro.TextMeshProUGUI>().color = UIInfo.SelectedColor;
            }
            else {
                button.GetComponentInChildren<TMPro.TextMeshProUGUI>().color = UIInfo.UnselectedColor;
            }
        }
    }

    void Start() {
        SetDescription("Zombie");
        for(int i = 0; i < buttons.Count; i++) {
            int index = i;
            buttons[i].onClick.AddListener(() => SetDescription(buttons[index].name));
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
