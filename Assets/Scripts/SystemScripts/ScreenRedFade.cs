using UnityEngine;
using UnityEngine.UI;

public class ScreenRedFade : MonoBehaviour
{
    [SerializeField]
    private Image overlay;

    [SerializeField]
    private float duration = 30f;

    [SerializeField]
    private Color redColor = new Color(0.45f, 0f, 0f, 1f);

    [SerializeField]
    [Range(0f, 1f)]
    private float redAmount = 0.8f;

    [SerializeField]
    [Range(0f, 1f)]
    private float blackAmount = 0.9f;

    [SerializeField]
    [Range(0f, 1f)]
    private float vignette = 0.5f;

    private Material material;
    private float timer;
    private bool playing;

    private float startProgress;
    private float targetProgress;
    private float transitionTime;
    private float transitionTimer;
    private bool transitioning;

    private void Awake()
    {
        if (overlay == null)
            overlay = GetComponent<Image>();

        // Create an instance so we don't modify the
        // original material asset.
        material = Instantiate(overlay.material);
        overlay.material = material;

        material.SetColor("_Color", redColor);
        material.SetFloat("_RedAmount", redAmount);
        material.SetFloat("_BlackAmount", blackAmount);
        material.SetFloat("_Vignette", vignette);
        material.SetFloat("_Progress", 0f);

        StartFade();
    }

    public void SetProgress(float progress, float time) {
        if(time == -1) {
            time = duration;
        }
        Debug.Log("Setting progress to: " + progress + " over " + time + " seconds");

        targetProgress = Mathf.Clamp01(progress);
        startProgress = material.GetFloat("_Progress");

        transitionTime = Mathf.Max(0.001f, time);
        transitionTimer = 0f;

        transitioning = true;
    }

    private void Update() {
    if (!transitioning)
        return;

    transitionTimer += Time.deltaTime;

    float t = Mathf.Clamp01(transitionTimer / transitionTime);

    // Linear interpolation
    float progress = Mathf.Lerp(
        startProgress,
        targetProgress,
        t
    );

    material.SetFloat("_Progress", progress);

    if (t >= 1f)
    {
        material.SetFloat("_Progress", targetProgress);
        transitioning = false;
    }
}

    public void StartFade()
    {
        timer = 0f;
        playing = true;

        material.SetFloat("_Progress", 0f);
    }

    public void StopFade()
    {
        playing = false;
    }

    public void ResetFade()
    {
        timer = 0f;
        playing = false;

        material.SetFloat("_Progress", 0f);
    }
}