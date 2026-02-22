using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Flashlight : MonoBehaviour {

    public Light beam;
    private float staticEnergyDrainPerSecond;
    private float totalEnergyDrainPerSecond;
    public float energyDrainModifier = 1f;
    private float timer;

    public GameObject PhysicalFlashlight;

    public AudioClip flashOn;
    public AudioClip flashOff;

    private Vector3 defaultFlashlightPos = new Vector3(0.28f, -0.55f, 0.3f);

    private Vector3 offFlashlingPos = new Vector3(0.3f, -0.55f, -1f);

    private float flashlightMoveTime = 0.2f;

    public bool isOn;

    private float bobSpeed = 2f;
    private float bobAmount = 0.01f;
    private float bobWeight = 0f;

    private float swayAmount = 1f;
    private float swaySmooth = 2f;

    private Vector3 initialLocalPos;
    private Quaternion initialLocalRot;

    private float bobTimer = 0f;

    public bool everTurnedOn = false;


    public float innerAngle = 30f;
    public float outerAngle = 70f;
    public static Flashlight Instance;
    // Start is called before the first frame update
    void Start() {
        beam.enabled = false;
        isOn = false;   
        float eps = PlayerPrefs.GetFloat("EPS");
        staticEnergyDrainPerSecond = eps * .95f;
        totalEnergyDrainPerSecond = staticEnergyDrainPerSecond * energyDrainModifier;
        Instance = this;

        initialLocalPos = defaultFlashlightPos;
        initialLocalRot = PhysicalFlashlight.transform.localRotation;

        beam.innerSpotAngle = innerAngle;
        beam.spotAngle = outerAngle;

        PhysicalFlashlight.transform.localPosition = offFlashlingPos;

        if(PlayerPrefs.GetInt("Darkness",0) == 1 && SceneManager.GetActiveScene().name != "Tutorial") {
            totalEnergyDrainPerSecond = 0.00001f;
        }
    }


    // Update is called once per frame
    void Update() {
        if(GameSystem.Instance.GameOver || PlayerUI.paused || PlayerUI.Instance.inMenu) {
            if(Popup.Instance != null) {
                if(Popup.Instance.isPopupOpen) {
                    return;
                }
            }
            return;
        }

        if(isOn) {
            ApplyBobbing();
            ApplySway();
            timer += Time.deltaTime;
            if(timer >= 1f) {
                timer = 0;
                GameSystem.Instance.ChangeEnergy(-totalEnergyDrainPerSecond);
            }
        }


        if(GameSystem.Instance.CurrentEnergy <= 1 && isOn) {
            TurnOffLight();
            return;
        }

        if(Input.GetKeyDown(KeybindManager.instance.GetKeybind("Flashlight"))) {
            if(!isOn) { TurnOnLight(); }
            else { TurnOffLight(); }
        }
    }

    private void TurnOnLight() {
        AudioSource.PlayClipAtPoint(flashOn, this.gameObject.transform.position);
        StartCoroutine(MoveOverTime(PhysicalFlashlight.transform, offFlashlingPos, defaultFlashlightPos, flashlightMoveTime));
        beam.enabled = true;
        isOn = true;
        everTurnedOn = true;
    }

    public void TurnOffLight() {
        AudioSource.PlayClipAtPoint(flashOn, this.gameObject.transform.position);
        StartCoroutine(MoveOverTime(PhysicalFlashlight.transform, defaultFlashlightPos, offFlashlingPos, flashlightMoveTime));
        beam.enabled = false;
        isOn = false;
    }

    public IEnumerator MoveOverTime(Transform obj, Vector3 startPos, Vector3 endPos, float duration) {
        float elapsed = 0f;
        while (elapsed < duration) {
            float t = elapsed / duration;
            obj.localPosition = Vector3.Lerp(startPos, endPos, t);
            elapsed += Time.deltaTime;
            yield return null;
        }
        obj.localPosition = endPos; // snap to exact final position
    }

    void ApplyBobbing() {
        bool isMoving = Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0;

        // Smoothly increase/decrease bob weight
        float targetWeight = isMoving ? 1f : 0f;
        bobWeight = Mathf.Lerp(bobWeight, targetWeight, Time.deltaTime * 6f);

        if (isMoving || bobWeight > 0.01f)
        {
            bobTimer += Time.deltaTime * bobSpeed;

            float bobX = Mathf.Sin(bobTimer) * bobAmount * bobWeight;
            float bobY = Mathf.Cos(bobTimer) * bobAmount * bobWeight;

            PhysicalFlashlight.transform.localPosition = initialLocalPos + new Vector3(bobX, bobY, 0);
        }
    }

    void ApplySway() {
        float mouseX = Input.GetAxis("Mouse X") * swayAmount;
        float mouseY = Input.GetAxis("Mouse Y") * swayAmount;

        Quaternion swayRotation = Quaternion.Euler(-mouseY, mouseX, 0);
        PhysicalFlashlight.transform.localRotation = Quaternion.Slerp(PhysicalFlashlight.transform.localRotation, initialLocalRot * swayRotation, Time.deltaTime * swaySmooth);
    }

}
