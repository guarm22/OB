using System.Collections.Generic;
using UnityEngine;

public class CampsiteSecret : MonoBehaviour {

    public List<GameObject> buttons = new List<GameObject>();

    private float timer = 60f;

    private List<GameObject> buttonsPressed = new List<GameObject>();


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update() {
        if(PlayerUI.paused) {
            return;
        }

        if(Input.GetKeyDown(KeybindManager.instance.GetKeybind("Interact"))|| Input.GetKeyDown(KeyCode.Mouse0)) {
            foreach(GameObject button in buttons) {
                if(button.GetComponent<Outliner>().hovering) {

                    //different message for first button press vs subsequent presses, also starts a timer on the first button press
                    if(buttonsPressed.Count == 0) {
                        timer = 60f;
                        PlayerUI.Instance.StartCoroutine(PlayerUI.Instance.Acquisition("You hear a faint ticking noise..", "", ""));
                    }
                    else {
                        PlayerUI.Instance.StartCoroutine(PlayerUI.Instance.Acquisition("The ticking grows louder.", "", ""));
                    }

                    if(buttonsPressed.Contains(button) == false) {
                        buttonsPressed.Add(button);
                    }                    
                    break;
                }
            }
        }
        timer -= Time.deltaTime;
        if(timer <= 0) {
            buttonsPressed.Clear();        
       }
    }
}
