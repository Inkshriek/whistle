using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour {

    public PlayerController controller;
    private Transform trans;
    private SkeletonAnimation anim;
    private string animationState;

	// Use this for initialization
	void Start () {
        trans = GetComponent<Transform>();
        anim = GetComponent<SkeletonAnimation>();
        animationState = "";
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        
        if (controller.isTouchingGround) {
            if (controller.moveDirection != 0) {

                if (controller.isRunning)
                    changeState("run", true);
                else if (controller.isCrouching)
                    changeState("crouch_walk", true);
                else
                    changeState("walk", true);
            }
            else {
                if (controller.isCrouching)
                    changeState("crouch_idle", true);
                else
                    changeState("idle", true);
            }
        }
        else if (controller.airV > 0)
            changeState("jump", false);
        else
            changeState("fall", true);

        if (controller.moveDirection != 0)
            trans.localScale = new Vector3(Mathf.Abs(trans.localScale.x) * Mathf.Sign(controller.moveDirection), trans.localScale.y, trans.localScale.z);
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
