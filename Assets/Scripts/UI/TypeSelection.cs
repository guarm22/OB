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
        Toggle toggle = selectedObject.GetComponent<Toggle>();
        Image toggleBackground = toggle.targetGraphic as Image; // Get the Image component
        if(toggle.isOn) {
            //toggle.colors.normalColor = new Color(0.5f, 0.5f, 0.5f, 1);
            ChangeColorOfAllImages(selectedObject, Color.gray); // Change the color of all images
            CurrentlySelected.Add(selectedObject.name);
        }
        else {
            toggleBackground.color = Color.white; // Change the background color back
            toggle.transform.GetChild(0).GetComponent<Image>().color = Color.black; // Change the color of the checkmark
            CurrentlySelected.Remove(selectedObject.name);
        }
    }

    
    private void ChangeColorOfAllImages(GameObject gameObject, Color color) {
        Image[] images = gameObject.GetComponentsInChildren<Image>();
        foreach (Image image in images) {
            image.color = color;
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
            Image toggleBackground = toggle.targetGraphic as Image; // Get the Image component
            toggleBackground.color = Color.white; // Change the background color back
            toggle.transform.GetChild(0).GetComponent<Image>().color = Color.black; // Change the color of the checkmark
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
