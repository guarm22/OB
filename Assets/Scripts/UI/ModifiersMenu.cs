using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ModifiersMenu : MonoBehaviour {
    [Header("UI Elements")]
    public Button backButton;
    public GameObject levelSelectionMenu;
    public TMP_Text scoreMultiplierText;
    public TMP_Text ModifierDescription;

    public List<Modifier> modifiersList = new List<Modifier>();

    [Header("Modifiers")]
    //Creature Overrun
    public Button coButton;
    public Image coImage;
    public float coScore = 1.2f;

    //Speed Bost
    public Button speedButton;
    public Image speedImage;
    public float speedScore = 0.8f;

    //No Warnings
    public Button noWarningButton;
    public Image noWarningImage;
    public float noWarningScore = 1.3f;

    //Darkness
    public Button darknessButton;
    public Image darknessImage;
    public float darknessScore = 1.5f;

    //RandomTeleporting
    public Button teleportButton;
    public Image teleportImage;
    public float teleportScore = 1.3f;


    //EnergySapped
    public Button energySappedButton;
    public Image energySappedImage;
    public float energySappedScore = 1.3f;

    private void ButtonEvent(String name) {
        Modifier m = modifiersList.Find(x => x.name.Equals(name));
        bool enabled = m.isEnabled;
        if(enabled) { m.Disable(); } 
        else { m.Enable(); }
    }

    private void BackButtonEvent() {
        PlayerPrefs.SetFloat("CurrentScoreMultiplier", getScoreMultiplier());
        this.gameObject.SetActive(false);
        levelSelectionMenu.SetActive(true);
    }

    void Start() {
        backButton.onClick.AddListener(BackButtonEvent);
        scoreMultiplierText.text = "Current Score Multiplier: " + getScoreMultiplier();
        Init();
        ChangeModifierDesc("Hover over a modifier to see its description.", "");
    }

    private void Init() {
        //modifier creation
        //this, name, button, image, score modifier, description, ButtonEvent function with name
        //creature overrun
        Modifier coM = new Modifier(this, "Creature Overrun", coButton, coImage, coScore, 
        "A creature spawns at every possible opportunity.",
        ()=>ButtonEvent("Creature Overrun"));
        PlayerPrefs.SetInt("Creature Overrun", coM.isEnabled ? 1 : 0);

        //speed boost
        Modifier speedM = new Modifier(this, "Speed Boost", speedButton, speedImage, speedScore, 
        "Increases player speed.",
        ()=>ButtonEvent("Speed Boost"));
        PlayerPrefs.SetInt("Speed Boost", speedM.isEnabled ? 1 : 0);

        //no warnings
        Modifier noWarningM = new Modifier(this, "No Warnings", noWarningButton, noWarningImage, noWarningScore, 
        "Disables all warnings. Includes divergence alerts and report menu alerts.",
        ()=>ButtonEvent("No Warnings"));
        PlayerPrefs.SetInt("No Warnings", noWarningM.isEnabled ? 1 : 0);

        //darkess
        Modifier darknessM = new Modifier(this, "Darkness", darknessButton, darknessImage, darknessScore, 
        "Darkens the world. Flashlight no longer costs energy.",
        ()=>ButtonEvent("Darkness"));
        PlayerPrefs.SetInt("Darkness", darknessM.isEnabled ? 1 : 0);

        //random teleports
        Modifier teleportM = new Modifier(this, "Teleport", teleportButton, teleportImage, teleportScore, 
        "Causes the player to teleport at random intervals.",
        ()=>ButtonEvent("Teleport"));
        PlayerPrefs.SetInt("Teleport", teleportM.isEnabled ? 1 : 0);

        Modifier energySappedM = new Modifier(this, "Energy Sapped", energySappedButton, energySappedImage, energySappedScore,
        "Incorrect reports cost more energy. Creatures drain energy on contact. Creatures that already drain energy will drain more.",
        ()=>ButtonEvent("Energy Sapped"));
        PlayerPrefs.SetInt("EnergySapped", energySappedM.isEnabled ? 1 : 0);
        
        modifiersList.Add(coM);
        modifiersList.Add(speedM);
        modifiersList.Add(noWarningM);
        modifiersList.Add(darknessM);
        modifiersList.Add(teleportM);
        modifiersList.Add(energySappedM);
    }

    public void ChangeModifierDesc(string modName, string newDesc) {
        ModifierDescription.text = modName + "" + newDesc;
    }

    private float getScoreMultiplier() {
        return modifiersList.Where(x => x.isEnabled==true).Aggregate(1f, (acc, n) => acc * n.scoreModifier);
    }

    void Update() {
        if(Input.GetKeyDown(KeyCode.Escape)) {
            BackButtonEvent();
        }
        scoreMultiplierText.text = "Current Score Multiplier: " + getScoreMultiplier();

        if(Input.GetKeyDown(KeyCode.Backspace)) {
            modifiersList.ForEach(x => Debug.Log(x.name + ": " +x.isEnabled));
        }
    }

    public class Modifier {
        public String name;
        public Button button;
        public Image image;
        public float scoreModifier;
        public String modDesc;
        public UnityAction action;
        public ModifiersMenu menu;

        public bool isEnabled;

        //constructor
        public Modifier(ModifiersMenu m, String n, Button b, Image i, float s, String desc, UnityAction buttonMethod) {
            menu = m;
            name = n;
            button = b;
            image = i;
            modDesc = desc;
            scoreModifier = s;
            action = buttonMethod;
            isEnabled = PlayerPrefs.GetInt(name, 0) == 1 ? true : false;
            Init(button, image);
        }

        private void Init(Button b, Image i) {
            SpriteChange();
            b.onClick.AddListener(action);

            var trigger = b.gameObject.AddComponent<EventTrigger>();
            var entryEnter = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
            entryEnter.callback.AddListener((_) => menu.ChangeModifierDesc(name + ": ", modDesc));
            trigger.triggers.Add(entryEnter); 
        }

        public int Enable() {
            if(isEnabled) {
                return 0;
            }
            else {
                PlayerPrefs.SetInt(name, 1);
                this.isEnabled = true;
                SpriteChange();
                return 1;
            }
        }

        public int Disable() {
            if(!isEnabled) {
                return 0;
            }
            else {
                PlayerPrefs.SetInt(name, 0);
                this.isEnabled = false;
                SpriteChange();
                return 1;
            }
        }

        private void SpriteChange() {
            if(!this.isEnabled) {
                //gray image
                image.color = Color.gray;
                image.color = new Color(image.color.r, image.color.g, image.color.b, 0.5f);
            }
            else {
                //change image to normal
                image.color = Color.white;
            }
        }
    }
}
