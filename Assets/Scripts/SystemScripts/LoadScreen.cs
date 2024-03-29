using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadScreen : MonoBehaviour
{
    public bool Loading;
    public static LoadScreen Instance;
    public GameObject LoadingScreen;
    public void StartLoading() {
        LoadingScreen.SetActive(true);
    }
    
    void Start() {
        Instance = this;
    }

    void Update() {
        if(Loading) {
            StartLoading();
        }
    }
}
