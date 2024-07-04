using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Dropdown : MonoBehaviour
{
    
    public List<String> options;

    [HideInInspector]
    public GameObject currentOption;

    public Button shownOption;

    public GameObject List;


    public void SetOption(GameObject option) {
        currentOption = option;
        shownOption.GetComponentInChildren<TMP_Text>().text = option.GetComponentInChildren<TMP_Text>().text;
        //close dropdown
        List.SetActive(false);
    }

    public void SetOption(String option) {
        //wait for the list to be created
        foreach (Transform child in List.transform) {
            //make sure child has tmp_text component
            if(child.GetComponentInChildren<TMP_Text>() == null) continue;

            if (child.GetComponentInChildren<TMP_Text>().text == option) {
                SetOption(child.gameObject);
                break;
            }
        }
    }

    public String GetCurrentOption() {
        return currentOption.GetComponentInChildren<TMP_Text>().text;
    }

    void Awake() {
        //create a dropdown menu with the list of options
        foreach (String o in options) {
            GameObject option = new GameObject(o);
            option.transform.SetParent(List.transform);
            option.transform.position = new Vector3
            (List.transform.position.x, List.transform.position.y - 90 * options.IndexOf(o), List.transform.position.z);
            option.transform.localScale = new Vector3(1, 1, 1);
            option.AddComponent<TMPro.TextMeshProUGUI>().text = o;
            option.GetComponent<TMPro.TextMeshProUGUI>().fontSize = 28;
            option.GetComponent<TMPro.TextMeshProUGUI>().alignment = TMPro.TextAlignmentOptions.Center;
            
            //change height
            RectTransform rt = option.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(rt.sizeDelta.x, 40);

            option.AddComponent<UnityEngine.UI.Button>();
            option.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(delegate { SetOption(option); });

            //make button hover color
            ColorBlock cb = option.GetComponent<UnityEngine.UI.Button>().colors;
            cb.highlightedColor = new Color(165/255f, 186/255f, 210/255f, 1f);
            option.GetComponent<UnityEngine.UI.Button>().colors = cb;

            //make button wider
            rt.sizeDelta = new Vector2(rt.sizeDelta.x*2f, rt.sizeDelta.y);

            //place a separator between each option
            GameObject separator = new GameObject("Separator");
            separator.transform.SetParent(List.transform);
            separator.transform.position = new Vector3
            (List.transform.position.x, List.transform.position.y - 90 * options.IndexOf(o) - 45, List.transform.position.z);
            separator.transform.localScale = new Vector3(1, 1, 1);
            separator.AddComponent<Image>();
            separator.GetComponent<Image>().color = new Color(165/255f, 186/255f, 210/255f, 1f);
            //extend the separator to fit the list
            RectTransform rt3 = separator.GetComponent<RectTransform>();
            rt3.sizeDelta = new Vector2(rt3.sizeDelta.x*1.95f, 2);
            
        }

        //create a blank image background for the list
        GameObject listBackground = new GameObject("ListBackground");
        listBackground.transform.SetParent(List.transform);
        listBackground.transform.position = new Vector3(List.transform.position.x, List.transform.position.y, List.transform.position.z);
        listBackground.transform.localScale = new Vector3(1, 1, 1);
        listBackground.AddComponent<Image>();
        listBackground.GetComponent<Image>().color = new Color(72/255f, 86/255f, 102/255f, 1f);
        
        //extend the background to fit the list
        RectTransform rt2 = listBackground.GetComponent<RectTransform>();
        rt2.sizeDelta = new Vector2(rt2.sizeDelta.x*2, 45 * options.Count);

        //move the background to be in the middle of the list
        listBackground.transform.position = new Vector3(listBackground.transform.position.x, listBackground.transform.position.y - 45 * options.Count / 2 - 35, listBackground.transform.position.z);

        //put it behind the text
        listBackground.transform.SetSiblingIndex(0);

        //add listener to the shown option to toggle the list
        shownOption.onClick.AddListener(delegate { ToggleList(); });
        //set initial shown option
        shownOption.GetComponentInChildren<TMP_Text>().text = options[0];
    }

    private void ToggleList() {
        List.SetActive(!List.activeSelf);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
