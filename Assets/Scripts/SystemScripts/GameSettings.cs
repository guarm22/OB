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
        higherDR.onClick.AddListener(() => setDR(higherDR));
        lowerDR.onClick.AddListener(() => setDR(lowerDR));
        highEPS.onClick.AddListener(() => setEPS(highEPS));
        lowEPS.onClick.AddListener(() => setEPS(lowEPS));
        highCT.onClick.AddListener(() => setCT(highCT));
        lowCT.onClick.AddListener(() => setCT(lowCT));
        highGP.onClick.AddListener(() => setGP(highGP));
        lowGP.onClick.AddListener(() => setGP(lowGP));
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

    private void setCT(Button b) {
        if(b.name.Contains("High")) {
            creatureThreshold += 1;
        }
        else {
            creatureThreshold -= 1;
        }
        if(creatureThreshold > 6) {
            creatureThreshold = 6;
        }
        else if(creatureThreshold < 0) {
            creatureThreshold = 0;
        }
        SetTexts();
    }
    private void setGP(Button b) {
        if(b.name.Contains("High")) {
            gracePeriod += 1;
        }
        else {
            gracePeriod -= 1;
        }
        if(gracePeriod > 25) {
            gracePeriod = 25;
        }
        else if(gracePeriod < 5) {
            gracePeriod = 5;
        }
        SetTexts();
    }

    private void setEPS(Button b) {
        if(b.name.Contains("High")) {
            EPS += 0.05f;
        }
        else {
            EPS -= 0.05f;
        }
        if(EPS > 2.5f) {
            EPS = 2.5f;
        }
        else if(EPS < 0.8f) {
            EPS = 0.8f;
        }
        SetTexts();
    }
    private void setDR(Button b) {
        if(b.name.Contains("High")) {
            DivergenceRate += 1;
        }
        else {
            DivergenceRate -= 1;
        }
        if(DivergenceRate > 40) {
            DivergenceRate = 40;
        }
        else if(DivergenceRate < 15) {
            DivergenceRate = 15;
        }
        SetTexts();
    }

    void LowerDifficulty() {
        if(Difficulty == "Easy") {
            Difficulty = "Easy";
        }
        else if(Difficulty == "Normal") {
            Difficulty = "Easy";
        }
        else if(Difficulty == "Hard") {
            Difficulty = "Normal";
        }
        else if(Difficulty == "Custom") {
            Difficulty = "Normal";
        }
        setDifficulty(Difficulty);
    }
    
    void HigherDifficulty() {
        if(Difficulty == "Easy") {
            Difficulty = "Normal";
        }
        else if(Difficulty == "Normal") {
            Difficulty = "Hard";
        }
        else if(Difficulty == "Hard") {
            Difficulty = "Hard";
        }
        else if(Difficulty == "Custom") {
            Difficulty = "Normal";
        }
        setDifficulty(Difficulty);
    }


    private void setDifficulty(string diff) {
        DivergenceRate = GetDivergenceRate(diff);
        EPS = getEPS(diff);
        creatureThreshold = getCreatureThreshold(diff);
        gracePeriod = getGracePeriod(diff);
        SetTexts();
    }

    private int GetDivergenceRate(string diff) {
        if(diff == "Easy") {
            return 23;
        }
        else if(diff == "Normal") {
            return 21;
        }
        else if(diff == "Hard") {
            return 18;
        }
        else {
            return DivergenceRate;
        }
    }

    private int getGracePeriod(string diff) {
        if(diff == "Easy") {
            return 20;
        }
        else if(diff == "Normal") {
            return 15;
        }
        else if(diff == "Hard") {
            return 5;
        }
        else {
            return gracePeriod;
        }
    }

    private int getCreatureThreshold(string diff) {
        if(diff == "Easy") {
            return 4;
        }
        else if(diff == "Normal") {
            return 4;
        }
        else if(diff == "Hard") {
            return 3;
        }
        else {
            return creatureThreshold;
        }
    }

    private float getEPS(string diff) {
        if(diff == "Easy") {
            return 1f;
        }
        else if(diff == "Normal") {
            return 1.1f;
        }
        else if(diff == "Hard") {
            return 1.3f;
        }
        else {
            return EPS;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
