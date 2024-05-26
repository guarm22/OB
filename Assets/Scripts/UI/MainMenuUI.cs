using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    public Button Play;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Test");
        Play.onClick.AddListener(Test);
    }
    void Test(){
        Debug.Log("Test");
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
