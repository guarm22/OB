using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameTimer : MonoBehaviour
{

    public float startTime = 60*15f;
    public float speedModifer = 1.0f;

    public Text text;

    public bool isTimerRunning;

    public float currentTime;
    // Start is called before the first frame update
    void Start()
    {
        currentTime = startTime;
        text = GetComponent<Text>();
        isTimerRunning = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentTime > 0 && isTimerRunning)
        {
            currentTime -= Time.deltaTime;
            text.text = "" + Mathf.FloorToInt(currentTime/60) + ":" + Mathf.FloorToInt(currentTime % 60);
;
        }
        else {
            isTimerRunning = false;
            GameSystem.Instance.EndGame();
        }
    }
}
