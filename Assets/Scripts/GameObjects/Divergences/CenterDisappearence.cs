using UnityEngine;

public class CenterDisappearence : CustomDivergence
{
    private bool active = false;

    private MeshRenderer meshRenderer;
    private Collider objectCollider;

    void Start() {
        meshRenderer = GetComponent<MeshRenderer>();
        objectCollider = GetComponent<Collider>();
    }

    private void TurnOff() {
        meshRenderer.enabled = false;
        objectCollider.enabled = false;
    }

    private void TurnOn() {
        meshRenderer.enabled = true;
        objectCollider.enabled = true;
    }

    void Update() {
        if(PlayerUI.paused) { return; }
        if(active) {
            if(Vector3.Distance(transform.position, SC_FPSController.Instance.transform.position) < 4f) {
                TurnOn();
                return;
            }
            //check if the player is looking at this object
            if(SC_FPSController.Instance.CheckInLOS(transform, 40, SC_FPSController.Instance.FOV-10f) == true) {
                TurnOn();
            }
            else {
                //if the player is not looking at this object, make it appear
                TurnOff();
            }
        }
    }

    public override void DoDivergenceAction(bool activate, DynamicObject gameObject) {
        active = activate;

        if(active) {
            TurnOff();
        }
        else {
            TurnOn();
        }
    }
}
