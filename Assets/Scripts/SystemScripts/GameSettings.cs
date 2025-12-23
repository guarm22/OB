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

    public int creatureThreshold;

    public int gracePeriod;

    public float creatureSpawnRate;

    public bool hintsEnabled;

    public const int NormalDivergenceRate = 30;
    public const int NormalCreatureThreshold = 4;
    public const int NormalGracePeriod = 24;
    public const float NormalEPS = 1.65f;
    public const float NormalCreatureSpawnRate = 28f;


    void Start() {
        Instance = this;
        LoadValues();
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

    public void setDifficulty(string diff) {
        PlayerPrefs.SetString("Difficulty", diff);
        DivergenceRate = GetDivergenceRate(diff);
        EPS = getEPS(diff);
        creatureThreshold = getCreatureThreshold(diff);
        gracePeriod = getGracePeriod(diff);
        creatureSpawnRate = GetCreatureSpawnRate(diff);
        hintsEnabled = getHints(diff);
    }

    private float GetCreatureSpawnRate(string diff) {
        if(diff == "Easy") {
            return 34f;
        }
        else if(diff == "Normal") {
            return 27f;
        }
        else if(diff == "Hard") {
            return 20f;
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
            return 45;
        }
        else if(diff == "Normal") {
            return 32;
        }
        else if(diff == "Hard") {
            return 24;
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
            return 45;
        }
        else if(diff == "Normal") {
            return 30;
        }
        else if(diff == "Hard") {
            return 15;
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
            return 1.7f;
        }
        else if(diff == "Normal") {
            return 1.75f;
        }
        else if(diff == "Hard") {
            return 1.8f;
        }
        else if(diff == "Nightmare") {
            return 2f;
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
}
