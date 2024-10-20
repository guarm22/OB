using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Chaser : CreatureBase
{
    public float slowMultiplier = 0.3f;
    private float foundPlayerSoundTimer = 5f;
    private float lastTimesincePlayerFound = 0f;

    public Volume disorientingLocalVolumePrefab;
    public bool canSeeTarget = false;

    private bool amTouchingPlayer() {
        if(Vector3.Distance(transform.position, player.transform.position) < 1.3f) {
                return true;
            }
        return false;
    }

    protected override void CreatureSounds() {
        soundTimerStart += Time.deltaTime;
        lastTimesincePlayerFound += Time.deltaTime;

        //want to play sounds quicker if creature can see player
        if(canSeePlayer()) {
            if(lastTimesincePlayerFound >= foundPlayerSoundTimer) {
                AudioSource.PlayClipAtPoint(closeSound, transform.position);
                lastTimesincePlayerFound = 0f;
                soundTimerStart -= 4.5f;
            }
        }

        if(soundTimerStart >= soundTimerMax) {
            if(Vector3.Distance(player.transform.position, transform.position) < closeSoundRange) {
                AudioSource.PlayClipAtPoint(closeSound, transform.position);
            }
            else {
                AudioSource.PlayClipAtPoint(farSound, transform.position);
            }
            soundTimerStart = 0f;
        }
    }



    protected override void Awake() {
        base.Awake();
    }

    private void DisorientPlayer() {
        Instantiate(disorientingLocalVolumePrefab, player.transform.position, Quaternion.identity);
    }

    // Update is called once per frame
    protected override void Update() {
        base.Update();
        if(amTouchingPlayer()) {
            DisorientPlayer();
            CreatureControl.Instance.RemoveCreature(gameObject);
        }
    }
}
