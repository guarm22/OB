using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicDifficulty : MonoBehaviour
{
    public float timeBeforeDRIncrease = 60f;
    public int DRIncrease = 2;
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        /* if(Time.time - GameSystem.Instance.lastCorrectGuessTime > timeBeforeDRIncrease) {
            GameSystem.Instance.ChangeDivergenceRate(GameSystem.Instance.GameObjectDisappearanceInterval + DRIncrease);
            GameSystem.Instance.lastCorrectGuessTime = Time.time-1.5f;
        }

        //lower divergence rate if player gets correct guess, and the rate is not below the player's set divergence rate
        if(Time.time - GameSystem.Instance.lastCorrectGuessTime <= 1 && GameSystem.Instance.lastCorrectGuessTime > 0
            && GameSystem.Instance.GameObjectDisappearanceInterval > PlayerPrefs.GetInt("DivergenceRate")) {
            GameSystem.Instance.ChangeDivergenceRate(GameSystem.Instance.GameObjectDisappearanceInterval - DRIncrease);
            GameSystem.Instance.lastCorrectGuessTime = Time.time-1.5f;
        } */
    }
}
