using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ReportPrompt : MonoBehaviour {
    public TMP_Text promptText;
    public Image promptBackground;
    public TMP_Text reportText;
    private Vector3 reportTextOriginalPos;

    void Start() {
        reportTextOriginalPos = reportText.rectTransform.position;
        UpdatePrompt();
    }

    void OnEnable() {
        UpdatePrompt();
    }

    void UpdatePrompt() {
        if(KeybindManager.instance == null) {
            return;
        }
        string currentKeybind = KeybindManager.instance.GetKeybindString("Report Menu");
        promptText.text = currentKeybind.ToUpper();
        promptBackground.rectTransform.sizeDelta = new Vector2(currentKeybind.Length*35 - (currentKeybind.Length), 50);
    }
}
