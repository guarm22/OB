using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UIElements;

public class MetalGateOpen : MonoBehaviour {
    
    public GameObject gateLeft;
    public GameObject gateRight;

    public AudioSource gateSound;

    public float openTime = 6.5f;

    public GameObject Player;

    public bool opened = false;
    private bool inAnim = false;

    public Vector3 leftOpenRotation = new Vector3(0, -90, 0);
    public Vector3 rightOpenRotation = new Vector3(0, 90, 0);

    public Vector3 leftCloseRotation = new Vector3(0, 0, 0);
    public Vector3 rightCloseRotation = new Vector3(0, 0, 0);

    public BoxCollider openCollider;
    public List<BoxCollider> closeColliders;

    public AudioClip lockedSound;
    public bool locked = false;

    void Start() {
        Player = GameObject.Find("Player");
        closeColliders.ForEach(collider => collider.enabled = false);
    }

    private IEnumerator animationTimer() {
        inAnim = true;
        yield return new WaitForSeconds(openTime);
        opened = !opened;
        inAnim = false;
    }

    public void OpenGate() {
        if(inAnim || opened) {
            return;
        }
        StartCoroutine(animationTimer());
        gateSound.Play();
        Debug.Log("Opening Gate");
        closeColliders.ForEach(collider => collider.enabled = true);
        openCollider.enabled = false;
        gateRight.transform.DOLocalRotate(rightOpenRotation, openTime);
        gateLeft.transform.DOLocalRotate(leftOpenRotation, openTime);
    }

    void OnMouseOver() {
        if(Vector3.Distance(this.transform.position, Player.transform.position) < 4f) {
            if(Input.GetKeyDown(KeyCode.Mouse0)|| Input.GetKeyDown(KeybindManager.instance.GetKeybind("Interact"))) {
                if(locked) {
                    gateSound.PlayOneShot(lockedSound);
                    return;
                }

                if(inAnim) {
                    return;
                }
                StartCoroutine(animationTimer());
                gateSound.Play();
                if(!opened) {
                    Debug.Log("Opening Gate");
                    closeColliders.ForEach(collider => collider.enabled = true);
                    openCollider.enabled = false;
                    gateRight.transform.DOLocalRotate(rightOpenRotation, openTime);
                    gateLeft.transform.DOLocalRotate(leftOpenRotation, openTime);
                }
                else {
                    Debug.Log("closing gate");
                    closeColliders.ForEach(collider => collider.enabled = false);
                    openCollider.enabled = true;
                    gateRight.transform.DOLocalRotate(rightCloseRotation, openTime);
                    gateLeft.transform.DOLocalRotate(leftCloseRotation, openTime); 
                }
            }
        }
    }
}
