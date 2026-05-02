using System.Collections.Generic;
using System.Linq;
using SojaExiles;
using UnityEngine;
using DG.Tweening;

public class CabinSecret : MonoBehaviour {
    public static CabinSecret Instance;
    
    public GameObject cellarDoor;
    private opencloseDoor doorScript;

    public List<GameObject> keys = new List<GameObject>();
    public List<GameObject> padlocks = new List<GameObject>();

    public GameObject button;

    public List<Light> lights = new List<Light>();
    public GameObject wall;
    private int currentLight=0;
    public AudioClip keyPickupSound;

    public bool finished = false;

    void Start() {
        doorScript = cellarDoor.GetComponent<opencloseDoor>();
        Instance = this;

        foreach (Light l in lights) {
            l.gameObject.SetActive(false);
        }
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
        if(Input.GetKeyDown(KeyCode.H) && GameSystem.InEditor()) {
            doorScript.ChangeLockState(!doorScript.locked);
        }
        if(Input.GetKeyDown(KeyCode.V) && GameSystem.InEditor()) {
            wall.SetActive(false);
            finished = true;
        }
        float gameTime = GameSystem.Instance.gameTime;
        int minutes = Mathf.FloorToInt(gameTime / 60);
        int seconds = Mathf.FloorToInt(gameTime % 60);

        if(Input.GetKeyDown(KeybindManager.instance.GetKeybind("Interact")) || Input.GetKeyDown(KeyCode.Mouse0)) {
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
            if(button.GetComponent<Outliner>().hovering) {
                if(currentLight >= lights.Count){return;}

                //first light
                if(minutes == 10 && seconds == 31 && currentLight == 0) {
                    lights[currentLight].color = Color.green;
                    currentLight += 1;
                    return;
                }

                if(minutes == 9 && seconds == 10 && currentLight == 1) {
                    lights[currentLight].color = Color.green;
                    currentLight += 1;
                    return;
                }

                if(minutes == 6 && seconds == 00 && currentLight == 2) {
                    lights[currentLight].color = Color.green;
                    currentLight += 1;
                    return;
                }

                if(minutes == 2 && seconds == 52 && currentLight == 3) {
                    lights[currentLight].color = Color.green;
                    currentLight += 1;
                }

                if(currentLight == 4) {
                    bool allGreen = true;
                    foreach(Light l in lights) {
                        if(l.color == Color.red){
                            allGreen = false;
                        }
                    }
                    if (allGreen) {
                        wall.transform.DOMoveY(-2, 2);
                        finished = true;
                        currentLight += 5;
                    }
                }
                if(currentLight >= 4) {
                    return;
                }

                lights[currentLight].color = Color.red;
                currentLight += 1;
            }

            if(keys.Where(k => k.activeInHierarchy).ToList().Count == 0) {
                Debug.Log("Unlocking door");
                doorScript.ChangeLockState(false);
                foreach (Light l in lights) {
                    l.gameObject.SetActive(true);
                }
            }
        }
    }
}
