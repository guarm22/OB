using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using NavKeypad;

public class GraveyardSecret : MonoBehaviour {
    public static GraveyardSecret Instance;
    public GameObject gate;

    public AudioSource audioSource;

    public List<GameObject> keys = new List<GameObject>();
    public List<GameObject> padlocks = new List<GameObject>();

    public AudioClip keyPickupSound;

    public bool finished = false;

    public List<GameObject> lightGroups = new List<GameObject>();

    private string code;
    private bool blinking = false;
    List<int> numbers;

    public GameObject upperGate;
    public GameObject gateKeypad;
    private bool upperGateOpened = false;

    public AudioClip warningClip;

    public GameObject button;

    void Start() {
        
        int count = 0;
        while(count < 5) {
            code += UnityEngine.Random.Range(1, 9).ToString();
            count +=1;
        }

        Debug.Log(code);
        Instance = this;
        numbers = Enumerable.Range(0, 5).ToList();
        System.Random rng = new System.Random();
        numbers = numbers.OrderBy(_ => rng.Next()).ToList();
        gateKeypad.GetComponent<Keypad>().SetCombo(code);

    }

    private void UnlockPadlock() {
        //get random unlocked padlock
        List<GameObject> lockedPadlocks = padlocks.Where(p => p.GetComponent<PadlockScript>().locked).ToList();
        if(lockedPadlocks.Count == 0) {
            return;
        }
        int index = UnityEngine.Random.Range(0, lockedPadlocks.Count);
        lockedPadlocks[index].GetComponent<PadlockScript>().Unlock();
    }

    private void UnlockGate() {
        gate.GetComponentInChildren<MetalGateOpen>().OpenGate();
        padlocks.ForEach(p => p.SetActive(false));
    }

    private void UnlockUpperGate()
    {
        upperGate.GetComponentInChildren<MetalGateOpen>().OpenGate();
    }

    private IEnumerator BlinkLightGroups() {
        if(blinking == true) {yield break;}
        button.transform.DOLocalMoveY(8.707f, .2f);
        yield return new WaitForSeconds(.2f);
        button.transform.DOLocalMoveY(8.8f, .2f);
        int count = 0;
        blinking = true;
        while(count<lightGroups.Count) {
            if(PlayerUI.paused) {yield return new WaitForSeconds(.5f); continue;}
            AudioSource.PlayClipAtPoint(warningClip, lightGroups[numbers[count]].transform.GetChild(0).position);
            yield return new WaitForSeconds(1.25f);
            StartCoroutine(Blink(lightGroups[numbers[count]].transform.GetChild(0).gameObject, code[count] - '0'));
            yield return new WaitForSeconds(WaitTime(.35f, code[count] - '0') + 1.5f);
            count += 1;
        }
        blinking = false;
    }

    private float WaitTime(float duration, int times)
    {
        return (duration + (duration*1.5f) + duration + (duration*1.5f)) * times;
    }

    private IEnumerator Blink(GameObject sphere, int times, float duration = .35f) {
        int count = 0;
        while(count < times) {
            if(PlayerUI.paused) {yield return null; continue;}
            sphere.transform.DOScale(new Vector3(1,1,1), duration);
            yield return new WaitForSeconds(duration*1.5f);
            sphere.transform.DOScale(Vector3.zero, duration);
            count += 1;
            yield return new WaitForSeconds(duration*1.5f);
        }
    }

   
    void Update() {
        if(Input.GetKeyDown(KeyCode.H) && GameSystem.InEditor()) {
            UnlockGate();
        }

        if(Input.GetKeyDown(KeybindManager.instance.GetKeybind("Interact"))|| Input.GetKeyDown(KeyCode.Mouse0)) {
            foreach(GameObject key in keys) {
                if(key.activeInHierarchy && key.GetComponent<Outliner>().hovering) {
                    CrosshairControl.Instance.SetObjectInRange(false);
                    PlayerUI.Instance.StartCoroutine(PlayerUI.Instance.Acquisition(key.name, "A padlock has been unlocked."));
                    AudioSource.PlayClipAtPoint(keyPickupSound, key.transform.position);
                    key.SetActive(false);
                    UnlockPadlock();
                    break;
                }
            }

            if(keys.Where(k => k.activeInHierarchy).ToList().Count == 0) {
                UnlockGate();
            }

            if(button.GetComponent<Outliner>().hovering) {
                StartCoroutine(BlinkLightGroups());
            }
        }
        if(gateKeypad.GetComponent<NavKeypad.Keypad>().accessWasGranted && !upperGateOpened) {
            upperGateOpened = true;
            finished = true;
            UnlockUpperGate();
        }
    }
}
