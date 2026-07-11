using System.Collections;
using UnityEngine;
using DG.Tweening;

public class WoodFenceOpen : MonoBehaviour {

    private bool inAnim = false;
    private bool opened = false;

    public float openTime = 2f;

    public AudioSource fenceSound;
    public AudioClip lockedSound;
    public bool locked = false;

    private GameObject Player;
    private Transform gateTransform;

    private Vector3 openRotation = new Vector3(0, 90, 0);
    private Vector3 closeRotation = new Vector3(0, 0, 0);

    private Vector3 movementAmount = new Vector3(-0.40f, 0, .215f);

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        Player = GameObject.Find("Player");
        gateTransform = transform.GetChild(0);
    }

    public void ChangeGateState(Vector3 targetRotation) {
        if(inAnim) {
            return;
        }
        Vector3 TargetMovement = opened ? -movementAmount : movementAmount; // Move in the opposite direction if closing
        StartCoroutine(animationTimer());
        StartCoroutine(GateAnimation(targetRotation, TargetMovement));
        StartCoroutine(GateEffects());
        fenceSound.Play();
    }

    private IEnumerator GateAnimation(Vector3 targetRotation, Vector3 targetMovement) {
        Vector3 extraRotation = new Vector3(0, Random.Range(-8f, 8f), 0); // Add some random rotation for a more natural effect
        gateTransform.DOLocalRotate(targetRotation + extraRotation, openTime);
        gateTransform.DOLocalMove(gateTransform.localPosition + targetMovement, openTime*0.8f).SetEase(Ease.OutQuad); // Move the gate while it opens/closes
        yield return new WaitForSeconds(openTime);
        gateTransform.DOLocalRotate(targetRotation, 0.5f); // Ensure it ends at the exact target rotation
    }
    
    private IEnumerator GateEffects() {
        //move gate up and down to simulate gate effect for first 0.25 seconds
        float elapsedTime = 0f;
        while (elapsedTime < 0.25f) {
            float yOffset = Mathf.Sin(elapsedTime * Mathf.PI * 4) * 0.02f; // Oscillate up and down
            gateTransform.localPosition = new Vector3(gateTransform.localPosition.x, yOffset, gateTransform.localPosition.z);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

     private IEnumerator animationTimer() {
        inAnim = true;
        yield return new WaitForSeconds(openTime);
        opened = !opened;
        inAnim = false;
    }

    // Update is called once per frame
    void Update() {
        if(Vector3.Distance(this.transform.position, Player.transform.position) < 4f) {
            if(Input.GetKeyDown(KeyCode.Mouse0)|| Input.GetKeyDown(KeybindManager.instance.GetKeybind("Interact"))) {
                //check if locked
                if(locked) {
                    //If locked, play locked sound and return
                    fenceSound.PlayOneShot(lockedSound);
                    return;
                }
                //If in animation, return
                if(inAnim) {
                    return;
                }
                //since it is not locked and not in animation, we can open or close the gate
                //if it's not opened, open it, otherwise close it
                if(!opened) {
                    ChangeGateState(openRotation);
                }
                else {
                    ChangeGateState(closeRotation);
                }
            }
        }
    }
}
