using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JournalPopup : MonoBehaviour
{

    public static JournalPopup Instance {get; private set;}

    public Text RightPage;
    public Text LeftPage;

    string LeftText;
    string RightText;


    public void SetText(string[] text) {

        LeftText = text[0];
        LeftPage.text = LeftText;
        

        if(text[1] != null) {
            RightText = text[1];
            RightPage.text = RightText;
        }

    }

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        LeftText = " ";
        RightText = " ";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
