using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSounds : MonoBehaviour {
    public AudioClip pendingSound;
    public AudioSource audioSource;
    public static PlayerSounds Instance;

    void Start() {
        Instance = this;
        audioSource = this.gameObject.GetComponent<AudioSource>();
    }

    public IEnumerator PendingReport() {
        audioSource.clip = pendingSound;
        audioSource.Play();
        yield return new WaitForSeconds(DivergenceControl.Instance.ReportLockout);
        audioSource.Stop();
    }
}
