using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MoveAtPlayer : CustomDivergence {
    public Rigidbody rb;
    public float speed = 3.5f;
    public float moveFreq = 1.5f;

    [SerializeField]
    private float screenBlackFadeDuration = 0.5f;

    [SerializeField]
    private AudioClip hitSound;

    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private Collider startingRoomCollider;
    private AudioSource audioSource;
    private string startingRoom;
    private bool isActive;
    private bool addedRigidbody;
    private bool originalIsKinematic;
    private MeshCollider[] meshColliders;
    private bool[] originalMeshColliderConvexity;
    private Coroutine movementRoutine;
    private Coroutine blackoutRoutine;
    private GameObject blackoutPanel;
    private Image blackoutImage;
    private Color previousBlackoutColor;
    private bool previousBlackoutActive;

    private void Awake() {
        initialPosition = transform.position;
        initialRotation = transform.rotation;
        startingRoom = DynamicObject.getRoomName(transform);
        CacheStartingRoomCollider();

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        if(hitSound == null) {
            hitSound = Resources.Load<AudioClip>("Sounds/Divergence Audios/physics hit");
        }
        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.spatialBlend = 1f;
        if (hitSound == null) {
            Debug.LogWarning("MoveAtPlayer has no hit sound assigned.", this);
        }
    }

    public override void DoDivergenceAction(bool activate, DynamicObject divergence) {
        if (activate) {
            if (isActive) {
                return;
            }

            if (startingRoomCollider == null) {
                startingRoom = divergence.Room;
                CacheStartingRoomCollider();
            }

            if (startingRoomCollider == null) {
                Debug.LogError("MoveAtPlayer requires a collider on its starting Room object.", this);
                return;
            }

            PrepareDynamicMeshColliders();
            rb = GetComponent<Rigidbody>();
            if (rb == null) {
                rb = gameObject.AddComponent<Rigidbody>();
                addedRigidbody = true;
            }

            originalIsKinematic = rb.isKinematic;
            rb.isKinematic = false;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
            isActive = true;
            movementRoutine = StartCoroutine(MoveTowardsPlayer());
            return;
        }

        StopMovementAndReset();
    }

    private void CacheStartingRoomCollider() {
        Transform roomTransform = transform;
        while (roomTransform != null) {
            if (roomTransform.CompareTag("Room")) {
                startingRoomCollider = roomTransform.GetComponent<Collider>();
                if (startingRoomCollider != null) {
                    return;
                }
            }
            roomTransform = roomTransform.parent;
        }

        if (!string.IsNullOrEmpty(startingRoom)) {
            GameObject roomObject = GameObject.Find(startingRoom);
            if (roomObject != null) {
                startingRoomCollider = roomObject.GetComponent<Collider>();
            }
        }
    }

    private void PrepareDynamicMeshColliders() {
        meshColliders = GetComponentsInChildren<MeshCollider>(true);
        originalMeshColliderConvexity = new bool[meshColliders.Length];

        for (int i = 0; i < meshColliders.Length; i++) {
            originalMeshColliderConvexity[i] = meshColliders[i].convex;
            if (!meshColliders[i].convex) {
                meshColliders[i].convex = true;
            }
        }
    }

    private void RestoreMeshColliderConvexity() {
        if (meshColliders == null || originalMeshColliderConvexity == null) {
            return;
        }

        bool hasDynamicRigidbody = rb != null && !rb.isKinematic;
        for (int i = 0; i < meshColliders.Length; i++) {
            if (meshColliders[i] != null
                && (!hasDynamicRigidbody || originalMeshColliderConvexity[i])) {
                meshColliders[i].convex = originalMeshColliderConvexity[i];
            }
        }

        meshColliders = null;
        originalMeshColliderConvexity = null;
    }

    private IEnumerator MoveTowardsPlayer() {
        while (isActive) {
            if (PlayerUI.paused || GameSystem.Instance.GameOver) {
                yield return null;
                continue;
            }

            SC_FPSController player = SC_FPSController.Instance;
            if (player != null && PlayerUI.Instance != null
                && PlayerUI.Instance.GetCurrentRoom() == startingRoom) {
                Vector3 direction = player.playerCamera.transform.position - rb.position;
                if (direction.sqrMagnitude > 0.0001f) {
                    rb.AddForce(direction.normalized * speed, ForceMode.Impulse);
                }
            }

            yield return new WaitForSeconds(Mathf.Max(0.1f, moveFreq));
        }
    }

    private void FixedUpdate() {
        if (!isActive || rb == null || startingRoomCollider == null) {
            return;
        }

        if (!startingRoomCollider.bounds.Contains(rb.position)) {
            rb.position = initialPosition;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    private void OnCollisionEnter(Collision collision) {
        if (IsPlayerCollider(collision.collider)) {
            HandlePlayerContact();
        }
        else if (collision.gameObject.tag == "Untagged" || collision.gameObject.tag == "Wall"
         || collision.gameObject.tag == "Floor") {
            PlayHitSound();
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (IsPlayerCollider(other)) {
            HandlePlayerContact();
        }
    }

    private bool IsPlayerCollider(Collider other) {
        return other.GetComponentInParent<SC_FPSController>() != null;
    }

    private void HandlePlayerContact() {
        if (!isActive) {
            return;
        }

        PlayHitSound();
        if (PlayerUI.Instance != null && PlayerUI.Instance.reportDeviceUp) {
            PlayerUI.Instance.PhysicalUI();
        }

        if (blackoutRoutine != null) {
            return;
        }

        SC_FPSController player = SC_FPSController.Instance;
        if (player == null || player.blacknessPanel == null) {
            Debug.LogError("MoveAtPlayer could not find the player's blackness panel.", this);
            return;
        }

        blackoutPanel = player.blacknessPanel;
        blackoutImage = blackoutPanel.GetComponent<Image>();
        if (blackoutImage == null) {
            Debug.LogError("The player's blackness panel requires an Image component.", blackoutPanel);
            blackoutPanel = null;
            return;
        }

        if (blackoutPanel.activeSelf) {
            blackoutPanel = null;
            blackoutImage = null;
            return;
        }

        previousBlackoutActive = blackoutPanel.activeSelf;
        previousBlackoutColor = blackoutImage.color;
        blackoutPanel.SetActive(true);
        blackoutImage.color = Color.black;
        blackoutRoutine = StartCoroutine(BlackoutPlayerScreen());
    }

    private void PlayHitSound() {
        if (!isActive) {
            return;
        }
        if (hitSound == null) {
            return;
        }

        audioSource.PlayOneShot(hitSound);
    }

    private IEnumerator BlackoutPlayerScreen() {
        float duration = Mathf.Max(0.01f, screenBlackFadeDuration);
        float elapsed = 0f;
        while (elapsed < duration) {
            elapsed += Time.unscaledDeltaTime;
            Color color = Color.black;
            color.a = Mathf.Lerp(1f, 0f, Mathf.Clamp01(elapsed / duration));
            blackoutImage.color = color;
            yield return null;
        }

        RestoreBlackoutPanel();
        blackoutRoutine = null;
    }

    private void RestoreBlackoutPanel() {
        if (blackoutPanel == null || blackoutImage == null) {
            return;
        }

        blackoutImage.color = previousBlackoutColor;
        blackoutPanel.SetActive(previousBlackoutActive);
        blackoutPanel = null;
        blackoutImage = null;
    }

    private void StopMovementAndReset() {
        isActive = false;
        if (movementRoutine != null) {
            StopCoroutine(movementRoutine);
            movementRoutine = null;
        }

        if (blackoutRoutine != null) {
            StopCoroutine(blackoutRoutine);
            blackoutRoutine = null;
        }
        RestoreBlackoutPanel();

        if (rb != null) {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            transform.SetPositionAndRotation(initialPosition, initialRotation);
            if (addedRigidbody) {
                rb.isKinematic = true;
                Destroy(rb);
                rb = null;
                addedRigidbody = false;
            }
            else {
                rb.isKinematic = originalIsKinematic;
            }
        }
        RestoreMeshColliderConvexity();
    }

    private void OnDisable() {
        StopMovementAndReset();
    }
}
