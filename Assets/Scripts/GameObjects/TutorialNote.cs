using UnityEngine;
using System.Collections;

public class TutorialNote : MonoBehaviour {

    private BoxCollider col;
    private int stacks;
    private bool playerOnNote = false;

    public AudioClip noteSound;

    void Start() {
        col = GetComponent<BoxCollider>();
    }

    void OnTriggerEnter(Collider other) {
        if(other.gameObject.name == "Player") {
            playerOnNote = true;
            Debug.Log("Standing on " + gameObject.name);
            StartCoroutine(Stack());
        }
    }

    void OnTriggerExit(Collider other) {
        if(other.gameObject.name == "Player") {
            playerOnNote = false;
            Debug.Log("Stopped standing on " + gameObject.name + " with " + stacks + " stacks.");
            StopAllCoroutines();
        }
    }

    private IEnumerator Stack() {
        float timer = 0f;
        while(true) {
            if(PlayerUI.paused) {
                yield return null;
                continue;
            }

            timer += Time.deltaTime;
            if(timer < 2f) {
                yield return null;
                continue;
            }
            AudioSource.PlayClipAtPoint(noteSound, transform.position);
            stacks++;
            timer = 0f;
        }
    }

    public int getStacks() {
        return stacks;
    }
    public bool isPlayerOnNote() {
        return playerOnNote;
    }

    public void resetStacks() {
        stacks = 0;
    }
}
