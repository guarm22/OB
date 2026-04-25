using UnityEngine;

public class LurkerAnimationController : MonoBehaviour {

    private Animator animator;
    private CreatureBase creature;

    private float defaultSpeed;

    void Start() {
        animator = GetComponent<Animator>();  
        creature = GetComponent<CreatureBase>(); 
        defaultSpeed = animator.speed;
    }

    void Update() {
        if(PlayerUI.paused) {
            //freeze animation when paused
            animator.speed = 0;
            return;
        }
        animator.speed = defaultSpeed;
        animator.SetBool("isDestinationSet", creature.isDestSet);
        animator.SetBool("isPlayerSeen", ((Lurker)creature).attackingPlayer);
        animator.SetBool("isPlayerInRange", creature.amCloseToPlayer());
        animator.SetBool("AtOriginalPos", ((Lurker)creature).atSpawnPos);
    }
}
