using UnityEngine;
using DG.Tweening;

public class MoveFollow : CustomDivergence {

    private Transform target;

    [SerializeField]
    private float followSpeed = 1.5f;

    [SerializeField]
    private float minDistance = 1.5f;

    private Vector3 originalPos;

    private bool followActive = false;

    private bool currentlyFollowing = false;

    private string room;

    void Start() {
        target = GameObject.Find("Player").transform;
        originalPos = transform.position;
    }

    void Update() {
        if(PlayerUI.paused) { return; }
        if(followActive == false) {
            return;
        }

        if(PlayerUI.Instance.currentRoom != room) {
            currentlyFollowing = false;
            transform.DOMove(originalPos, 0.75f);
        }
        else {
            currentlyFollowing = true;
        }

        if(currentlyFollowing) {
            //don't move if you are in the line of sight of the player
            if(SC_FPSController.Instance.CheckInLOS(transform) == true) {
                return;
            }

            float distance = Vector3.Distance(transform.position, target.position);
            if(distance > minDistance) {
                Vector3 direction = (target.position - transform.position).normalized;

                //keep y value of original object
                direction.y = 0f;

                transform.position += direction * followSpeed * Time.deltaTime;
            }
        }
    }

    public override void DoDivergenceAction(bool activate, DynamicObject gameObject) {
        room = gameObject.Room;
        followActive = activate;
    }
}
