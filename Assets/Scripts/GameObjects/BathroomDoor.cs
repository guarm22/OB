using UnityEngine;
using DG.Tweening;
using System.Collections;

public class BathroomDoor : MonoBehaviour {

    [SerializeField]
    private AudioClip openSound;
    [SerializeField]
    private AudioClip closeSound;

    [SerializeField]
    private Vector3 axis = Vector3.zero;

    [SerializeField]
    private float openAngle = 90f;

    private Vector3 closeRotation = Vector3.zero;

    private bool isOpen = false;

    private GameObject player;

    private bool inAnim = false;

    void Start() {
        player = GameObject.Find("Player");
        closeRotation = transform.eulerAngles;
    }

    void OpenDoor() {
        if(inAnim) {
            return;
        }
        if (openSound != null) {
            AudioSource.PlayClipAtPoint(openSound, transform.position);
        }
        //target angle is current rotation  where only the selected axis is changed to the specified open angle
        Vector3 targetRotation = transform.eulerAngles + new Vector3(axis.x * openAngle, axis.y * openAngle, axis.z * openAngle);
        StartCoroutine(ChangeDoorState(targetRotation, 1f, openSound, true));
    }

    void CloseDoor() {
        if(inAnim) {
            return;
        }
        StartCoroutine(ChangeDoorState(closeRotation, 1f, closeSound, false));
    }

    private IEnumerator ChangeDoorState(Vector3 targetRotation, float duration, AudioClip sound, bool doorOpen) {
        isOpen = doorOpen;
        if (sound != null) {
            AudioSource.PlayClipAtPoint(sound, transform.position);
        }
        transform.DORotate(targetRotation, duration);
        inAnim = true;
        yield return new WaitForSeconds(duration+0.15f); // Wait for the animation to finish plus a small buffer
        inAnim = false;
    }

    void Update() {
        if(Vector3.Distance(this.transform.position, player.transform.position) < 4f) {
            if(Input.GetKeyDown(KeyCode.Mouse0)|| Input.GetKeyDown(KeybindManager.instance.GetKeybind("Interact"))) {

                //shoot a raycast from the camera to the mouse position to check if the player is looking at the door
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if(Physics.Raycast(ray, out hit, 5.5f)) {
                    if(hit.transform != this.transform) {
                        return;
                    }
                } else {
                    return;
                }

                if(!isOpen) {
                    OpenDoor();
                } else {
                    CloseDoor();
                }
            }
        }
    }
}
