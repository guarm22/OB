using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SubtitleControl : MonoBehaviour
{
    private TMP_Text subtitleText;
    public static SubtitleControl Instance;

    private float defaultSubtitleDuration = 8.0f;

    void Start() {
        Instance = this;
        subtitleText = GameObject.Find("SubtitleText").GetComponent<TMP_Text>();
        subtitleText.text = "";
    }
    public void ShowSubtitle(string text, float duration = -1f) {
        subtitleText.text = text;
        StopAllCoroutines();

        if(duration > 0f) {
            StartCoroutine(ClearSubtitleAfterTime(duration));
            return;
        }
        StartCoroutine(ClearSubtitleAfterTime(defaultSubtitleDuration));

    }

    private IEnumerator ClearSubtitleAfterTime(float time) {
        yield return new WaitForSeconds(time);
        subtitleText.text = "";
    }

}
