using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CharacterController))]

public class SC_FPSController : MonoBehaviour
{
    public float walkingSpeed = 7.5f;
    [HideInInspector]
    public float originalWalkSpeed;
    [HideInInspector]
    public float originalRunSpeed;
    public float runningSpeed = 11.5f;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;
    public Camera playerCamera;
    public float lookSpeed = 2.0f;
    public float lookXLimit = 90.0f;
    CharacterController characterController;
    Vector3 moveDirection = Vector3.zero;
    float rotationX = 0;
    public static SC_FPSController Instance;

    [HideInInspector]
    public bool isCrouching = false;
    public float crouchAnimationTime = 0.15f;
    public float crouchSpeed = 2.5f;
    [HideInInspector]
    public float originalCrouchSpeed;
    private bool isCrouchAnimation = false;
    [HideInInspector]

    public bool canMove = true;

    [HideInInspector]
    public float FOV;
    [HideInInspector]
    public float originalFOV;
    public float minFOV = 30;

    public GameObject blacknessPanel;

    private bool mouseAccel;
    private Vector3 prevMousePosition;
    private float accelerationFactor = 0.01f;
    private bool teleported = false;
    public int timesCrouched = 0;
    public bool isRunning = false;
    private List<GameObject> Rooms = new List<GameObject>();

    public bool controlsGlitch = false;

    void Start() {
        characterController = GetComponent<CharacterController>();
        characterController.stepOffset = 0.4f; // Increase step height here
        Instance = this;
        // Lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        originalRunSpeed = runningSpeed;
        originalWalkSpeed = walkingSpeed;
        originalCrouchSpeed = crouchSpeed;
        FOV = PlayerPrefs.GetInt("FOV", 85);
        if(FOV < 80) {
            FOV = 85;
            PlayerPrefs.SetInt("FOV", 85);
        }
        lookSpeed = PlayerPrefs.GetFloat("MouseSens", 2);
        originalFOV = FOV;
        playerCamera.fieldOfView = FOV;
        mouseAccel = PlayerPrefs.GetInt("MouseAccel", 0) == 1 ? true : false;


        if(PlayerPrefs.GetInt("Speed Boost", 0) == 1 && SceneManager.GetActiveScene().name != "Tutorial") {
            originalRunSpeed *= 1.5f;
            originalCrouchSpeed *= 1.6f;
            originalWalkSpeed *= 1.3f;
        }
        Rooms = GameObject.FindGameObjectsWithTag("Room").ToList();

        runningSpeed = originalRunSpeed;
        walkingSpeed = originalWalkSpeed;
        crouchSpeed = originalCrouchSpeed;

        if(PlayerPrefs.GetInt("Teleport", 0) == 1 && SceneManager.GetActiveScene().name != "Tutorial") {
            StartCoroutine(RandomTeleporting());
        }
    }

    private IEnumerator RandomTeleporting() {
        float tptimer = 0f;
        float maxWait = 32f;
        float minWait = 24f;

        float currentWait = UnityEngine.Random.Range(minWait, maxWait);
        while(true) {
            if(tptimer > currentWait) {
                teleported = true;
                TeleportRoom(Rooms[UnityEngine.Random.Range(0, Rooms.Count)]);
                tptimer = 0f;
            }
            else {
                tptimer += Time.deltaTime;
                yield return null;
            }

            if(GameSystem.Instance.GameOver == true) {
                break;
            }
        }
        yield return null;
    }

