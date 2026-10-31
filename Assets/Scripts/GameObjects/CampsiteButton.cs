using UnityEngine;

public class CampsiteButton : MonoBehaviour {

    private AudioSource audioSource;

    private float repeatTimer = 0f;

    [SerializeField]
    private AudioClip buttonSound;
    [SerializeField]
    private float repeatFrequency = 3f;

    private float randomDelay = 0.15f;

    private bool pressed = false;

    void Start() {
        audioSource = this.gameObject.GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.spatialBlend = 1f;
        audioSource.volume = PlayerPrefs.GetInt("SFXVolume") / 100f;

        this.gameObject.SetActive(false);
    }

    public void SetPressed(bool value) {
        pressed = value;
    }

    // Update is called once per frame
    void Update() {
        if(PlayerUI.paused) {
            return;
        }

        if(pressed) {
            return;
        }

        if(repeatTimer <= repeatFrequency) {
            repeatTimer += Time.deltaTime;
        }
        else {
            repeatTimer = Random.Range(-randomDelay, randomDelay);
            audioSource.PlayOneShot(buttonSound);
        }
    }
}
