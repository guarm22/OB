using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]

public class SC_FPSController : MonoBehaviour
{
    ///Default UI that appears on the bottom right of the players screen
    public GameObject defaultUI;
    //UI that appears on the bottom right when the player presses tab
    public GameObject selectionUI;
    public GameObject roomSelectionUI;
    public GameObject typeSelectionUI;
    public GameObject journalUI;
    public GameObject journalPanel;
    public GameObject togglePrefab;
    public GameObject roomText;
    public GameObject escapeMenuUI;
    public GameObject EndGameUI;
    public float walkingSpeed = 7.5f;
    public float originalWalkSpeed;
    public float originalRunSpeed;
    public float runningSpeed = 11.5f;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;
    public Camera playerCamera;
    public float lookSpeed = 2.0f;
    public float lookXLimit = 90.0f;
    public float maxDistance = 5.0f;
    public string targetTag = "Room";
    public string tutTag = "Tutorial";
    public string interactableTag;
    CharacterController characterController;
    Vector3 moveDirection = Vector3.zero;
    float rotationX = 0;
    [HideInInspector]
    public bool canMove = true;
    public bool inJournal = false;
    public bool inMenu = false;
    public static bool paused = false;
    public static SC_FPSController Instance;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        Instance = this;
        // Lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        originalRunSpeed = runningSpeed;
        originalWalkSpeed = walkingSpeed;
        paused = false;
        PopulateSelectorUI();
    }

    public string GetPlayerRoom() {
        return roomText.GetComponent<Text>().text;
    }

    void PopulateSelectorUI() {
        GameObject[] rooms = GameObject.FindGameObjectsWithTag("Room");
        //Float that determines height between each selector
        float iter = 0f;
        //for getting each element in the list
        int i = 0;

        //Creates each of the room selectors in the selection UI
        foreach(GameObject room in rooms) {
            GameObject ui = Instantiate(togglePrefab, transform);
            ui.transform.SetParent(roomSelectionUI.transform);
            ui.transform.localPosition = new Vector3(0f, iter, 0f);
            ui.transform.GetChild(1).gameObject.GetComponent<Text>().text = rooms[i].name;
            //ui.transform.GetChild(1).gameObject.GetComponent<Text>().fontSize = 40;

            ui.GetComponent<Toggle>().onValueChanged.AddListener(
                delegate { RoomSelection.Instance.Select(ui); });

            ui.transform.localScale = new Vector3(3f,3f,3f);
            ui.name = rooms[i++].name;
            iter+=100f;
        }

        List<string> types = DynamicObject.GetAllAnomalyTypes();
        iter = 0f;
        //Creates each of the type selectors
        foreach(string type in types) {
            GameObject ui = Instantiate(togglePrefab, transform);
            ui.transform.SetParent(typeSelectionUI.transform);
            ui.transform.localPosition = new Vector3(0f, iter, 0f);
            ui.transform.GetChild(1).gameObject.GetComponent<Text>().text = type;
            ui.transform.localScale = new Vector3(3f,3f,3f);
            ui.name = type;
            iter+=100f;
            ui.GetComponent<Toggle>().onValueChanged.AddListener(
            delegate { TypeSelection.Instance.Select(ui); });       
        }
    }

    private void turnOffSelection() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        inMenu = false;
        selectionUI.SetActive(false);
        defaultUI.transform.GetChild(0).GetChild(1).gameObject.SetActive(true);
        canMove = true;
    }
    private void turnOnSelection() {
        selectionUI.SetActive(true);
        defaultUI.transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        inMenu = true;
        canMove = false;
    }

    void SelectionMenu() {
        if(inJournal) {
            return;
        }
        bool beforeTimer = Time.time - GameSystem.LastGuess >= GameSystem.Instance.GuessLockout-GameSystem.Instance.reportTextTimer;
        //if menu is open, check if tab is pressed to close, otherwise stop movement
        if(inMenu) {
            if(Input.GetKeyDown(KeyCode.Tab) || Time.time - GameSystem.LastGuess < 1) {
                turnOffSelection();
            }
        }
        else if(Input.GetKeyDown(KeyCode.Tab) && beforeTimer) {
            turnOnSelection();
        }
    }

    void Interact() {
        if(Input.GetKeyDown(KeyCode.E)) { 
            if(inJournal) {
                canMove = true;
                inJournal = false;
                journalUI.SetActive(false);
                return;
            }
            // Determine the direction that the player is looking
            Vector3 lookDirection = Camera.main.transform.forward;

            // Cast a ray in the look direction
            RaycastHit hit;
            if (Physics.Raycast(transform.position, lookDirection, out hit, maxDistance)) {
                // Check if the object has the desired tag
                if (hit.collider.gameObject.CompareTag(interactableTag)){
                    // The ray hit an object with the target tag
                    //hit.collider.gameObject.name
                    GameObject found = Interactable.GetInteractableByName(hit.collider.gameObject.name);

                    if(found != null) {
                        Interactable script = found.GetComponent<Interactable>();
                        string[] text = script.GetJournalText();

                        JournalPopup journalScript = journalPanel.GetComponent<JournalPopup>();
                        journalScript.SetText(text);

                        journalUI.SetActive(true);
                        inJournal = true;
                        canMove = false;
                    }
                }
                else {
                    // The ray did not hit an object with the target tag

                }
            }
        }
    }

    //Currently used for figuring out which room the player is in and displaying it on the top right
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals(targetTag))
        {
            // Player is within a GameObject with the specified tag
        //Debug.Log("Player is within a GameObject with the tag: " + targetTag + " with object name: " + other.gameObject.name);
            roomText.GetComponent<Text>().text = other.gameObject.name;
        }

        if (other.tag.Equals(tutTag)) {
            Tutorial.Instance.ActivateTrigger(other.gameObject);
        }
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

    private void openEscape() {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        escapeMenuUI.SetActive(true);
        paused = true;
        defaultUI.SetActive(false);
    }

    public void closeEscape() {
        paused = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        escapeMenuUI.SetActive(false);
        defaultUI.SetActive(true);
    }

    private void EscapeMenu() {
        //CHANGE TO ESCAPE
        if(Input.GetKeyDown(KeyCode.Q)) { 
            if(!paused) {
                openEscape();
            }
            else {
                closeEscape();
            }
        }
        if(Input.GetKeyDown(KeyCode.P)) {
            Popup.Instance.OpenPopup("Test message!");
        }
    }

    private void EndingGame() {
        EndGameUI.SetActive(true);
    }

    public void Debuff(string type) {
        if(type == "Slow") {
            StartCoroutine(PlayerDebuffs.Instance.Slow());
        }
    }

    void Update()
    {
        if(Popup.Instance != null) {
            if(Popup.Instance.isPopupOpen ) {
                return;
            }
        }
        if(CreatureControl.Instance.IsJumpscareFinished) {
            EndingGame();
            return;
        }
        //now waiting for jumpscare to finish, if any
        if(GameSystem.Instance.GameOver) {
            return;
        }
        EscapeMenu();
        if(paused) {
            return;
        }
        SelectionMenu();
        Interact();
        PlayerMove();
    }
}
