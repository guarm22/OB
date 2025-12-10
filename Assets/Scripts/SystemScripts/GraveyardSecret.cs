using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class GraveyardSecret : MonoBehaviour {
    public GameObject lockedDoorLeft;
    public GameObject lockedDoorRight;

    public AudioSource audioSource;

    public List<GameObject> keys = new List<GameObject>();
    public List<GameObject> padlocks = new List<GameObject>();

    void Start() {
    }

    private void UnlockPadlock() {
        //get random unlocked padlock
        List<GameObject> lockedPadlocks = padlocks.Where(p => p.GetComponent<PadlockScript>().locked).ToList();
        if(lockedPadlocks.Count == 0) {
            return;
        }
        int index = Random.Range(0, lockedPadlocks.Count);
        lockedPadlocks[index].GetComponent<PadlockScript>().Unlock();
    }

    private void UnlockGate() {
        lockedDoorRight.transform.DORotate(new Vector3(-90, 0, -270), 6f);
        lockedDoorLeft.transform.DORotate(new Vector3(-90 , 0, 90), 6f);

        lockedDoorLeft.transform.DOMove(new Vector3(lockedDoorLeft.transform.position.x + 0.8f, 1, lockedDoorLeft.transform.position.z+1), 2f);
        lockedDoorRight.transform.DOMove(new Vector3(lockedDoorRight.transform.position.x - 0.8f, 1, lockedDoorRight.transform.position.z+1), 2f);

        foreach(GameObject padlock in padlocks) {
            padlock.SetActive(false);
        }

        audioSource.Play();
    }

   
    void Update() {
        if(Input.GetKeyDown(KeyCode.H)) {
            UnlockGate();
        }

        if(Input.GetKeyDown(KeybindManager.instance.GetKeybind("Interact"))) {
            foreach(GameObject key in keys) {
                if(key.activeInHierarchy && key.GetComponent<Outliner>().hovering) {
                    key.SetActive(false);
                    UnlockPadlock();
                    break;
                }
            }

            if(keys.Where(k => k.activeInHierarchy).ToList().Count == 0) {
                Debug.Log("Unlocking door");
                //add door unlocking logic here
            }
        }
    }
}
