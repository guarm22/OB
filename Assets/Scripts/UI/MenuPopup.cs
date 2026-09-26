using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuPopup : MonoBehaviour {

    [SerializeField]
    private Button closeButton;


    [SerializeField]
    private TMP_Text title;

    [SerializeField]
    private TMP_Text description;
    [SerializeField]
    private Toggle toggle;

    private string popupName = null;

    void Start() {
        closeButton.onClick.AddListener(ClosePopup);
    }

    public void SetInfo(string title, string description, string popupName) {
        this.title.text = title;
        this.description.text = description;
        this.popupName = popupName;
    }

    private void ClosePopup(){
        MenuPopupManager.Instance.ClosePopup(new MenuPopupData(title.text, description.text, this.popupName, toggle.isOn), toggle.isOn);
    }


}
