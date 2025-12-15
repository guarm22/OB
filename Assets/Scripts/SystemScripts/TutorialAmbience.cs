using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialAmbience : MonoBehaviour {

    public float ambienceReplayDelay = 42f;
    public float ambienceReplayRandomOffset = 6.5f;
    private float ambienceTimer = 0f;

    private Vector3 originalPosition;

    public AudioSource audioPlayer;

    public List<AudioClip> ambienceClips;
    public AudioSource backgroundNoise;

    public GameObject Player;

    void Start() {
        Player = GameObject.Find("Player");
        originalPosition = audioPlayer.transform.localPosition;
        ambienceTimer = ambienceReplayDelay - 8f;
        audioPlayer.volume = PlayerPrefs.GetInt("AmbienceVolume") / 100f;
        backgroundNoise.volume = PlayerPrefs.GetInt("AmbienceVolume") / 1000f;
    }

    private void ChangeAudioDirection() {
        GameObject a = audioPlayer.gameObject;
        a.transform.localPosition = new Vector3(
            Random.Range(-25f, 20f),
            Random.Range(1f, 4f),
            Random.Range(-20f, 25f)
        );
        a.gameObject.transform.parent = null;
    }

    private IEnumerator PlayRandomSound() {
        audioPlayer.Stop();
        AudioClip c = ambienceClips[Random.Range(0, ambienceClips.Count)];
        audioPlayer.PlayOneShot(c);
        ambienceTimer = 0f - c.length + Random.Range(-ambienceReplayRandomOffset, ambienceReplayRandomOffset);
        yield return new WaitForSeconds(c.length);

        audioPlayer.gameObject.transform.parent = Player.transform;
        audioPlayer.gameObject.transform.localPosition = originalPosition;
    }

    // Update is called once per frame
    void Update() {
        if(PlayerUI.paused) return;

        if(ambienceTimer >= ambienceReplayDelay) {
            StopAllCoroutines();
            ChangeAudioDirection();
            StartCoroutine(PlayRandomSound());
        } else {
            ambienceTimer += Time.deltaTime;
        }
    }
}