    private void PlayerMove() {
        if(teleported) {
            teleported = false;
            characterController.enabled = true;
            return;
        }

        // We are grounded, so recalculate move direction based on axes
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);
        // Press Left Shift to run
        isRunning = Input.GetKey(KeybindManager.instance.GetKeybind("Sprint"));
        float curSpeedX = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Vertical") : 0;
        float curSpeedY = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Horizontal") : 0;
        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);
        //So speed is not doubled when moving in two directions
        moveDirection = Vector3.ClampMagnitude(moveDirection, isRunning ? runningSpeed : walkingSpeed);

        CrouchLogic();
        CameraZoom();

        if (Input.GetKey(KeybindManager.instance.GetKeybind("Jump")) && canMove && characterController.isGrounded) {
            //moveDirection.y = jumpSpeed;
        }
        else {
            moveDirection.y = movementDirectionY;
        }
        // Apply gravity. Gravity is multiplied by deltaTime twice (once here, and once below
        // when the moveDirection is multiplied by deltaTime). This is because gravity should be applied
        // as an acceleration (ms^-2)
        if (!characterController.isGrounded) {
            moveDirection.y -= gravity * Time.deltaTime;
        }
        // Move the controller
        characterController.Move(moveDirection * Time.deltaTime);
        // Player and Camera rotation
        if (canMove && !PlayerUI.Instance.reportDeviceUp) {
            float actualLookSpeed = lookSpeed;

            if(mouseAccel) {
                Vector3 mouseDelta = Input.mousePosition - prevMousePosition;
                float mouseSpeed = mouseDelta.magnitude / Time.deltaTime;
                actualLookSpeed = lookSpeed * (1 + mouseSpeed * accelerationFactor);
            }

            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * actualLookSpeed, 0);
        }
        prevMousePosition = Input.mousePosition;
    }

    public void MoveCamera(Vector3 newRot) {
        rotationX = newRot.x;
        playerCamera.transform.DORotate(newRot, 0.2f);
        transform.DORotate(new Vector3(0, newRot.y, 0), 0.2f);
    }

    public IEnumerator ForceCrouch(float duration) {
        //crouch then uncrouch 2 seconds later
        yield return StartCoroutine(Crouch(true));
        yield return new WaitForSeconds(duration);
        yield return StartCoroutine(Crouch(false));
    }

    private void CrouchLogic() {
        if(controlsGlitch) {
            return;
        }

        if(Input.GetKeyDown(KeybindManager.instance.GetKeybind("Crouch")) && !isCrouchAnimation) {
            timesCrouched++;
            StartCoroutine(Crouch(true));
        }
        else if (Input.GetKeyUp(KeybindManager.instance.GetKeybind("Crouch")) && !isCrouchAnimation && isCrouching) {
            StartCoroutine(Crouch(false));
        }
        if(isCrouchAnimation && isCrouching) {
            return;
        }
        if(!Input.GetKey(KeybindManager.instance.GetKeybind("Crouch")) && isCrouching) {
            StartCoroutine(Crouch(false));
        }

    }

    private IEnumerator Crouch(bool enable) {
        float duration = 0f;
        //uncrouch
        if(!enable) {
            isCrouchAnimation = true;
            runningSpeed = originalRunSpeed;
            walkingSpeed = originalWalkSpeed;
            while(duration < crouchAnimationTime) {
                duration += Time.deltaTime;
                characterController.height = Mathf.Lerp(1, 2, duration/crouchAnimationTime);
                characterController.center = new Vector3(0, Mathf.Lerp(-0.5f, 0, duration/crouchAnimationTime), 0);
                playerCamera.transform.localPosition = new Vector3(0, Mathf.Lerp(-0.5f, 0.639f, duration/crouchAnimationTime));
                yield return null;
            }
            isCrouchAnimation = false;
            isCrouching = false;
        }
        //crouch
        else {
            isCrouchAnimation = true;
            runningSpeed = originalCrouchSpeed;
            walkingSpeed = originalCrouchSpeed;
            while(duration < crouchAnimationTime) {
                duration += Time.deltaTime;
                characterController.height = Mathf.Lerp(2, 1, duration/crouchAnimationTime);
                characterController.center = new Vector3(0, Mathf.Lerp(0, -0.5f, duration/crouchAnimationTime), 0);
                playerCamera.transform.localPosition = new Vector3(0, Mathf.Lerp(0.639f, -0.5f, duration/crouchAnimationTime));
                yield return null;
            }
            isCrouchAnimation = false;
            isCrouching = true;
        }
    }

    public void Debuff(string type, float multiplier, float duration=0) {
        if(type == "Slow") {
            StartCoroutine(PlayerDebuffs.Instance.Slow(multiplier, duration));
        }
        else if(type == "Energy") {
            PlayerDebuffs.Instance.Energy(multiplier);
        }
        else {
            Debug.LogError("Invalid debuff type: " + type);
        }
    }

    public void TeleportRoom(GameObject room) {
        StartCoroutine(TeleportEffects(room));
    }

    private IEnumerator TeleportEffects(GameObject room) {
        blacknessPanel.SetActive(true);
        blacknessPanel.GetComponent<Image>().color = new Color(0, 0, 0, 0);
        //slowly turn up alpha of panel over 0.5 seconds
        float timer = 0f;
        while(timer < 0.5f) {
            timer += Time.deltaTime;
            blacknessPanel.GetComponent<Image>().color += new Color(0, 0, 0, Time.deltaTime / 0.5f);

            //warp fov
            playerCamera.fieldOfView = Mathf.Lerp(FOV, 35, timer / 0.5f);

            yield return null;
        }
        //return fov
        playerCamera.fieldOfView = FOV;
        teleported = true;
        Vector3 tpLoc = GameObject.FindGameObjectsWithTag("Teleport").Where(x => x.name.Contains(room.name)).ElementAt(0).transform.position;
        if(tpLoc == null) {
            tpLoc = new Vector3(room.transform.position.x, room.transform.position.y + 1, room.transform.position.z);
        }
        transform.position = tpLoc;
        characterController.enabled = false;
        
        yield return new WaitForSeconds(1f);
        timer = 0f;
        //slowly turn down alpha of panel
        while(timer < 4f) {
            timer += Time.deltaTime;
            blacknessPanel.GetComponent<Image>().color -= new Color(0, 0, 0, Time.deltaTime / 4f);
            yield return null;
        }
        blacknessPanel.GetComponent<Image>().color = new Color(0, 0, 0, 0);
        blacknessPanel.SetActive(false);

    }

    public void ChangeFOV(float fov) {
        FOV = fov;
        playerCamera.fieldOfView = FOV;
    }

    public void CameraZoom() {
        if(Input.GetKey(KeybindManager.instance.GetKeybind("Zoom"))) {
            SlowlyZoom(30);
        }
        else {
            SlowlyZoom(originalFOV);
        }
    }

    private void SlowlyZoom(float targetFOV) {
        if(playerCamera.fieldOfView < minFOV) {
            return;
        }
        playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, targetFOV, 0.25f);
    }

    private void CheckOutOfMap() {
        if(transform.position.y < -90) {
            if(GameObject.Find("SPAWNPOINT")) {
                transform.position = GameObject.Find("SPAWNPOINT").transform.position;
            }
            else {
                transform.position = new Vector3(0, 0, 0);
            }
        }
    }

    public void LockMovement(bool lockMove) {
        canMove = !lockMove;
    }

    void Update()  { 
        CheckOutOfMap();  
        if(PlayerUI.paused || GameSystem.Instance.GameOver || CreatureControl.Instance.IsJumpscareFinished) {
            return;
        }
        
        //if in dev mode and press g, teleport to random room
        if(GameSystem.InEditor() && Input.GetKeyDown(KeyCode.G)) {
            TeleportRoom(Rooms[UnityEngine.Random.Range(0, Rooms.Count)]);
        }

        PlayerMove();
    }
}
