using UnityEngine;
using System.Collections;

public class PhysicsMove : CustomDivergence {

    public Rigidbody rb;
    public float speed = 3.5f;

    public float moveFreq = 9f;
    private float moveTimer = 0f;

    private Vector3 initialPosition;
    private Vector3 initialRotation;

    [SerializeField]
    private AudioClip hitSound;

    private AudioSource audioSource;

    private bool inAir = false;


    private Vector3 lastPosition;
    
    private bool isActive = false;
    private bool addedRigidbody;
    private bool originalIsKinematic;
    private MeshCollider[] meshColliders;
    private bool[] originalMeshColliderConvexity;

    void Awake() {
        initialPosition = transform.position;
        initialRotation = transform.rotation.eulerAngles;

        if(hitSound == null) {
            hitSound = Resources.Load<AudioClip>("Sounds/Divergence Audios/physics hit");
        }

        audioSource = this.gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 1f;
        audioSource.loop = false;
        audioSource.clip = hitSound;
    }

    public override void DoDivergenceAction(bool activate, DynamicObject gameObject) {
       if(activate) {
            if(isActive) {
                return;
            }

            PrepareDynamicMeshColliders();
            rb = GetComponent<Rigidbody>();
            if(rb == null) {
                rb = this.gameObject.AddComponent<Rigidbody>();
                addedRigidbody = true;
            }
            originalIsKinematic = rb.isKinematic;
            rb.isKinematic = false;
            isActive = true;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
            StartCoroutine(ApplyMovement());
       } 
       else {
            StopAllCoroutines();
            isActive = false;
            if(rb != null) {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
            transform.position = initialPosition;
            transform.rotation = Quaternion.Euler(initialRotation);
            if(rb != null) {
                if(addedRigidbody) {
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
    }

    private void PrepareDynamicMeshColliders() {
        meshColliders = GetComponentsInChildren<MeshCollider>(true);
        originalMeshColliderConvexity = new bool[meshColliders.Length];

        for(int i = 0; i < meshColliders.Length; i++) {
            originalMeshColliderConvexity[i] = meshColliders[i].convex;
            if(!meshColliders[i].convex) {
                meshColliders[i].convex = true;
            }
        }
    }

    private void RestoreMeshColliderConvexity() {
        if(meshColliders == null || originalMeshColliderConvexity == null) {
            return;
        }

        bool hasDynamicRigidbody = rb != null && !rb.isKinematic;
        for(int i = 0; i < meshColliders.Length; i++) {
            if(meshColliders[i] != null
                && (!hasDynamicRigidbody || originalMeshColliderConvexity[i])) {
                meshColliders[i].convex = originalMeshColliderConvexity[i];
            }
        }

        meshColliders = null;
        originalMeshColliderConvexity = null;
    }

    void Update() {
        if(isActive) {
            inAir = (transform.position - lastPosition).sqrMagnitude > 0.000001f;
            lastPosition = transform.position;
        }
    }

    private IEnumerator ApplyMovement() {
        while(true) {
            if(PlayerUI.paused) { 
                yield return null; 
                continue; 
            }
            inAir = true;
            if(Vector3.Distance(transform.position, initialPosition) > 1f) {
                //bias movement back towards initial position
                Vector3 directionToInitial = (initialPosition - transform.position).normalized;
                rb.AddForce(directionToInitial * speed* 0.5f, ForceMode.Impulse);
            }

            Vector3 randomDirection = new Vector3(Random.Range(-0.2f, 0.2f), 1, Random.Range(-0.2f, 0.2f)).normalized;
            rb.AddForce(randomDirection * speed, ForceMode.Impulse);
            yield return new WaitForSeconds(moveFreq);
        }
    }

    void OnCollisionEnter(Collision collision) {
        if(!inAir) {
            return;
        }

        if(collision.gameObject.tag == "Untagged" || collision.gameObject.tag == "Wall"
        || collision.gameObject.tag == "Floor") {
            audioSource.Play();
        }
    }

}
