using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OutlinedButton : MonoBehaviour,IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public Image ButtonBackground;
    void Start() {
        ButtonBackground.color = new Color(80/255f,80/255f,80/255f, 0f);
    }

    private IEnumerator DarkenButton(float alpha) {
        while (true) {
            ButtonBackground.color = Color.Lerp(ButtonBackground.color, new Color(80/255f,80/255f,80/255f, alpha/255f), Time.deltaTime * 15f);
            if (Mathf.Abs(ButtonBackground.color.a - (alpha/255f)) < 0.01f) {
                ButtonBackground.color = new Color(80/255f,80/255f,80/255f, alpha/255f);
                break;
            }
            yield return null;
        }
    }

    private void OnEnable()
    {
        ButtonBackground.color = new Color(80/255f,80/255f,80/255f, 0/255f);
    }

    private IEnumerator DarkenButtonInstant(float alpha) {
        ButtonBackground.color = new Color(120/255f,120/255f,120/255f, alpha/255f);
        yield return new WaitForSeconds(0.15f);
        ButtonBackground.color = new Color(80/255f,80/255f,80/255f, 100/255f);
    }

    public void OnPointerClick(PointerEventData eventData) {
        StopAllCoroutines();
        StartCoroutine(DarkenButtonInstant(150f));
    }

    public void OnPointerEnter(PointerEventData eventData) {
        StopAllCoroutines();
        StartCoroutine(DarkenButton(100f));
    }
    public void OnPointerExit(PointerEventData eventData){
        StopAllCoroutines();
        StartCoroutine(DarkenButton(0f));
    }

}
