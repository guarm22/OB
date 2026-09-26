using System.Collections.Generic;
using UnityEngine;

public class MenuPopupManager : MonoBehaviour {    

    public static MenuPopupManager Instance { get; private set; }

    [SerializeField]
    private GameObject PopupPrefab;

    private List<MenuPopupData> popupList = new List<MenuPopupData>();

    [SerializeField]
    private GameObject popupParent;

    private GameObject currentPopup = null;


    void Awake() {
        Instance = this;

        if(PFileUtil.Load<JsonWrapperUtil<MenuPopupData>>("menuPopups.json") == null) {
            Debug.Log("No menu popup file found, creating new one");
            popupList = new List<MenuPopupData>();
            Save();
        }
        else {
            popupList = PFileUtil.Load<JsonWrapperUtil<MenuPopupData>>("menuPopups.json").list;
        }
    }

    void Save() {
        PFileUtil.Save("menuPopups.json", new JsonWrapperUtil<MenuPopupData>(popupList));
    }

    public void OpenPopup(MenuPopupData popupData) {
        if(!popupList.Exists(p => p.name == popupData.name)) {
            //popup does not exist in the list, add it then save
            Debug.Log("Popup " + popupData.name + " does not exist in the list, adding it");
            popupList.Add(popupData);
            Save();
        }
        MenuPopupData existingPopup = popupList.Find(p => p.name == popupData.name);

        if(existingPopup != null && existingPopup.doNotShowAgain) {
            Debug.Log("Popup " + existingPopup.name + " is set to not show again, skipping");
            return;
        }

        GameObject popupInstance = Instantiate(PopupPrefab, popupParent.transform);
        MenuPopup popupScript = popupInstance.GetComponent<MenuPopup>();

        // Set the title and description of the popup
        popupScript.SetInfo(popupData.title, popupData.description, popupData.name);
        currentPopup = popupInstance;
    }

    public void ClosePopup(MenuPopupData popupData, bool doNotShowAgain) {
        // Find the popup in the list and set its doNotShowAgain to true
        MenuPopupData existingPopup = popupList.Find(p => p.name == popupData.name);
        //print list of all popupList items
        foreach(MenuPopupData p in popupList) {
            Debug.Log("Popup in list: " + p.name + ", doNotShowAgain: " + p.doNotShowAgain);
        }
        

        if(existingPopup != null) {
            Destroy(currentPopup);
            currentPopup = null;
            existingPopup.doNotShowAgain = doNotShowAgain;
            Save();
        }
    }

}
