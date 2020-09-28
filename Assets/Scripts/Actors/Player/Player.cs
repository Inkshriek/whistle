using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Whistle.Conditions;
using Spine.Unity;
using Whistle.Actors;

[RequireComponent(typeof(ActorController))]
public class Player : ActorMethods, IActor, IConditions, IHealth {

    public string DisplayName { get; set; }

    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;
    [SerializeField] private float crouchSpeed;
    [SerializeField] private float jumpHeight;
    [SerializeField] private float health;
    [SerializeField] private float healthDeficit;
    [SerializeField] private float maxHealth;
    [SerializeField] private float bleedRate;
    [SerializeField] private SkeletonAnimation anim;

    private Rigidbody2D rb;
    private List<Cond> conds;
    private bool jumping;

    public ActorController Controller { get; private set; }
    public bool Active { get; set; }
    public PlayerState State { get; set; }

    public enum PlayerState {
        //This is just a set of states intended exclusively for the player.
        Crouching,
        Walking,
        Running,
        Pushing,
        LedgeGrabbed
    }

    public float Speed {
        get {
            switch (State) {

                case PlayerState.Crouching:
                    return Modifier.AdjustNumber(conds, crouchSpeed, Modifier.Tag.Speed);

                case PlayerState.Running:
                    return Modifier.AdjustNumber(conds, runSpeed, Modifier.Tag.Speed);

                default:
                    return Modifier.AdjustNumber(conds, walkSpeed, Modifier.Tag.Speed);
            }
        }
        set {
            float multRun = runSpeed / walkSpeed;
            float multCrouch = crouchSpeed / walkSpeed;
            walkSpeed = value;
            runSpeed = value * multRun;
            crouchSpeed = value * multCrouch;
        }
    }

    public float JumpHeight {
        get {
            return Modifier.AdjustNumber(conds, jumpHeight, Modifier.Tag.JumpHeight);
        }
        set {
            jumpHeight = value;
        }
    }

    public float Health {
        get {
            return health;
        }
        set {
            health = Mathf.Clamp(value, 0, MaxHealth);
        }
    }

    public float HealthDeficit {
        get {
            return healthDeficit;
        }
        set {
            healthDeficit = value;
        }
    }

    public float MaxHealth {
        get {
            return Modifier.AdjustNumber(conds, maxHealth, Modifier.Tag.MaxHealth);
        }
        set {
            maxHealth = Mathf.Max(0, value);
        }
    }

    public float BleedRate {
        get {
            return Modifier.AdjustNumber(conds, bleedRate, Modifier.Tag.PlayerBleed);
        }
        set {
            bleedRate = Mathf.Abs(value);
        }
    }

    private void Awake() {
        DisplayName = "Ichabod";
        conds = new List<Cond>();

        rb = GetComponent<Rigidbody2D>();
        Controller = GetComponent<ActorController>();

        Active = true;
        State = PlayerState.Walking;
        jumping = false;

        Controller.Jumped += JumpAnim;
    }

    private void Update() {
        if (Active) {
            if (!Busy) {
                if (Controller.IsTouchingGround && Input.GetKeyDown(GameController.jumpKey)) {
                    Controller.ApplyJump(JumpHeight);
                    jumping = true;
                    rb.gravityScale /= 1.5f;
                }

                if (Input.GetAxisRaw("Vertical") < 0) {
                    State = PlayerState.Crouching;
                }
                else if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) {
                    State = PlayerState.Running;
                }
                else {
                    State = PlayerState.Walking;
                }

                if (Controller.IsTouchingGround) {
                    Controller.InputMotion = new Vector2(Input.GetAxisRaw("Horizontal") * Speed, 0);
                }
                else {
                    Controller.InputMotion = new Vector2(Input.GetAxisRaw("Horizontal") * walkSpeed, 0);
                }

                if (jumping && (!Input.GetKey(GameController.jumpKey) || rb.velocity.y < 0)) {
                    rb.gravityScale *= 1.5f;
                    jumping = false;
                }
            }

            TickConds();
            BleedHealth();
        }

