using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class GraveyardSecret : MonoBehaviour {
    public static GraveyardSecret Instance;
    public GameObject gate;

    public AudioSource audioSource;

    public List<GameObject> keys = new List<GameObject>();
    public List<GameObject> padlocks = new List<GameObject>();

    public bool finished = false;

    void Start()
    {
        Instance = this;
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
        gate.GetComponentInChildren<MetalGateOpen>().OpenGate();
        padlocks.ForEach(p => p.SetActive(false));
        finished = true;
    }

   
    void Update() {
        if(Input.GetKeyDown(KeyCode.H)) {
            UnlockGate();
        }

        if(Input.GetKeyDown(KeybindManager.instance.GetKeybind("Interact"))|| Input.GetKeyDown(KeyCode.Mouse0)) {
            foreach(GameObject key in keys) {
                if(key.activeInHierarchy && key.GetComponent<Outliner>().hovering) {
                    PlayerUI.Instance.StartCoroutine(PlayerUI.Instance.Acquisition(key.name, "A padlock has been unlocked."));
                    key.SetActive(false);
                    UnlockPadlock();
                    break;
                }
            }

            if(keys.Where(k => k.activeInHierarchy).ToList().Count == 0) {
                UnlockGate();
            }
        }
    }
}
