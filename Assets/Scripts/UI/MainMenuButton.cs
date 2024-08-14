using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class MainMenuButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public float defaultAlpha = 0.9f;

    public void OnPointerEnter(PointerEventData eventData)
    {
        //Debug.Log("Pointer entered");
        //change highlighted color of button
        this.GetComponent<Image>().color = new Color(155, 155, 155, defaultAlpha);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //Debug.Log("Pointer exited");
        this.GetComponent<Image>().color = new Color(155, 155, 155, 0);
    }

    void Start() {
        this.GetComponent<Button>().onClick.AddListener(OnClick);

        //set weight of text in button to 500
    }

    private void OnClick() {
        //remove highlight when clicked
        //this is because the buttons become unactive when clicked, so the onpointerexit event is not called
        this.GetComponent<Image>().color = new Color(155, 155, 155, 0);
    }
}