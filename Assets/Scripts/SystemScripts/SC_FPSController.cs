using System.Collections;
using UnityEngine;

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

    private bool mouseAccel;
    private Vector3 prevMousePosition;
    private float accelerationFactor = 0.01f;

    public int timesCrouched = 0;

    void Start() {
        characterController = GetComponent<CharacterController>();
        Instance = this;
        // Lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        originalRunSpeed = runningSpeed;
        originalWalkSpeed = walkingSpeed;
        originalCrouchSpeed = crouchSpeed;
        FOV = PlayerPrefs.GetInt("FOV", 60);
        lookSpeed = PlayerPrefs.GetFloat("MouseSens", 2);
        originalFOV = FOV;
        playerCamera.fieldOfView = FOV;
        mouseAccel = PlayerPrefs.GetInt("MouseAccel", 0) == 1 ? true : false;
    }

    private void PlayerMove() {
        // We are grounded, so recalculate move direction based on axes
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);
        // Press Left Shift to run
        bool isRunning = Input.GetKey(KeybindManager.instance.GetKeybind("Sprint"));
        float curSpeedX = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Vertical") : 0;
        float curSpeedY = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Horizontal") : 0;
        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);
        //So speed is not doubled when moving in two directions
        moveDirection = Vector3.ClampMagnitude(moveDirection, isRunning ? runningSpeed : walkingSpeed);

        CrouchLogic();
        CameraZoom();

        if(Input.GetKeyDown(KeybindManager.instance.GetKeybind("Interact"))) {
            Interact();
        }

        if (Input.GetKey(KeybindManager.instance.GetKeybind("Jump")) && canMove && characterController.isGrounded) {
            moveDirection.y = jumpSpeed;
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
        if (canMove) {
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

    private void CrouchLogic() {
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
        else {
            isCrouchAnimation = true;
            runningSpeed = crouchSpeed;
            walkingSpeed = crouchSpeed;
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

    private void Interact() {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit)) {
            if(hit.collider.gameObject.CompareTag("Collectible")) {
                CollectibleControl.Instance.Collect(hit.collider.gameObject);
            }
        }
    }

    void Update()  { 
        CheckOutOfMap();  
        if(PlayerUI.paused || GameSystem.Instance.GameOver || CreatureControl.Instance.IsJumpscareFinished || PlayerUI.Instance.inMenu) {
            return;
        }
        PlayerMove();
    }
}
