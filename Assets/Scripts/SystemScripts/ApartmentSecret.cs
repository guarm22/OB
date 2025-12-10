using System.Collections;
using SojaExiles;
using UnityEngine;

public class ApartmentSecret : MonoBehaviour{
    public GameObject doorKey;

    public GameObject frontDoor;

    public GameObject ElevatorKeypad;

    public GameObject ElevatorDoor;

    public AudioClip elevatorMovingSound;
    private bool doorOpened = false;

    private IEnumerator OnElevatorKeypad() {
        ElevatorDoor.GetComponent<AudioSource>().PlayOneShot(elevatorMovingSound);
        yield return new WaitForSeconds(elevatorMovingSound.length);
        ElevatorDoor.GetComponent<ElevatorDoor>().TriggerDoor();
    }

    void Update() {
        if(Input.GetKeyDown(KeybindManager.instance.GetKeybind("Interact"))) {
            if(doorKey.activeInHierarchy && doorKey.GetComponent<Outliner>().hovering) {
                doorKey.SetActive(false);
                frontDoor.GetComponentInChildren<opencloseDoor>().locked = false;
            }
        }
        if(ElevatorKeypad.GetComponent<NavKeypad.Keypad>().accessWasGranted && !doorOpened) {
            doorOpened = true;
            StartCoroutine(OnElevatorKeypad());
        }
    }
}
