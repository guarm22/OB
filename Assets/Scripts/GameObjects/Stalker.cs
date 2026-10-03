using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class Stalker : CreatureBase {
    
    private float timeSpentInRoom = 0f;
    private float timeSpentOutsideRoom = 0f;

    //the sound that plays when the player is in the same room as the stalker
    private float whisperFrequency = 8;

    //the sound that plays when the player is NOT in the same room as the stalker
    private float laughFrequency = 12f;
    
    private bool playerEnteredRoom = false;
    private float angleDifference = 75f;

    private float flashDuration = 2f;
    private GameObject defaultUI;

    [SerializeField]
    private GameObject whiteFlashPrefab;

    [SerializeField]
    private AudioClip stalkerLaugh;
    [SerializeField]
    private AudioClip stalkerScream;
    [SerializeField]
    private AudioClip stalkerWhisper;

    private Quaternion playerRotation;

    private bool aboutToRemove = false;

    [SerializeField]
    private Volume volume;

    protected override void Awake() {
        a = GetComponent<AudioSource>();
        defaultUI = GameObject.Find("DefaultUI");
        a.spatialBlend = 1f;
        a.volume = PlayerPrefs.GetInt("CreatureVolume", 50)/100f;
        timeSpentInRoom = whisperFrequency - 0.33f; //so that the first whisper plays almost immediately
    }

    // Update is called once per frame
    protected override void Update() {
        //check if player is in the same room as the stalker
        if(PlayerUI.paused) { return; }
        if(aboutToRemove) { return; }

        if(PlayerUI.Instance.currentRoom == this.HomeRoom) {
            HideBehindPlayer();
            timeSpentInRoom += Time.deltaTime;
            timeSpentOutsideRoom = 0f;
            

            if(playerRotation.eulerAngles.y >= SC_FPSController.Instance.transform.rotation.eulerAngles.y + angleDifference || 
            playerRotation.eulerAngles.y <= SC_FPSController.Instance.transform.rotation.eulerAngles.y - angleDifference) {
                //the player has turned around, trigger the event
                StalkerKillEvent("camera");
            }
            if(timeSpentInRoom >= whisperFrequency) {
                StalkerTimerEvent();
            }
        }
        else if(playerEnteredRoom) {
            //the player has left the room after being stalked, trigger the event
            StalkerKillEvent(reason: "room");
        }
        else {
            //reset the timer so that the first laugh plays almost immediately when the player enters the room again
            timeSpentInRoom = whisperFrequency - 0.33f;
            OutsideRoomBehavior();
        }
    }

    private void StalkerKillEvent(string reason = "unknown") {
        //trigger the event that happens when the player turns around and sees the stalker
        //this should be a loud scream, a flash of white, and a large energy drain

        StartCoroutine(Sound(stalkerScream, 2.5f));
        if(reason == "camera") {
            Debug.Log("Stalker kill event triggered by camera in " + this.HomeRoom);
            //play "WHY DID YOU LOOK"
        }
        else if(reason == "room") {
            Debug.Log("Stalker kill event triggered by leaving room in " + this.HomeRoom);
            //play "DON'T LEAVE ME"
        }

        SC_FPSController.Instance.Debuff("Energy", 15f);
        StartCoroutine(FlashScreen(flashDuration));
        StartCoroutine(Disorient());
        StartCoroutine(AboutToRemove());
    }

    private IEnumerator Disorient() {
        yield return null;
        Instantiate(volume, transform.position, Quaternion.identity);
    }

    private IEnumerator AboutToRemove() {
        aboutToRemove = true;
        float elapsed = 0f;
        while(elapsed < flashDuration) {
            if(PlayerUI.paused) { yield return null; continue;}
            elapsed += Time.deltaTime;
            yield return null;
        }
        CreatureControl.Instance.RemoveCreature(gameObject);
    }

    private void StalkerTimerEvent() {
        //trigger the event that happens when the player is in the same room as the stalker for too long

        Debug.Log("Stalker timer event triggered in " + this.HomeRoom);
        timeSpentInRoom = 0f;
        SC_FPSController.Instance.Debuff("Energy", 5f);
        StartCoroutine(Sound(stalkerWhisper, 1f));
    }

    private void OutsideRoomBehavior() {
        timeSpentOutsideRoom += Time.deltaTime;
        if(timeSpentOutsideRoom >= laughFrequency) {
            StartCoroutine(Sound(stalkerLaugh, 0.5f));
            Debug.Log("Stalker laugh triggered in " + this.HomeRoom);
            timeSpentOutsideRoom = 0f;
        }
    }   

    private IEnumerator FlashScreen(float duration = 1f) {
        GameObject whiteFlash = Instantiate(whiteFlashPrefab, defaultUI.transform);
        float elapsed = 0f;
        while(elapsed < duration) {
            if(PlayerUI.paused) {
                yield return null;
                continue;
            }

            if(elapsed < flashDuration/3f) {
                whiteFlash.GetComponent<Image>().color = new Color(1f, 1f, 1f, Mathf.Lerp(0f, 1f, elapsed/(flashDuration/2f)));
            }
            else {
                whiteFlash.GetComponent<Image>().color = new Color(1f, 1f, 1f, Mathf.Lerp(1f, 0f, (elapsed-(flashDuration/2f))/(flashDuration/2f)));
            }

            elapsed += Time.deltaTime;
            whiteFlash.GetComponent<Image>().color = new Color(1f, 1f, 1f, Mathf.Lerp(1f, 0f, elapsed/duration));
            yield return null;
        }
        Destroy(whiteFlash);
    }

    private void HideBehindPlayer() {
        //move the stalker to a position directly behind the player
        Vector3 behindPlayer = SC_FPSController.Instance.transform.position - SC_FPSController.Instance.transform.forward * 2f;
        this.transform.position = behindPlayer;

        if(!playerEnteredRoom) {
            playerEnteredRoom = true;
            playerRotation = SC_FPSController.Instance.transform.rotation;
        }
    }

    private IEnumerator Sound(AudioClip clip, float volumeModifier = 1f) {
        float originalVolume = a.volume;
        
        a.pitch = Random.Range(0.8f, 1.2f);
        a.clip = clip;
        a.volume *= volumeModifier;
        a.Play();
        yield return new WaitForSeconds(clip.length);

        a.volume = originalVolume;
    }
}
