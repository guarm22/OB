using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]

public class SC_FPSController : MonoBehaviour
{
    public float walkingSpeed = 7.5f;
    public float originalWalkSpeed;
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
    public bool isCrouching = false;
    public float crouchAnimationTime = 0.15f;
    public float crouchSpeed = 2.5f;
    private bool isCrouchAnimation = false;
    
    public bool canMove = true;

    void Start() {
        characterController = GetComponent<CharacterController>();
        Instance = this;
        // Lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        originalRunSpeed = runningSpeed;
        originalWalkSpeed = walkingSpeed;
    }

    private void PlayerMove() {
        // We are grounded, so recalculate move direction based on axes
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);
        // Press Left Shift to run
        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        float curSpeedX = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Vertical") : 0;
        float curSpeedY = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Horizontal") : 0;
        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);
        //So speed is not doubled when moving in two directions
        moveDirection = Vector3.ClampMagnitude(moveDirection, isRunning ? runningSpeed : walkingSpeed);

        CrouchLogic();

        if (Input.GetButton("Jump") && canMove && characterController.isGrounded) {
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
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }
    }

    private void CrouchLogic() {
        if(Input.GetKeyDown(KeyCode.LeftControl) && !isCrouchAnimation) {
            StartCoroutine(Crouch(true));
        }
        else if (Input.GetKeyUp(KeyCode.LeftControl) && !isCrouchAnimation && isCrouching) {
            StartCoroutine(Crouch(false));
        }
        if(isCrouchAnimation && isCrouching) {
            return;
        }
        if(!Input.GetKey(KeyCode.LeftControl) && isCrouching) {
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

    public void Debuff(string type, float multiplier) {
        if(type == "Slow") {
            StartCoroutine(PlayerDebuffs.Instance.Slow(multiplier));
        }
    }

    void Update() {
        PlayerMove();
    }
}
