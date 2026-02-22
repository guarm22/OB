using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NavKeypad;
using SojaExiles;
using UnityEngine;

public class ApartmentSecret : MonoBehaviour{
    public static ApartmentSecret Instance;
    public GameObject doorKey;

    public GameObject frontDoor;

    public GameObject ElevatorKeypad;

    public GameObject ElevatorDoor;

    public AudioClip elevatorMovingSound;
    private bool doorOpened = false;

    public GameObject hallwayMugs;
    public GameObject kitchenMugs;
    public GameObject livingRoomMugs;
    public GameObject storageMugs;
    public GameObject bathroomMugs;
    public GameObject bedroomMugs;

    public bool finished = false;

    public AudioClip keyPickupSound;

    void Start() {
        string dig1 = RemoveRandomAmount(bathroomMugs) +"";
        string dig2 = RemoveRandomAmount(bedroomMugs)+"";
        string dig3 = RemoveRandomAmount(hallwayMugs)+"";
        string dig4 = RemoveRandomAmount(kitchenMugs)+"";
        string dig5 = RemoveRandomAmount(livingRoomMugs)+"";
        string dig6 = RemoveRandomAmount(storageMugs)+"";
        Instance = this;

        string code = dig1+dig2+dig3+dig4+dig5+dig6+"";
        ElevatorKeypad.GetComponent<Keypad>().SetCombo(code);
    }

    private int RemoveRandomAmount(GameObject parent) {
        List<MeshRenderer> children = parent.GetComponentsInChildren<MeshRenderer>().ToList();
        int removeElement = Random.Range(0, children.Count);
        for(int i = 0; i < children.Count; i++) {
            if(i >= removeElement) { children[i].gameObject.SetActive(false); }
        }
        return removeElement;
    }

    private IEnumerator OnElevatorKeypad() {
        ElevatorDoor.GetComponent<AudioSource>().PlayOneShot(elevatorMovingSound);
        yield return new WaitForSeconds(elevatorMovingSound.length);
        ElevatorDoor.GetComponent<ElevatorDoor>().TriggerDoor();
        finished = true;
    }

    void Update() {
        if(Input.GetKeyDown(KeybindManager.instance.GetKeybind("Interact")) || Input.GetKeyDown(KeyCode.Mouse0)) {
            if(doorKey.activeInHierarchy && doorKey.GetComponent<Outliner>().hovering) {
                CrosshairControl.Instance.SetObjectInRange(false);
                PlayerUI.Instance.StartCoroutine(PlayerUI.Instance.Acquisition(doorKey.name, "You hear a clicking sound from the front door."));
                AudioSource.PlayClipAtPoint(keyPickupSound, doorKey.transform.position);
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
