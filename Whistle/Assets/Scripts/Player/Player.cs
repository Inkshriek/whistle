using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Whistle.Conditions;
using Whistle.Characters;

[RequireComponent(typeof(CharController))]
public class Player : MonoBehaviour, ICharacter, IBehavior, IConditions, IHealth {

    public string DisplayName { get; set; }

    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;
    [SerializeField] private float crouchSpeed;
    [SerializeField] private float jumpHeight;
    [SerializeField] private float maxHealth;
    [SerializeField] private float effectiveHealth;
    [SerializeField] private float currentHealth;
    [SerializeField] private CharacterMode charMode;

    [HideInInspector] public PlayerState state;

    private Transform trans;
    private Rigidbody2D rb;
    private BoxCollider2D col;

    private Cond[] condsApplied = new Cond[12];
    private Behavior currentBehavior;
    private bool jumping;

    public float ActiveSpeed {
        get {
            switch (state) {
                case PlayerState.Walking:
                    return walkSpeed;

                case PlayerState.Crouching:
                    return crouchSpeed;

                case PlayerState.Running:
                    return runSpeed;

                default:
                    Debug.LogError("PlayerState is in an impossible position! May want to fix that quick.");
                    return 0;
            }
        }
    }

    public float Speed {
        get {
            return walkSpeed;
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

    public float CurrentHealth {
        get {
            return currentHealth;
        }
        set {
            currentHealth = Mathf.Max(0, value);
        }
    }

    public float EffectiveHealth {
        get {
            throw new System.NotImplementedException();
        }
        set {

        }
    }

    public float MaxHealth {
        get {
            return maxHealth;
        }
        set {
            maxHealth = value;
        }
    }

    public CharController Controller { get; private set; }

    public CharacterMode Mode {
        get {
            return charMode;
        }
        set {
            charMode = value;
        }
    }

    void Start () {
        DisplayName = "Ichabod";

        trans = GetComponent<Transform>();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<BoxCollider2D>();
        Controller = GetComponent<CharController>();

        state = PlayerState.Walking;
        currentBehavior = DefaultBehavior;
        jumping = false;
    }

    void Update() {
        currentBehavior(Mode);

        TickConds();
    }

    public void ApplyBehavior(Behavior newBehavior) {
        currentBehavior = newBehavior;
    }

    public void ResetBehavior() {
        currentBehavior = DefaultBehavior;
    }

    private void DefaultBehavior(CharacterMode mode) {
        if (Controller.isTouchingGround && Input.GetKeyDown(GameController.jumpKey)) {
            Controller.ApplyJump(JumpHeight);
            jumping = true;
            rb.gravityScale /= 1.5f;
        }

        if (Input.GetAxisRaw("Vertical") < 0) {
            state = PlayerState.Crouching;
        }
        else if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) {
            state = PlayerState.Running;
        }
        else {
            state = PlayerState.Walking;
        }

        if (Controller.isTouchingGround) {
            Controller.Motion = new Vector2(Input.GetAxisRaw("Horizontal") * ActiveSpeed, 0);
        }
        else {
            Controller.Motion = new Vector2(Input.GetAxisRaw("Horizontal") * Speed, 0);
        }

        if (jumping && (!Input.GetKey(GameController.jumpKey) || rb.velocity.y < 0)) {
            rb.gravityScale *= 1.5f;
            jumping = false;
        }
    }

    public Cond GetCond(string name) {
        throw new System.NotImplementedException();
    }

    public void AddCond(Cond cond) {
        cond.obj = gameObject;
        bool duplicateFound = false;
        for (int i = 0; i < condsApplied.Length; i++) {
            if (condsApplied[i] != null && condsApplied[i].name == cond.name && cond.overwriteable == true) {
                Debug.Log(condsApplied[i].name + " was reapplied to " + name + "!");
                condsApplied[i].OverwriteEffect(cond);
                duplicateFound = true;
                break;
            }
        }
        if (!duplicateFound) {
            for (int i = 0; i < condsApplied.Length; i++) {
                if (condsApplied[i] == null) {
                    condsApplied[i] = cond;
                    Debug.Log(condsApplied[i].name + " was applied to " + name + "!");
                    condsApplied[i].ApplyInitialEffect();
                    break;
                }
            }
        }
    }

    public void RemoveCond(Cond cond) {
        for (int i = 0; i < condsApplied.Length; i++) {
            if (condsApplied[i] != null && condsApplied[i].name == cond.name) {
                Debug.Log(condsApplied[i].name + " has been removed from " + name + ".");
                condsApplied[i].RemoveEffect();
                condsApplied[i] = null;
                RealignCondList();
                break;
            }
        }
    }

    protected void TickConds() {
        for (int i = 0; i < condsApplied.Length; i++) {
            if (condsApplied[i] != null) {
                condsApplied[i].time -= Time.deltaTime;
                if (condsApplied[i].time <= 0) {
                    Debug.Log(condsApplied[i].name + " has been removed from " + name + " after running out of time.");
                    condsApplied[i].RemoveEffect();
                    condsApplied[i] = null;
                    RealignCondList();
                }
                else {
                    condsApplied[i].ApplyContinuousEffect();
                }
            }
        }
    }

    protected void RealignCondList() {
        int arrayShift = 0;
        for (int i = 0; i < condsApplied.Length; i++) {
            if (condsApplied[i] == null) {
                arrayShift += 1;
            }
            if (i + arrayShift > condsApplied.Length - 1) {
                condsApplied[i] = null;
            }
            else {
                condsApplied[i] = condsApplied[i + arrayShift];
            }
        }
    }
}
