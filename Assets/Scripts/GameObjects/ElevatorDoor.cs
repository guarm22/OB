using System.Collections;
using DG.Tweening;
using UnityEngine;

public class ElevatorDoor : MonoBehaviour{

    public GameObject leftDoor;
    public GameObject rightDoor;

    private bool open = false;
    public AudioClip arriveBell;

    private AudioSource audioSource;
    private float doorOpenTime = 2.5f;


    void Start() {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = arriveBell;
    }

    private IEnumerator OpenDoors() {
        audioSource.Play();
        yield return new WaitForSeconds(1);
        leftDoor.transform.DOMove(new Vector3(leftDoor.transform.position.x-1, leftDoor.transform.position.y, leftDoor.transform.position.z), doorOpenTime);
        rightDoor.transform.DOMove(new Vector3(rightDoor.transform.position.x+1, rightDoor.transform.position.y, rightDoor.transform.position.z), doorOpenTime);
        yield return new WaitForSeconds(1);
    }

    private IEnumerator CloseDoors() {
        leftDoor.transform.DOMove(new Vector3(leftDoor.transform.position.x+1, leftDoor.transform.position.y, leftDoor.transform.position.z), doorOpenTime);
        rightDoor.transform.DOMove(new Vector3(rightDoor.transform.position.x-1, rightDoor.transform.position.y, rightDoor.transform.position.z), doorOpenTime);
        yield return new WaitForSeconds(1);
    }

    public void TriggerDoor() {
        if(open){
            open = false;
            StartCoroutine(CloseDoors());
            return;
        }
        else {
            open = true;
            StartCoroutine(OpenDoors());
        }
    }

    void Update() {
        if(Input.GetKeyDown(KeyCode.O)) {
            if(open){
                open = false;
                StartCoroutine(CloseDoors());
                return;
            }
            else {
                open = true;
                StartCoroutine(OpenDoors());
            }
        }
    }
}
