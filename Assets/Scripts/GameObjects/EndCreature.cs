using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EndCreature : MonoBehaviour
{
    public GameObject player;
    public float sightRange = 15;
    public Vector3 posCheck;
    Vector3 dest;
    bool isDestSet;
    [SerializeField] float walkRange;

    public AudioClip yippie;
    private float timeSinceLastYip = 1;

    private void CreaturePatrol() {
    //if the creature can see the player, chasing the player takes priority over everything else
    MoveTowards(player.transform.position);

    if(timeSinceLastYip > 2) {
        AudioSource.PlayClipAtPoint(yippie, transform.position);
        timeSinceLastYip = 0;
    }
    timeSinceLastYip += Time.deltaTime;

    if(amTouchingPlayer()) {
        GameSystem.Instance.EndGame();
    }
    return;
}

    private void MoveTowards(Vector3 target) {
        Vector3 direction = (target - transform.position).normalized;
        transform.position += direction * 5f * Time.deltaTime;
    }

    private bool amTouchingPlayer() {
        if(Vector3.Distance(transform.position, player.transform.position) < 1.3f) {
                return true;
            }
        return false;
    }

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if(SC_FPSController.paused || GameSystem.Instance.GameOver) {
            return;
        }
        CreaturePatrol();
    }
}
