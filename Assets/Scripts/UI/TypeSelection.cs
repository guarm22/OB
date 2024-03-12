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
    void Start() {
        Instance = this;
        CurrentlySelected = new List<string>();
    }

    public void Select(GameObject selectedObject) {
        if(selectedObject.GetComponent<Toggle>().isOn) {
            CurrentlySelected.Add(selectedObject.name);
        }
        else {
            CurrentlySelected.Remove(selectedObject.name);
        }
    }

    public void ResetToggles(){
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
            toggle.isOn = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
    
    }
}
