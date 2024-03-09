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
    void Start()
    {
        this.gameObject.transform.localScale = new Vector3(1,1,1);
        Instance = this;
        SetValues();
        SetTexts();

        lowerDifficulty.onClick.AddListener(LowerDifficulty);
        higherDifficulty.onClick.AddListener(HigherDifficulty);
        higherDR.onClick.AddListener(HigherDR);
        lowerDR.onClick.AddListener(LowerDR);
        highEPS.onClick.AddListener(HigherEPS);
        lowEPS.onClick.AddListener(LowerEPS);
    }

    private void SetValues() {
        Difficulty = PlayerPrefs.GetString("Difficulty", "Normal");
        DivergenceRate = PlayerPrefs.GetInt("DivergenceRate", 22);
        EPS = PlayerPrefs.GetFloat("EPS", 1.1f);
    }
    private void SetTexts() {
        DifficultyText.text = Difficulty;
        divergenceRateText.text = "Divergence Rate: " + DivergenceRate.ToString(); // Convert DivergenceRate to string
        energyPerSecondText.text = "Energy Per Second: " + EPS.ToString();
    }

    void HigherEPS() {
        if(EPS <= 0.8f) {
            EPS = 0.8f;
        }

        else if(EPS >= 2.5f) {
                EPS = 2.5f;
        }
        else {
            EPS += 0.05f;
        }
        Difficulty= "Custom";
        SetTexts();
    }

    void LowerEPS() {
        if(EPS > 2.5f) {
            EPS = 2.5f;
        }
        else if(EPS <= 0.8f) {
            EPS = 0.8f;
        }
        else {
            EPS -= 0.05f;
        }
        Difficulty= "Custom";
        SetTexts();
    }

    void HigherDR() {
        if(DivergenceRate <= 15) {
            DivergenceRate = 15;
        }

        else if(DivergenceRate >= 40) {
            DivergenceRate = 40;
        }
        else {
            DivergenceRate += 1;
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
            DivergenceRate = 28;
            EPS = 0.95f;
        }
        else if(Difficulty == "Normal") {
            Difficulty = "Easy";
            DivergenceRate = 28;
            EPS = 1f;
        }
        else if(Difficulty == "Hard") {
            Difficulty = "Normal";
            DivergenceRate = 22;
            EPS = 1.1f;
        }
        else if(Difficulty == "Custom") {
            Difficulty = "Normal";
            DivergenceRate = 22;
            EPS = 1.1f;
        }
        SetTexts();
    }
    
    void HigherDifficulty() {
        if(Difficulty == "Easy") {
            Difficulty = "Normal";
            DivergenceRate = 22;
            EPS = 1.1f;
        }
        else if(Difficulty == "Normal") {
            Difficulty = "Hard";
            DivergenceRate = 18;
            EPS = 1.3f;
        }
        else if(Difficulty == "Hard") {
            Difficulty = "Hard";
            DivergenceRate = 18;
            EPS = 1.3f;
        }
        else if(Difficulty == "Custom") {
            Difficulty = "Normal";
            DivergenceRate = 22;
            EPS = 1.1f;
        }
        SetTexts();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
