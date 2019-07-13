using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;
using Whistle.Actors;

public class PlayerAnimator : MonoBehaviour {

    public Player player;
    private ActorController controller;
    private Transform trans;
    private SkeletonAnimation anim;
    private string animationState;

	// Use this for initialization
	void Start () {
        trans = GetComponent<Transform>();
        anim = GetComponent<SkeletonAnimation>();
        controller = player.GetComponent<ActorController>();
        animationState = "";
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        
        if (controller.isTouchingGround) {
            if (controller.Motion.x != 0) {
                
                switch (player.Action) {
                    case PlayerState.Running:
                        changeState("run", true);
                        break;
                    case PlayerState.Crouching:
                        changeState("crouch_walk", true);
                        break;
                    case PlayerState.Walking:
                        changeState("walk", true);
                        break;
                }
            }
            else {
                switch (player.Action) {
                    case PlayerState.Crouching:
                        changeState("crouch_idle", true);
                        break;
                    default:
                        changeState("idle", true);
                        break;
                }
            }
        }
        else if (controller.gameObject.GetComponent<Rigidbody2D>().velocity.y > 0)
            changeState("jump", false);
        else
            changeState("fall", true);

        if (controller.Motion.x != 0)
            trans.localScale = new Vector3(Mathf.Abs(trans.localScale.x) * Mathf.Sign(controller.Motion.x), trans.localScale.y, trans.localScale.z);
	}

    void changeState(string animation, bool loop)
    {
        if (animationState.Equals(animation) != true)
        {
            anim.state.SetAnimation(1, animation, loop);
            if (animation == "jump")
            {
                anim.state.AddAnimation(1, "fall", true, 0.5f);
            }
            animationState = animation;
        }
    }
}
