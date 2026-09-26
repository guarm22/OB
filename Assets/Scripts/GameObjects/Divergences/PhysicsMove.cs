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
            isActive = true;
            this.gameObject.AddComponent<Rigidbody>();
            rb = this.gameObject.GetComponent<Rigidbody>();
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
            StartCoroutine(ApplyMovement());
       } 
       else {
            StopAllCoroutines();
            isActive = false;
            rb.linearVelocity = Vector3.zero;
            transform.position = initialPosition;
            transform.rotation = Quaternion.Euler(initialRotation);
            Destroy(this.gameObject.GetComponent<Rigidbody>());
       }
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

        if(collision.gameObject.tag == "Untagged" || collision.gameObject.tag == "Wall") {
            audioSource.Play();
        }
    }

}
