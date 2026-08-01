using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TutorialSecret : MonoBehaviour {

    public List<GameObject> rocks;

    public GameObject fenceGate;
    public GameObject normalGate;
    private string sequence = "";

    public List<TutorialNote> notes = new List<TutorialNote>();
    private List<TutorialNote> noteSequence = new List<TutorialNote>();

    private int currentNoteIndex = 0;

    private bool doNotContinue = false;

    public AudioClip failedSequenceSound;
    public AudioClip correctSequenceSound;

    public TMP_Text paperNote;

    private bool secretActive = true;

    public static TutorialSecret Instance;
    public bool finished = false;

    void Start() {
        Instance = this;
        GenerateSequence();
        PrintNoteSequence();
    }

    void Update() {
        if(Input.GetKeyDown(KeybindManager.instance.GetKeybind("Interact")) || Input.GetKeyDown(KeyCode.Mouse0)) {
            CollectRock();
        }
        if(secretActive) {
            CheckSequence();
        }
    }

    private void CheckSequence() {
        //if sequence finished:
        if(currentNoteIndex >= noteSequence.Count) {
            return;
        }

        //check if the player is currently standing on the incorrect note
        foreach(TutorialNote note in noteSequence) {
            if(note.isPlayerOnNote() && note != noteSequence[currentNoteIndex] && !doNotContinue) {
                Debug.Log("Standing on wrong note: " + note.gameObject.name + ".");
                StartCoroutine(WrongNoteReset(note));
                currentNoteIndex = 0;
                return;
            }
        }

        //if current note in sequence has the required stacks, move to the next note, otherwise reset the sequence
        //sequence[currentNoteIndex * 2] is the number of stacks required for the current note
        //also, if player is still standing on the note, don't move to the next note until they step off
        if(noteSequence[currentNoteIndex].getStacks() == int.Parse(sequence[currentNoteIndex * 2].ToString()) && noteSequence[currentNoteIndex].isPlayerOnNote() == false) {
            currentNoteIndex++;
            Debug.Log("Correct note! Moving to next note. Current index: " + currentNoteIndex);
            if(currentNoteIndex >= noteSequence.Count) {
                fenceGate.GetComponent<WoodFenceOpen>().locked = false;
                PlayerUI.Instance.StartCoroutine(PlayerUI.Instance.Acquisition("A gate clicks in the distance.", "", ""));
                AudioSource.PlayClipAtPoint(correctSequenceSound, fenceGate.transform.position);
                finished = true;
            }
        }

        else if(noteSequence[currentNoteIndex].getStacks() > int.Parse(sequence[currentNoteIndex * 2].ToString()) && noteSequence[currentNoteIndex].isPlayerOnNote() == false) {
            Debug.Log("Too many stacks on " + noteSequence[currentNoteIndex].gameObject.name + ".");
            
            StartCoroutine(WrongNoteReset(noteSequence[currentNoteIndex]));
            currentNoteIndex = 0;
        }
        
    }

    private IEnumerator WrongNoteReset(TutorialNote note) {
        doNotContinue = true;
        Debug.Log("Preparing to reset sequence...");
        yield return new WaitUntil(() => note.isPlayerOnNote() == false);
        currentNoteIndex = 0;
        Debug.Log("Reset sequence.");
        foreach(TutorialNote n in noteSequence) {
            n.resetStacks();
        }
        yield return new WaitForSeconds(0.5f);
        AudioSource.PlayClipAtPoint(failedSequenceSound, note.transform.position);
        doNotContinue = false;
    }

    private void GenerateSequence() {
        //create a random sequence of letters from the possible letters without duplicates
        string possibleLetters = "abcde";
        string preSeq = "";
        string letters = possibleLetters;
        for(int i = 0; i < possibleLetters.Length; i++) {
            int index = Random.Range(0, letters.Length);
            preSeq += letters[index];

            TutorialNote note = GetNote(letters[index]);
            noteSequence.Add(note);
            
            letters = letters.Remove(index, 1);
        }
        string nums = "";
        int seqLength = preSeq.Length;
        for(int i = 0; i < seqLength; i++) {
            nums += Random.Range(2, 6).ToString();
        }

        for(int i=0; i<seqLength; i++) {
            sequence += nums[i].ToString() + preSeq[i].ToString();
        }
        paperNote.text = sequence + "\n\nThe letters beneath your feet.";
        Debug.Log("Sequence: " + sequence);
    }

    private TutorialNote GetNote(char letter) {
        foreach(TutorialNote note in notes) {
            if(char.ToLower(note.gameObject.name[4]) == char.ToLower(letter)) {
                return note;
            }
        }
        return null;
    }

    private void PrintNoteSequence() {
        string seq = "";
        foreach(TutorialNote note in noteSequence) {
            seq += note.gameObject.name + " ";
        }
        Debug.Log("Note Sequence: " + seq);
    }


    private void CollectRock(){
        foreach(GameObject rock in rocks) {
            if(rock.activeInHierarchy && rock.GetComponent<Outliner>().hovering) {
                CrosshairControl.Instance.SetObjectInRange(false);
                PlayerUI.Instance.StartCoroutine(PlayerUI.Instance.Acquisition(rock.name, "It's a rock."));
                rock.SetActive(false);
                break;
            }
        }
    }
}
