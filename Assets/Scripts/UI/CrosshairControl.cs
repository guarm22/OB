using UnityEngine;
using UnityEngine.UI;

public class CrosshairControl : MonoBehaviour {
    public Sprite defaultCrosshair;
    public Sprite collectibleCrosshair;

    public static CrosshairControl Instance;

    private bool objectInRange;
    public string targetTag = "Collectible";
    public float detectionRadius = 3f;

    private Vector3 collectibleCrosshairScale = new Vector3(4.5f,3f,4);

    void Start() {
        objectInRange = false;
        Instance = this;
    }

    public void SetObjectInRange(bool inRange, string tag = "") {
        objectInRange = inRange;
        if (objectInRange && tag == "") {
            GetComponent<Image>().sprite = collectibleCrosshair;
            GetComponent<RectTransform>().localScale = collectibleCrosshairScale; 
        } 
        /*else if (objectInRange && tag == "Door") {
            GetComponent<Image>().sprite = collectibleCrosshair;
            GetComponent<RectTransform>().localScale = new Vector3(4.5f,3f,4); 
        } */
        else {
            GetComponent<RectTransform>().localScale = Vector3.one;
            GetComponent<Image>().sprite = defaultCrosshair;
        }
    }

    // Update is called once per frame
    void Update() {
        // Check for objects with the specified tag within the detection radius
        /*Collider[] hitColliders = Physics.OverlapSphere(SC_FPSController.Instance.gameObject.transform.position, detectionRadius);
        // Iterate through the results
        foreach (var hitCollider in hitColliders) {
            // Check if the hit object has the desired tag
            if (hitCollider.CompareTag(targetTag)) {
                // Found an object with the tag within range
                objectInRange = true;
                GetComponent<Image>().sprite = collectibleCrosshair;
                GetComponent<RectTransform>().localScale = new Vector3(4.5f,3f,4); 
                return;
            }
        }
        GetComponent<RectTransform>().localScale = new Vector3(1,1,1);
        GetComponent<Image>().sprite = defaultCrosshair;
        objectInRange = false;
        */
    }
}
