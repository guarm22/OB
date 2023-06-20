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
    }

    void Select(GameObject selectedObject) {
        int childCount = RoomSelector.transform.childCount;

        // Iterate over all children
        for (int i = 0; i < childCount; i++)
        {
            // Get the child at the current index
            Transform child = RoomSelector.transform.GetChild(i);

            // Do something with the child
            Toggle toggle = child.GetComponent<Toggle>();
            if(toggle == null) {
                return;
            }

            if(!selectedObject.name.Equals(child.name)) {
                toggle.isOn = false;
            }
            else {
                //get the text from the label
                CurrentlySelected = toggle.transform.GetChild(1).GetComponent<Text>().text;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0)) {
            if (EventSystem.current.IsPointerOverGameObject()) {
                // Get a reference to the selected UI game object
                
                GameObject selectedObject = EventSystem.current.currentSelectedGameObject;
                
                if(selectedObject == null || !selectedObject.transform.parent.gameObject.name.Equals("Room Selector")) {
                    return;
                }
                Select(selectedObject);
            }
        }     
    }
}
