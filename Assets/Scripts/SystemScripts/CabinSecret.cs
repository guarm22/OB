using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using SojaExiles;
using UnityEngine;

public class CabinSecret : MonoBehaviour {
    
    public GameObject cellarDoor;
    private opencloseDoor doorScript;

    public List<GameObject> keys = new List<GameObject>();
    public List<GameObject> padlocks = new List<GameObject>();

    void Start() {
        doorScript = cellarDoor.GetComponent<opencloseDoor>();
        
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

   
    void Update() {
        if(Input.GetKeyDown(KeyCode.H)) {
            doorScript.ChangeLockState(!doorScript.locked);
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
                doorScript.ChangeLockState(false);
            }
        }
    }
}
