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

    [SerializeField]
    private float timeToOpen = 1f;
    [SerializeField]
    private float timeToClose = 1f;

    [SerializeField]
    private float volModifier = 1f;

    private float interactRange = 5f;

    [SerializeField]
    private float pitch = 1f;

    private AudioSource audioSource;

    void Start() {
        player = GameObject.Find("Player");
        closeRotation = transform.eulerAngles;
        audioSource = this.gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 1f;
        audioSource.volume = PlayerPrefs.GetInt("SFXVolume") / 100f * volModifier;
        audioSource.pitch = pitch;
    }   

    void OpenDoor() {
        if(inAnim) {
            return;
        }
        //target angle is current rotation  where only the selected axis is changed to the specified open angle
        Vector3 targetRotation = transform.eulerAngles + new Vector3(axis.x * openAngle, axis.y * openAngle, axis.z * openAngle);
        StartCoroutine(ChangeDoorState(targetRotation, timeToOpen, openSound, true));
    }

    void CloseDoor() {
        if(inAnim) {
            return;
        }
        StartCoroutine(ChangeDoorState(closeRotation, timeToClose, closeSound, false));
    }

    private IEnumerator ChangeDoorState(Vector3 targetRotation, float duration, AudioClip sound, bool doorOpen) {
        isOpen = doorOpen;
        if (sound != null) {
            audioSource.PlayOneShot(sound);
        }
        transform.DORotate(targetRotation, duration);
        inAnim = true;
        yield return new WaitForSeconds(duration+0.15f); // Wait for the animation to finish plus a small buffer
        inAnim = false;
    }

    void Update() {
        if(Vector3.Distance(this.transform.position, player.transform.position) < interactRange) {
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
