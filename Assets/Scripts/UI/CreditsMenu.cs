using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreditsMenu : MonoBehaviour {

    public Button backButton;
    // Start is called before the first frame update
    public GameObject defaultUI;

    public TMP_Text creditsText;

    private void BackButtonEvent() {
        defaultUI.SetActive(true);
        this.gameObject.SetActive(false);
    }

    void Start() {
        backButton.onClick.AddListener(BackButtonEvent);
        creditsText.text = @"The Puncture
        Created by Bladerift Games

        Lead by:
        Programming, Game Design, and Level Design: Michael Guarrasi
        
        Art, Game Design, and UI/UX Design: Maria Oswald
             
        3D Modeling, Asset Creation, and Level Design: Matthieu Frenette
        
        Playtesters:
        Joseph Triolo
        Kevin Moran
        Nolan Bohn
        Faith Laliberte
        REEE Gang Members - Doc, Chickin, and CStan
        Aggressive Napkin Members - Silver, Arcima, Noley, and Rabboni

        Music
        Main Menu Theme - 'atmosphärischer loop 128 bpm 8972' by van_Wiese on pixabay

        Sounds
        All in game sounds created by Michael Guarrasi or found on pixabay.com
        ";

    }

    void Update() {
        if(Input.GetKeyDown(KeyCode.Escape)) {
            BackButtonEvent();
        }
    }
}