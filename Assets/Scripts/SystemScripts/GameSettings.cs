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

    public TMP_Text creatureSpawnrateText;
    public Button highCS;
    public Button lowCS;
    public float creatureSpawnRate;

    public Button hints;
    public TMP_Text hintsText;
    public bool hintsEnabled;

    public const int NormalDivergenceRate = 28;
    public const int NormalCreatureThreshold = 4;
    public const int NormalGracePeriod = 28;
    public const float NormalEPS = 1.1f;
    public const float NormalCreatureSpawnRate = 23f;


    void Start()
    {
        Instance = this;
        LoadValues();
        SetTexts();

        /*higherDR.onClick.AddListener(() => setDR(higherDR));
        lowerDR.onClick.AddListener(() => setDR(lowerDR));
        highEPS.onClick.AddListener(() => setEPS(highEPS));
        lowEPS.onClick.AddListener(() => setEPS(lowEPS));
        highCT.onClick.AddListener(() => setCT(highCT));
        lowCT.onClick.AddListener(() => setCT(lowCT));
        highGP.onClick.AddListener(() => setGP(highGP));
        lowGP.onClick.AddListener(() => setGP(lowGP));
        highCS.onClick.AddListener(() => setCS(highCS));
        lowCS.onClick.AddListener(() => setCS(lowCS));
        hints.onClick.AddListener(setHints);*/
    }

    public void SetValues() {
        PlayerPrefs.SetString("Difficulty", Difficulty);
        PlayerPrefs.SetInt("DivergenceRate", DivergenceRate);
        PlayerPrefs.SetFloat("EPS", EPS);
        PlayerPrefs.SetInt("MaxDivergences", creatureThreshold);
        PlayerPrefs.SetInt("GracePeriod", gracePeriod);
        PlayerPrefs.SetFloat("CreatureSpawnRate", creatureSpawnRate);
        PlayerPrefs.SetInt("HintsEnabled", hintsEnabled ? 1 : 0);
    }

    private void LoadValues() {
        Difficulty = PlayerPrefs.GetString("Difficulty", "Normal");
        DivergenceRate = PlayerPrefs.GetInt("DivergenceRate", NormalDivergenceRate);
        EPS = PlayerPrefs.GetFloat("EPS", NormalEPS);
        creatureThreshold = PlayerPrefs.GetInt("MaxDivergences", NormalCreatureThreshold);
        gracePeriod = PlayerPrefs.GetInt("GracePeriod", NormalGracePeriod);
        creatureSpawnRate = PlayerPrefs.GetFloat("CreatureSpawnRate", NormalCreatureSpawnRate);
        hintsEnabled = PlayerPrefs.GetInt("HintsEnabled", 1) == 1;
    }
    private void SetTexts() {
        DifficultyText.text = Difficulty;
        divergenceRateText.text = "Divergence Rate: " + DivergenceRate.ToString(); // Convert DivergenceRate to string
        energyPerSecondText.text = "Energy Per Second: " + EPS.ToString();
        creatureThresholdText.text = "Creature Threshold: " + creatureThreshold.ToString();
        gracePeriodText.text = "Grace Period: " + gracePeriod.ToString();
        creatureSpawnrateText.text = "Creature Spawn Rate: " + creatureSpawnRate.ToString();
        hintsText.text = hintsEnabled ? "Divergence Hints: Enabled" : "Divergence Hints: Disabled";
    }

    private void setCS(Button b) {
        Difficulty = "Custom";
        if(b.name.Contains("High")) {
            creatureSpawnRate += 0.5f;
        }
        else {
            creatureSpawnRate -= 0.5f;
        }
        if(creatureSpawnRate > 35) {
            creatureSpawnRate = 35f;
        }
        else if(creatureSpawnRate < 10) {
            creatureSpawnRate = 10f;
        }
        SetTexts();
    }

    private void setCT(Button b) {
        Difficulty = "Custom";
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
        Difficulty = "Custom";
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
        Difficulty = "Custom";
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
        Difficulty = "Custom";
        if(b.name.Contains("High")) {
            DivergenceRate += 1;
        }
        else {
            DivergenceRate -= 1;
        }
        if(DivergenceRate > 40) {
            DivergenceRate = 40;
        }
        else if(DivergenceRate < 10) {
            DivergenceRate = 10;
        }
        SetTexts();
    }
    void setHints() {
        Difficulty = "Custom";
        if(hintsEnabled) {
            hintsEnabled = false;
            hintsText.text = "Divergence Hints: Disabled";
        }
        else {
            hintsEnabled = true;
            hintsText.text = "Divergence Hints: Enabled";
        }
        SetTexts();
    }


    public void setDifficulty(string diff) {
        PlayerPrefs.SetString("Difficulty", diff);
        DivergenceRate = GetDivergenceRate(diff);
        EPS = getEPS(diff);
        creatureThreshold = getCreatureThreshold(diff);
        gracePeriod = getGracePeriod(diff);
        creatureSpawnRate = GetCreatureSpawnRate(diff);
        hintsEnabled = getHints(diff);
        SetTexts();
    }

    private float GetCreatureSpawnRate(string diff) {
        if(diff == "Easy") {
            return 30f;
        }
        else if(diff == "Normal") {
            return 23f;
        }
        else if(diff == "Hard") {
            return 18f;
        }
        else if(diff == "Nightmare") {
            return 16f;
        }
        else {
            return creatureSpawnRate;
        }
    }

    private int GetDivergenceRate(string diff) {
        if(diff == "Easy") {
            return 33;
        }
        else if(diff == "Normal") {
            return 28;
        }
        else if(diff == "Hard") {
            return 22;
        }
        else if(diff == "Nightmare") {
            return 15;
        }
        else {
            return DivergenceRate;
        }
    }

    private int getGracePeriod(string diff) {
        if(diff == "Easy") {
            return 30;
        }
        else if(diff == "Normal") {
            return 28;
        }
        else if(diff == "Hard") {
            return 16;
        }
        else if(diff == "Nightmare") {
            return 8;
        }
        else {
            return gracePeriod;
        }
    }

    private int getCreatureThreshold(string diff) {
        if(diff == "Easy") {
            return 5;
        }
        else if(diff == "Normal") {
            return 4;
        }
        else if(diff == "Hard") {
            return 3;
        }
        else if(diff == "Nightmare") {
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
            return 1.2f;
        }
        else if(diff == "Hard") {
            return 1.45f;
        }
        else if(diff == "Nightmare") {
            return 1.8f;
        }
        else {
            return EPS;
        }
    }

    private bool getHints(string diff) {
        if(diff == "Easy") {
            return true;
        }
        else if(diff == "Normal") {
            return true;
        }
        else if(diff == "Hard") {
            return false;
        }
        else if(diff == "Nightmare") {
            return false;
        }
        else {
            return hintsEnabled;
        }
    }

    // Update is called once per frame
    void Update() {
        
    }
}
