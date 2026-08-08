using UnityEngine;
using System.Collections;
using DG.Tweening;

public class RealmSphere : CustomDivergence {
    private float speed = 40f;

    private Vector3 centerPoint;

    private Vector3 originalScale;

    private float targetDistance = .75f;

    private string room;
    private bool active = false;
    private GameObject player;

    private bool inRealm = false;
    private bool returning = false;

    void Start() {
        centerPoint = transform.position;
        originalScale = transform.localScale;
        transform.localScale = Vector3.zero;
        player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update() {
        if(!active) { return; }

        if(inRealm) { RealmBehavior(); return; }

        //if touching player
        if(Vector3.Distance(this.transform.position, player.transform.position) < 1f) {
            PunctureRealm.Instance.InitRealm(this.gameObject);
            inRealm = true;
        }

        //if player is in room, move towards player
        if(PlayerUI.Instance.GetCurrentRoom() == room) {
            transform.DOMove(player.transform.position, 2.5f);
        }

        //slowly move around centerpoint
        transform.RotateAround(centerPoint, Vector3.up, speed * Time.deltaTime);
        Vector3 directionFromTarget = (transform.position - centerPoint).normalized;
        transform.position = centerPoint + directionFromTarget * targetDistance;
    }

    public void Return() {
        returning = true;
    }

    private void RealmBehavior() {
        if(returning) {
            StartCoroutine(Cooldown());
        }
        transform.RotateAround(centerPoint, Vector3.up, speed * Time.deltaTime);
        Vector3 directionFromTarget = (transform.position - centerPoint).normalized;
        transform.position = centerPoint + directionFromTarget * targetDistance;
    }

    private IEnumerator Cooldown() {
        returning = false;
        yield return new WaitForSeconds(10f);
        inRealm = false;
    }

    public override void DoDivergenceAction(bool activate, DynamicObject gameObject) {
        room = gameObject.Room;
        if(activate) {
            active = true;
            transform.position = centerPoint + new Vector3(0, 0, 2);
            transform.DOScale(originalScale, .5f).SetEase(Ease.OutBack);
        } else {
            active = false;
            transform.DOScale(Vector3.zero, .5f).SetEase(Ease.InBack);
        }
    }
}
