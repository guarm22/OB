using UnityEngine;
using UnityEngine.UI;

public class FillBar : MonoBehaviour {
    public GameObject bg;
    public GameObject fg;
    private float width;
    private float currentPercent;

    RectTransform fgRect;

    void Awake() {
        width = bg.GetComponent<Image>().rectTransform.rect.width;
        fgRect = fg.GetComponent<Image>().rectTransform;
        currentPercent = 1.0f;
    }

    public void SetPercent(float percent) {
        currentPercent = percent;
        fgRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width * currentPercent);
    }
}
