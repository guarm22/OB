using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TypeSelection : MonoBehaviour
{
    public GameObject TypeSelector;

    public static TypeSelection Instance {get; private set;}

    public static List<string> CurrentlySelected {get; private set;}

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        CurrentlySelected = new List<string>();
    }

    void Select(GameObject selectedObject) {
        int childCount = TypeSelector.transform.childCount;

        // Iterate over all children
        for (int i = 0; i < childCount; i++) {
            // Get the child at the current index
            Transform child = TypeSelector.transform.GetChild(i);

            // Do something with the child
            Toggle toggle = child.GetComponent<Toggle>();
            if(toggle == null) {
                return;
            }

            if(!selectedObject.name.Equals(child.name)) {
                //toggle.isOn = false;
            }
            else {
                //get the text from the label
                CurrentlySelected.Add(toggle.transform.GetChild(1).GetComponent<Text>().text);
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
                
                if(selectedObject == null || !selectedObject.transform.parent.gameObject.name.Equals("Type Selector")) {
                    return;
                }
                Select(selectedObject);
            }
        }        
    }
}
