using UnityEngine;

[System.Serializable]
public class MenuPopupData {

    public string title;
    public string description;
    public string name;
    public bool doNotShowAgain;

    public MenuPopupData(string title, string description, string name, bool doNotShowAgain) {
        this.title = title;
        this.description = description;
        this.name = name;
        this.doNotShowAgain = doNotShowAgain;
    }
 
}
