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

    public float walkingSpeed = 7.5f;
    public float runningSpeed = 11.5f;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;
    public Camera playerCamera;
    public float lookSpeed = 2.0f;
    public float lookXLimit = 45.0f;

    public float maxDistance = 5.0f;

    public string targetTag = "Room";

    public GameObject body;

    public string interactableTag;

    CharacterController characterController;
    Vector3 moveDirection = Vector3.zero;
    float rotationX = 0;

    [HideInInspector]
    public bool canMove = true;

    public bool inJournal = false;
    public bool inMenu = false;

    void Start()
    {
        characterController = GetComponent<CharacterController>();

        // Lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        PopulateSelectorUI();
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
            ui.transform.localScale = new Vector3(3f,3f,3f);
            ui.name = rooms[i++].name;
            iter+=100f;
        }

        GameObject[] types = GameObject.FindGameObjectsWithTag("Anomaly Type");
        i = 0;
        iter = 0f;
        //Creates each of the type selectors
        foreach(GameObject type in types) {
            //Create Object
            GameObject ui = Instantiate(togglePrefab, transform);
            //Set parent to correct UI element
            ui.transform.SetParent(typeSelectionUI.transform);
            //Set position
            ui.transform.localPosition = new Vector3(0f, iter, 0f);
            //Set text
            ui.transform.GetChild(1).gameObject.GetComponent<Text>().text = types[i].name;
            //Scale is set to 0 by default for some reason
            ui.transform.localScale = new Vector3(3f,3f,3f);
            ui.name = types[i++].name;
            //Increase Y value of next object
            iter+=100f;
        }
    }

    void SelectionMenu() {
        if(inJournal) {
            return;
        }

        //if menu is open, check if tab is pressed to close, otherwise stop movement
        if(inMenu) {
            if(Input.GetKeyDown(KeyCode.Tab) || GameSystem.Guessed) {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                inMenu = false;

                selectionUI.SetActive(false);
                defaultUI.SetActive(true);
                canMove = true;
            }

            //return;
        }

        else if(Input.GetKeyDown(KeyCode.Tab) && !GameSystem.Guessed) {
            //activate selection UI
            selectionUI.SetActive(true);
            defaultUI.SetActive(false);

            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
            inMenu = true;
            canMove = false;
            //return;
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
                    Debug.Log("Did not hit object with target tag.");

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

        if (Input.GetButton("Jump") && canMove && characterController.isGrounded)
        {
            moveDirection.y = jumpSpeed;
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }

        // Apply gravity. Gravity is multiplied by deltaTime twice (once here, and once below
        // when the moveDirection is multiplied by deltaTime). This is because gravity should be applied
        // as an acceleration (ms^-2)
        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        // Move the controller
        characterController.Move(moveDirection * Time.deltaTime);

        // Player and Camera rotation
        if (canMove)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }
    }

    void Update()
    {
        SelectionMenu();

        Interact();

        PlayerMove();
    }
}
