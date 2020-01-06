using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Whistle.Actors;

[ExecuteAlways]
[RequireComponent(typeof(ActorControllerDebug))]
public class PlayerDebug : ActorMethods, IActor {
    public string DisplayName { get; set; }

    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;
    [SerializeField] private float crouchSpeed;
    [SerializeField] private float jumpHeight;
    [SerializeField] private float health;
    [SerializeField] private float healthDeficit;
    [SerializeField] private float bleedRate;

    private Rigidbody2D rb;
    private bool jumping;

    public ActorController Controller { get; private set; }
    public bool Active { get; set; }
    public Player.PlayerState State { get; set; }

    public float Speed {
        get {
            switch (State) {

                case Player.PlayerState.Crouching:
                    return crouchSpeed;

                case Player.PlayerState.Running:
                    return runSpeed;

                default:
                    return walkSpeed;
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
            return jumpHeight;
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
            health = Mathf.Max(value, 0);
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

    public float BleedRate {
        get {
            return bleedRate;
        }
        set {
            bleedRate = Mathf.Abs(value);
        }
    }

    private void Awake() {
        DisplayName = "Ichabod";

        rb = GetComponent<Rigidbody2D>();
        Controller = GetComponent<ActorController>();

        Active = true;
        State = Player.PlayerState.Walking;
        jumping = false;
    }

    private void Update() {
        if (Active) {
            if (Controller.IsTouchingGround && Input.GetKeyDown(GameController.jumpKey)) {
                Controller.ApplyJump(JumpHeight);
                jumping = true;
                rb.gravityScale /= 1.5f;
            }

            if (Input.GetAxisRaw("Vertical") < 0) {
                State = Player.PlayerState.Crouching;
            }
            else if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) {
                State = Player.PlayerState.Running;
            }
            else {
                State = Player.PlayerState.Walking;
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

            BleedHealth();
        }
    }

    public void Damage(float value, DamageType type) {
        HealthDeficit += value;
    }

    private void BleedHealth() {

        float adjustment;
        if (HealthDeficit > 0) {
            adjustment = Mathf.Min(HealthDeficit, BleedRate * Time.deltaTime);
        }
        else if (HealthDeficit < 0) {
            adjustment = HealthDeficit;
        }
        else {
            adjustment = 0;
        }

        HealthDeficit -= adjustment;
        Health -= adjustment;
    }
}
