using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RoomSelection : MonoBehaviour
{
    public GameObject RoomSelector;

    public static RoomSelection Instance {get; private set;}

    public static string CurrentlySelected {get; private set;}

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        RoomSelector = GameObject.Find("Room Selector");
    }

    public void Select(GameObject selectedObject) {
        if(selectedObject.GetComponent<Toggle>().isOn) {
            CurrentlySelected = selectedObject.name;
            List<Toggle> toggles = new List<Toggle>(RoomSelector.GetComponentsInChildren<Toggle>());
            toggles.Remove(selectedObject.GetComponent<Toggle>());
            foreach(Toggle t in toggles) {
                 // Temporarily remove the event listener
                t.onValueChanged.RemoveAllListeners();
                t.isOn = false;
                // Add the event listener back
                t.onValueChanged.AddListener(delegate {
                    Select(t.gameObject);
                });
            }
            selectedObject.GetComponent<Toggle>().isOn = true;
        }
        else {
            CurrentlySelected = null;
        }
    }

    public void ResetToggles(){
        int childCount = RoomSelector.transform.childCount;
        CurrentlySelected = null;
        // Iterate over all children
        for (int i = 0; i < childCount; i++) {
            // Get the child at the current index
            Transform child = RoomSelector.transform.GetChild(i);

            // Do something with the child
            Toggle toggle = child.GetComponent<Toggle>();
            if(toggle == null) {
                return;
            }
            toggle.isOn = false;
        }
    }

    // Update is called once per frame
    void Update()
    {   
    }
}
