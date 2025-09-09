using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ModifiersMenu : MonoBehaviour {
    
    public Button backButton;
    public GameObject levelSelectionMenu;

    public TMP_Text scoreMultiplierText;
    private int totalModifiersEnabled = 0;

    public TMP_Text ModifierDescription;


    //Creature Overrun
    public Button coButton;
    public Image coImage;

    //Speed Bost
    public Button speedButton;
    public Image speedImage;

    private void SpriteChange(int a, Image image, Button button) {
        bool activate = a==1;
        
        if(!activate) {
            //gray image
            image.color = Color.gray;
            image.color = new Color(image.color.r, image.color.g, image.color.b, 0.5f);

            
        }
        else {
            //change image to normal
            image.color = Color.white;
        }
    }

    private int NewEnabled(int a) {
        return a == 0 ? 1 : 0;
    }

    private void coButtonEvent() {
        int enabled = PlayerPrefs.GetInt("CreatureOverrun", 0);
        int newEnabled = NewEnabled(enabled);
        PlayerPrefs.SetInt("CreatureOverrun", newEnabled);
        SpriteChange(newEnabled, coImage, coButton);

        totalModifiersEnabled += newEnabled==1 ? 1 : -1;
    }

    private void speedButtonEvent() {
        int enabled = PlayerPrefs.GetInt("SpeedModifier", 0);
        int newEnabled = NewEnabled(enabled);
        PlayerPrefs.SetInt("SpeedModifier", newEnabled);
        SpriteChange(newEnabled, speedImage, speedButton);

        totalModifiersEnabled += newEnabled==1 ? 1 : -1;
    }

    private void BackButtonEvent() {
        PlayerPrefs.SetFloat("CurrentScoreMultiplier", getScoreMultiplier());
        this.gameObject.SetActive(false);
        levelSelectionMenu.SetActive(true);
    }

    void Start() {
        backButton.onClick.AddListener(BackButtonEvent);
        scoreMultiplierText.text = "Current Score Multiplier: " + getScoreMultiplier();
        ChangeModifierDesc("Hover over a modifier to see its description.", "");
        Init();
    }

    private void Init() {
        //creature overrun
        int co = PlayerPrefs.GetInt("CreatureOverrun");
        SpriteChange(co, coImage, coButton);
        coButton.onClick.AddListener(coButtonEvent);
        totalModifiersEnabled += co;

        var trigger = coButton.gameObject.AddComponent<EventTrigger>();
        var entryEnter = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
        entryEnter.callback.AddListener((_) => ChangeModifierDesc("Creature Overrun: ", "A creature spawns at every possible chance."));
        trigger.triggers.Add(entryEnter);


        //speed
        int speed = PlayerPrefs.GetInt("SpeedModifier");
        SpriteChange(speed, speedImage, speedButton);
        speedButton.onClick.AddListener(speedButtonEvent);
        totalModifiersEnabled += speed;
        
        trigger = speedButton.gameObject.AddComponent<EventTrigger>();
        entryEnter = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
        entryEnter.callback.AddListener((_) => ChangeModifierDesc("Speed Boost: ", "Increases Speed"));
        trigger.triggers.Add(entryEnter);



    }

    private void ChangeModifierDesc(string modName, string newDesc) {
        ModifierDescription.text = modName + "" + newDesc;
    }

    private float getScoreMultiplier() {
        return totalModifiersEnabled == 0 ? 1 : totalModifiersEnabled + (1*1.1f);
    }

    void Update() {
        if(Input.GetKeyDown(KeyCode.Escape)) {
            BackButtonEvent();
        }
        scoreMultiplierText.text = "Current Score Multiplier: " + getScoreMultiplier();
    }
}
