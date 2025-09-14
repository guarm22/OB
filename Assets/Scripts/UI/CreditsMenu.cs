using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreditsMenu : MonoBehaviour {

    public Button backButton;
    // Start is called before the first frame update
    public GameObject defaultUI;

    public TMP_Text patchNotes;

    private void BackButtonEvent() {
        defaultUI.SetActive(true);
        this.gameObject.SetActive(false);
    }

    void Start() {
        backButton.onClick.AddListener(BackButtonEvent);
        patchNotes.text = @"WIP";

    }

    void Update() {
        if(Input.GetKeyDown(KeyCode.Escape)) {
            BackButtonEvent();
        }
    }
}