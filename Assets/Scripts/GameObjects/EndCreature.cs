using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EndCreature : MonoBehaviour
{
    public GameObject player;
    public AudioClip yippie;
    private float timeSinceLastYip = 1;
    public float speed = 5f;

    private void CreaturePatrol() {
    //if the creature can see the player, chasing the player takes priority over everything else
        MoveTowards(player.transform.position);

        if(timeSinceLastYip > 2) {
            AudioSource.PlayClipAtPoint(yippie, transform.position);
            timeSinceLastYip = 0;
        }
        timeSinceLastYip += Time.deltaTime;

        if(amTouchingPlayer()) {
            StartCoroutine(GameSystem.Instance.EndGame("yippie"));
        }
    }

    private void MoveTowards(Vector3 target) {
        Vector3 direction = (target - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;
    }
    private bool amTouchingPlayer() {
        return Vector3.Distance(transform.position, player.transform.position) < 1.3f; 
    }

    void Start() {
        player = GameObject.Find("Player");
    }

    void Update() {
        if(PlayerUI.paused || GameSystem.Instance.GameOver) {
            return;
        }
        CreaturePatrol();
    }
}
