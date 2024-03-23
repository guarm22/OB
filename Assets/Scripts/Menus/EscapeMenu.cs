using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EscapeMenu : MonoBehaviour
{
    public GameObject escapeMenuUI;
    public GameObject defaultUI;
    public Button quitButton;
    public Button returnButton;

    public void ReturnToGame() {
        PlayerUI.paused = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        defaultUI.SetActive(true);
        escapeMenuUI.SetActive(false);
    }

    public void QuitGame() {
        SceneManager.LoadScene("MainMenuScene");
    }

    // Start is called before the first frame update
    void Start()
    {
        if(defaultUI == null) {
            defaultUI = GameObject.Find("DefaultUI");
        }
        quitButton.onClick.AddListener(QuitGame);
        returnButton.onClick.AddListener(ReturnToGame);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