        AnimationSelector();
    }

    public void Damage(float value, DamageType type) {
        value = Modifier.AdjustNumber(conds, value, Modifier.Tag.Damage);
        HealthDeficit += value;
        Controller.InputMotion = new Vector2(2, 0);
        Controller.ApplyJump(new Vector2(5,5));
    }

    private void BleedHealth() {
        //This whole method here intends to adjust the current health according to the deficit, making it "bleed" over time until the deficit is zero.

        float adjustment;
        if (HealthDeficit > 0) {
            adjustment = Mathf.Min(HealthDeficit, BleedRate * Time.deltaTime);
        }
        else if (HealthDeficit < 0) {
            adjustment = HealthDeficit;
        }
        else {
            adjustment = 0;
            //*lip smack* nice
        }
        
        HealthDeficit -= adjustment;
        Health -= adjustment;
    }

    private void AnimationSelector() {
        if (anim == null) {
            return;
        }

        if (!jumping) {
            if (Controller.IsTouchingGround) {
                if (Controller.InputMotion.x == 0) {
                    if (State == PlayerState.Crouching && anim.AnimationName != "crouch idle") {
                        anim.AnimationState.SetAnimation(0, "crouch idle", true);
                    }
                    else if (State != PlayerState.Crouching && anim.AnimationName != "idle") {
                        anim.AnimationState.SetAnimation(0, "idle", true);
                    }
                }
                else if (State == PlayerState.Walking && anim.AnimationName != "walk") {
                    anim.AnimationState.SetAnimation(0, "walk", true);
                }
                else if (State == PlayerState.Running && anim.AnimationName != "run") {
                    anim.AnimationState.SetAnimation(0, "run", true);
                }
                else if (State == PlayerState.Crouching && anim.AnimationName != "crouch walk") {
                    anim.AnimationState.SetAnimation(0, "crouch walk", true);
                }
            }
            else {
                anim.AnimationState.SetAnimation(0, "fall", true);
            }
        }
        
        Vector3 animScale = anim.transform.localScale;

        if (Controller.InputMotion.x != 0) {
            anim.transform.localScale = new Vector3(Mathf.Abs(animScale.x) * Mathf.Sign(Controller.InputMotion.x), animScale.y, animScale.z);
        }
    }

    private void JumpAnim(string message) {
        if (anim == null) {
            return;
        }

        anim.AnimationState.SetAnimation(0, "jump", false);
        anim.AnimationState.AddAnimation(0, "fall", true, 0);
    }

    public Cond GetCond(string name) {
        throw new System.NotImplementedException();
    }

    public void AddCond(Cond cond) {
        cond.obj = gameObject;
        bool duplicateFound = false;
        for (int i = 0; i < conds.Count; i++) {
            if (conds[i] != null && conds[i].name == cond.name && cond.overwriteable == true) {
                Debug.Log(conds[i].name + " was reapplied to " + DisplayName + "!");
                conds[i].OverwriteEffect(cond);
                duplicateFound = true;
                break;
            }
        }
        if (!duplicateFound) {
            conds.Add(cond);
            Debug.Log(cond.name + " was applied to " + name + "!");
            cond.ApplyInitialEffect();
        }
    }

    public void RemoveCond(string name) {
        for (int i = 0; i < conds.Count; i++) {
            if (conds[i] != null && conds[i].name == name) {
                Debug.Log(conds[i].name + " has been removed from " + DisplayName + ".");
                conds[i].RemoveEffect();
                conds[i] = null;
                break;
            }
        }
    }

    protected void TickConds() {
        for (int i = 0; i < conds.Count; i++) {
            if (conds[i] != null) {
                conds[i].time -= Time.deltaTime;
                if (conds[i].time <= 0) {
                    Debug.Log(conds[i].name + " has been removed from " + DisplayName + " after running out of time.");
                    conds[i].RemoveEffect();
                    conds[i] = null;
                }
                else {
                    conds[i].ApplyContinuousEffect();
                }
            }
        }
    }
}
