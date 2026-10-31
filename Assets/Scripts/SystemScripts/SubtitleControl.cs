using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SubtitleControl : MonoBehaviour
{
    private const float SubtitleGap = 15f;
    private const float BackgroundPadding = 25f;

    private TMP_Text subtitleText;
    public static SubtitleControl Instance;
    private TMP_Text titleText;
    private Image bg;

    private float defaultSubtitleDuration = 5.0f;

    private bool displayingSubtitle = false;

    private float subtitleTimer = 0f;
    private float subtitleTimeShown = 5f;


    void Start() {
        Instance = this;
        subtitleText = GameObject.Find("SubtitleText").GetComponent<TMP_Text>();
        subtitleText.text = "";

        titleText = GameObject.Find("SubtitleTitle").GetComponent<TMP_Text>();
        titleText.text = ":";

        bg = GameObject.Find("SubtitleBG").GetComponent<Image>();
        bg.gameObject.SetActive(false);

        //ShowSubtitle("testestesgtuudghuiofdhuiofdghuio", "STALKER:", 3f);
    }

    public void ShowSubtitle(string text, string title, float duration = -1f) {
        subtitleText.text = text;
        titleText.text = title;
        bg.gameObject.SetActive(true);

        //stops timer from preemptively killing subtitle if one was alive before
        StopAllCoroutines();

        if(duration > 0f) {
            StartCoroutine(ClearSubtitleAfterTime(duration));
            return;
        }
        StartCoroutine(ClearSubtitleAfterTime(defaultSubtitleDuration));

    }


    void Update() {
        if(PlayerUI.paused) {
            return;
        }
    }

    private IEnumerator ClearSubtitleAfterTime(float time) {
        float timeElapsed = 0f;

        while(true) {
            if(PlayerUI.paused) {
                yield return null;
                continue;
            }
            timeElapsed += Time.deltaTime;
            if(timeElapsed > time) {
                break;
            }
            yield return null;
        }
        
        titleText.text = "";
        subtitleText.text = "";
        bg.gameObject.SetActive(false);
    }


    private void LateUpdate() {
        subtitleText.ForceMeshUpdate();
        titleText.ForceMeshUpdate();

        Bounds subtitleBounds = subtitleText.textBounds;
        Bounds titleBounds = titleText.textBounds;
        Transform titleParent = titleText.rectTransform.parent;
        Vector3 subtitleLeft = titleParent.InverseTransformPoint(
            subtitleText.transform.TransformPoint(
                new Vector3(subtitleBounds.min.x, subtitleBounds.center.y, subtitleBounds.center.z)));
        Vector3 titleRight = titleParent.InverseTransformPoint(
            titleText.transform.TransformPoint(
                new Vector3(titleBounds.max.x, titleBounds.center.y, titleBounds.center.z)));

        Vector2 titlePosition = titleText.rectTransform.anchoredPosition;
        titlePosition.x += subtitleLeft.x - SubtitleGap - titleRight.x;
        titleText.rectTransform.anchoredPosition = titlePosition;

        RectTransform bgRect = bg.rectTransform;
        Transform bgParent = bgRect.parent;
        Bounds contentBounds = new Bounds();
        bool hasBounds = false;
        EncapsulateTextBounds(subtitleText, bgParent, ref contentBounds, ref hasBounds);
        EncapsulateTextBounds(titleText, bgParent, ref contentBounds, ref hasBounds);

        Vector3 bgPosition = bgRect.localPosition;
        bgPosition.x = contentBounds.center.x;
        bgPosition.y = contentBounds.center.y;
        bgRect.localPosition = bgPosition;
        bgRect.sizeDelta = new Vector2(
            contentBounds.size.x + BackgroundPadding * 2f,
            contentBounds.size.y + BackgroundPadding * 2f);
    }

    private static void EncapsulateTextBounds(TMP_Text text, Transform targetSpace, ref Bounds combinedBounds, ref bool hasBounds) {
        Bounds textBounds = text.textBounds;
        for (int x = 0; x < 2; x++) {
            for (int y = 0; y < 2; y++) {
                for (int z = 0; z < 2; z++) {
                    Vector3 corner = new Vector3(
                        x == 0 ? textBounds.min.x : textBounds.max.x,
                        y == 0 ? textBounds.min.y : textBounds.max.y,
                        z == 0 ? textBounds.min.z : textBounds.max.z);
                    Vector3 point = targetSpace.InverseTransformPoint(text.transform.TransformPoint(corner));
                    if (hasBounds) {
                        combinedBounds.Encapsulate(point);
                    } else {
                        combinedBounds = new Bounds(point, Vector3.zero);
                        hasBounds = true;
                    }
                }
            }
        }
    }


}
