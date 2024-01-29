using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System;

public class MainMenu : MonoBehaviour
{
    public GameObject DefaultMenu;

    public GameObject LevelSelectionMenu;

    public GameObject Settings;


    static int GetLevelInt(string name) {
        switch(name) {
            case "Test":
                return 1;

            case "The Puncture":
                return 2;

            default:
                return 0;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

     if(Input.GetMouseButtonDown(0)) {
            if (EventSystem.current.IsPointerOverGameObject()) {
                // Get a reference to the selected UI game object
                GameObject selectedObject = EventSystem.current.currentSelectedGameObject;

                if(selectedObject == null) {
                    return;
                }
                
                if(selectedObject.name.Equals("ExitGame")) {
                    Application.Quit();
                }
                else if(selectedObject.name.Equals("ChooseLevel")) {
                    DefaultMenu.SetActive(false);
                    LevelSelectionMenu.SetActive(true);
                }

                else if(selectedObject.name.Equals("LevelBack")) {
                    DefaultMenu.SetActive(true);
                    LevelSelectionMenu.SetActive(false);
                }

                else if(selectedObject.name.Equals("SettingsButton")) {
                    DefaultMenu.SetActive(false);
                    Settings.SetActive(true);
                }

                else if(selectedObject.name.Equals("SettingsBack")) {
                    DefaultMenu.SetActive(true);
                    Settings.SetActive(false);
                }


                //Level Selection
                else {
                    try {
                        SceneManager.LoadScene(GetLevelInt(selectedObject.name));
                    }
                    catch(Exception e) {
                        Debug.Log("Level does not exist: " +e);
                    }
                }
                
            }
        }

    }
}
