using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class CampsiteSecret : MonoBehaviour {

    private List<string> roomNames = new List<string> { "Cave", "Marsh", "Firepit", "Outhouse", "Rock Path", "Tents" };

    private List<GameObject> pressedButtons = new List<GameObject>();

    [SerializeField]
    private List<GameObject> buttons = new List<GameObject>();
    [SerializeField]
    private GameObject startButton;

    private bool sequenceStarted = false;

    private float sequenceTimer = 0f;
    private float sequenceTimeLimit = 70f;


    void Start() {
        RandomizeRoomOrder();
    }

    private void RandomizeRoomOrder() {
        //scramble list of rooms
        for (int i = 0; i < roomNames.Count; i++) {
            string temp = roomNames[i];
            int randomIndex = Random.Range(i, roomNames.Count);
            roomNames[i] = roomNames[randomIndex];
            roomNames[randomIndex] = temp;
        }
        //cave not first
        if(roomNames[0] == "Cave") {
            int newIndex = Random.Range(1, roomNames.Count);
            string temp = roomNames[0];
            roomNames[0] = roomNames[newIndex];
            roomNames[newIndex] = temp;
        }        
    }

    private void StartSequence() {
        sequenceStarted = true;
        StartCoroutine(ButtonSequence());
        RandomizeRoomOrder();
        sequenceTimer = 0f;
        Debug.Log("Sequence Started! Room Order: " + string.Join(", ", roomNames));
        SubtitleControl.Instance.ShowSubtitle(string.Join(", ", roomNames), "WIP: ", 20f);

        foreach (GameObject button in buttons) {
            button.SetActive(true);
        }
    }

    private void EndSequence(bool success = false) {
        Debug.Log("Sequence Ended! Success: " + success);
        sequenceStarted = false;
        foreach (GameObject button in buttons) {
            button.SetActive(false);
            button.GetComponent<CampsiteButton>().SetPressed(false);
        }
        pressedButtons.Clear();
    }

    private void CompleteSequence() {
        //open secret
        EndSequence(success: true);
    }

    private IEnumerator ButtonSequence(GameObject button = null) {
        if (button == null) {
            button = startButton;
        }
        
        float originalY = button.transform.localPosition.y;
        Sequence pressAnimation = DOTween.Sequence();
        pressAnimation.Append(button.transform.DOLocalMoveY(originalY - 0.05f, 0.1f).SetEase(Ease.OutQuad));
        pressAnimation.Append(button.transform.DOLocalMoveY(originalY, 0.1f).SetEase(Ease.InQuad));

        if (button != startButton) {
            pressedButtons.Add(button);
        }
        else { yield break; }
        button.GetComponent<CampsiteButton>().SetPressed(true);

        //if the pressed buttons match the room names in order, then end the sequence and open the secret
        if (pressedButtons.Count == roomNames.Count) {
            bool correct = true;
            for (int i = 0; i < pressedButtons.Count; i++) {
                if (pressedButtons[i].name != roomNames[i]+"Button") {
                    correct = false;
                    break;
                }
            }
            if (correct) {
                CompleteSequence();
            } else {
                EndSequence();
            }
        }

        
    }

    void Update() {
        if(PlayerUI.paused) {
            return;
        }

        if(sequenceStarted) {
            sequenceTimer += Time.deltaTime;
            if(sequenceTimer >= sequenceTimeLimit) {
                EndSequence();
            }
        }

        if(Input.GetKeyDown(KeybindManager.instance.GetKeybind("Interact")) || Input.GetKeyDown(KeyCode.Mouse0)) {
            if (startButton.GetComponent<Outliner>().hovering) {
                StartSequence();
            }

            foreach (GameObject button in buttons) {
                if (button.GetComponent<Outliner>().hovering) {
                    StartCoroutine(ButtonSequence(button));
                }
            }
        }
    }
}
