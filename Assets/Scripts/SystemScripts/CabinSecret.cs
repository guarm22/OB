using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using SojaExiles;
using UnityEngine;
using UnityEngine.AI;

public class CabinSecret : MonoBehaviour {
    
    public GameObject cellarDoor;
    private opencloseDoor doorScript;

    public List<GameObject> keys = new List<GameObject>();
    public List<GameObject> padlocks = new List<GameObject>();

    public GameObject button;

    public List<Light> lights = new List<Light>();
    public GameObject wall;
    private int currentLight=0;

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
        float gameTime = GameSystem.Instance.gameTime;
        int minutes = Mathf.FloorToInt(gameTime / 60);
        int seconds = Mathf.FloorToInt(gameTime % 60);

        //first light
        if(minutes == 10 && seconds == 30 && currentLight == 0) {
            lights[currentLight].color = Color.red;
            currentLight += 1;
        }

        if(minutes == 9 && seconds == 09 && currentLight == 1) {
            lights[currentLight].color = Color.red;
            currentLight += 1;
        }

        if(minutes == 5 && seconds == 59 && currentLight == 2) {
            lights[currentLight].color = Color.red;
            currentLight += 1;
        }

        if(minutes == 2 && seconds == 51 && currentLight == 3) {
            lights[currentLight].color = Color.red;
            currentLight += 1;
        }



        if(Input.GetKeyDown(KeybindManager.instance.GetKeybind("Interact"))) {
            foreach(GameObject key in keys) {
                if(key.activeInHierarchy && key.GetComponent<Outliner>().hovering) {
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

                if(minutes == 9 && seconds == 09 && currentLight == 1) {
                    lights[currentLight].color = Color.green;
                    currentLight += 1;
                    return;
                }

                if(minutes == 5 && seconds == 59 && currentLight == 2) {
                    lights[currentLight].color = Color.green;
                    currentLight += 1;
                    return;
                }

                if(minutes == 2 && seconds == 51 && currentLight == 3) {
                    lights[currentLight].color = Color.green;
                    currentLight += 1;
                    return;
                }

                if(currentLight == 4) {
                    bool allGreen = true;
                    foreach(Light l in lights) {
                        if(l.color == Color.red){
                            allGreen = false;
                        }
                    }
                    if (allGreen) {
                        wall.SetActive(false);
                        currentLight += 5;
                    }
                    return;
                }
                if(currentLight >= 4)
                {
                    return;
                }

                lights[currentLight].color = Color.red;
                currentLight += 1;
            }

            if(keys.Where(k => k.activeInHierarchy).ToList().Count == 0) {
                Debug.Log("Unlocking door");
                doorScript.ChangeLockState(false);
            }
        }
    }
}
