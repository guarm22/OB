using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameSettings : MonoBehaviour
{
    // Start is called before the first frame update
    public static GameSettings Instance;
    public string Difficulty;
    public int DivergenceRate;
    public float EPS;

    public Button lowerDifficulty;
    public Button higherDifficulty;

    public TMP_Text DifficultyText;
    public TMP_Text divergenceRateText;
    public Button higherDR;
    public Button lowerDR;

    public TMP_Text energyPerSecondText;
    public Button highEPS;
    public Button lowEPS;

    public TMP_Text creatureThresholdText;
    public Button highCT;
    public Button lowCT;
    public int creatureThreshold;

    public TMP_Text gracePeriodText;
    public Button highGP;
    public Button lowGP;
    public int gracePeriod;

    void Start()
    {
        Instance = this;
        LoadValues();
        SetTexts();

        lowerDifficulty.onClick.AddListener(LowerDifficulty);
        higherDifficulty.onClick.AddListener(HigherDifficulty);
        higherDR.onClick.AddListener(HigherDR);
        lowerDR.onClick.AddListener(LowerDR);
        highEPS.onClick.AddListener(HigherEPS);
        lowEPS.onClick.AddListener(LowerEPS);
        highCT.onClick.AddListener(HigherCT);
        lowCT.onClick.AddListener(LowerCT);
        highGP.onClick.AddListener(HigherGP);
        lowGP.onClick.AddListener(LowerGP);
    }

    public void SetValues() {
        PlayerPrefs.SetString("Difficulty", Difficulty);
        PlayerPrefs.SetInt("DivergenceRate", DivergenceRate);
        PlayerPrefs.SetFloat("EPS", EPS);
        PlayerPrefs.SetInt("MaxDivergences", creatureThreshold);
        PlayerPrefs.SetInt("GracePeriod", gracePeriod);
    }

    private void LoadValues() {
        Difficulty = PlayerPrefs.GetString("Difficulty", "Normal");
        DivergenceRate = PlayerPrefs.GetInt("DivergenceRate", 21);
        EPS = PlayerPrefs.GetFloat("EPS", 1.1f);
        creatureThreshold = PlayerPrefs.GetInt("MaxDivergences", 4);
        gracePeriod = PlayerPrefs.GetInt("GracePeriod", 15);
    }
    private void SetTexts() {
        DifficultyText.text = Difficulty;
        divergenceRateText.text = "Divergence Rate: " + DivergenceRate.ToString(); // Convert DivergenceRate to string
        energyPerSecondText.text = "Energy Per Second: " + EPS.ToString();
        creatureThresholdText.text = "Creature Threshold: " + creatureThreshold.ToString();
        gracePeriodText.text = "Grace Period: " + gracePeriod.ToString();
    }

    void HigherCT() {
        creatureThreshold += 1;
        if(creatureThreshold <= 0) {
            creatureThreshold = 0;
        }
        else if(creatureThreshold >= 5) {
            creatureThreshold = 5;
        }
        SetTexts();
    }

    void LowerCT() {
        creatureThreshold -= 1;
        if(creatureThreshold > 6) {
            creatureThreshold = 6;
        }
        else if(creatureThreshold <= 0) {
            creatureThreshold = 0;
        }
        SetTexts();
    }

    void HigherGP() {
        gracePeriod += 1;
        if(gracePeriod <= 0) {
            gracePeriod = 0;
        }
        else if(gracePeriod >= 20) {
            gracePeriod = 20;
        }
        SetTexts();
    }

    void LowerGP() {
        gracePeriod -= 1;
        if(gracePeriod > 25) {
            gracePeriod = 25;
        }
        else if(gracePeriod <= 5) {
            gracePeriod = 5;
        }
        SetTexts();
    }

    void HigherEPS() {
        EPS += 0.05f;
        if(EPS <= 0.8f) {
            EPS = 0.8f;
        }

        else if(EPS >= 2.5f) {
                EPS = 2.5f;
        }
        Difficulty= "Custom";
        SetTexts();
    }

    void LowerEPS() {
        EPS -= 0.05f;
        if(EPS > 2.5f) {
            EPS = 2.5f;
        }
        else if(EPS <= 0.8f) {
            EPS = 0.8f;
        }
        Difficulty= "Custom";
        SetTexts();
    }

    void HigherDR() {
        DivergenceRate += 1;
        if(DivergenceRate <= 15) {
            DivergenceRate = 15;
        }

        else if(DivergenceRate >= 40) {
            DivergenceRate = 40;
        }
        Difficulty= "Custom";
        SetTexts();
    }

    void LowerDR() {
        DivergenceRate -= 1;
        if(DivergenceRate >= 40) {
            DivergenceRate = 40;
        }
        else if(DivergenceRate <= 15) {
            DivergenceRate = 15;
        }
        SetTexts();
    }

    void LowerDifficulty() {
        if(Difficulty == "Easy") {
            Difficulty = "Easy";
            DivergenceRate = 23;
            EPS = 1f;
            creatureThreshold = 4;
            gracePeriod = 20;
        }
        else if(Difficulty == "Normal") {
            Difficulty = "Easy";
            DivergenceRate = 23;
            EPS = 1f;
            creatureThreshold = 4;
            gracePeriod = 20;
        }
        else if(Difficulty == "Hard") {
            Difficulty = "Normal";
            DivergenceRate = 21;
            EPS = 1.1f;
            creatureThreshold = 4;
            gracePeriod = 15;
        }
        else if(Difficulty == "Custom") {
            Difficulty = "Normal";
            DivergenceRate = 21;
            EPS = 1.1f;
            creatureThreshold = 4;
            gracePeriod = 15;
        }
        SetTexts();
    }
    
    void HigherDifficulty() {
        if(Difficulty == "Easy") {
            Difficulty = "Normal";
            DivergenceRate = 21;
            EPS = 1.1f;
            creatureThreshold = 4;
            gracePeriod = 15;
        }
        else if(Difficulty == "Normal") {
            Difficulty = "Hard";
            DivergenceRate = 18;
            EPS = 1.3f;
            creatureThreshold = 4;
            gracePeriod = 5;
        }
        else if(Difficulty == "Hard") {
            Difficulty = "Hard";
            DivergenceRate = 18;
            EPS = 1.3f;
            creatureThreshold = 4;
            gracePeriod = 5;
        }
        else if(Difficulty == "Custom") {
            Difficulty = "Normal";
            DivergenceRate = 21;
            EPS = 1.1f;
            creatureThreshold = 4;
            gracePeriod = 15;
        }
        SetTexts();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
