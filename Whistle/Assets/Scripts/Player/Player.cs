using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Whistle.Conditions;
using Whistle.Characters;

[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour, ICharacter, IConditions, IHealth {

    private string charName = "Ichabod";

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

    public string Name {
        get {
            return charName;
        }
        set {
            charName = value;
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
            throw new System.NotImplementedException();
        }

        set {
            throw new System.NotImplementedException();
        }
    }

    void Start () {
        trans = GetComponent<Transform>();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<BoxCollider2D>();
        Controller = GetComponent<CharController>();

        state = PlayerState.Walking;
    }

    void Update() {
        if (Controller.isTouchingGround && Input.GetKeyDown(KeyCode.Space)) {
            Controller.ApplyJump(JumpHeight);
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
        

        TickConds();
    }

    public Cond GetCond(string name) {
        throw new System.NotImplementedException();
    }

    public void AddCond(Cond cond) {
        cond.obj = gameObject;
        bool duplicateFound = false;
        for (int i = 0; i < condsApplied.Length; i++) {
            if (condsApplied[i] != null && condsApplied[i].name == cond.name && cond.overwriteable == true) {
                Debug.Log(condsApplied[i].name + " was reapplied to " + Name + "!");
                condsApplied[i].OverwriteEffect(cond);
                duplicateFound = true;
                break;
            }
        }
        if (!duplicateFound) {
            for (int i = 0; i < condsApplied.Length; i++) {
                if (condsApplied[i] == null) {
                    condsApplied[i] = cond;
                    Debug.Log(condsApplied[i].name + " was applied to " + Name + "!");
                    condsApplied[i].ApplyInitialEffect();
                    break;
                }
            }
        }
    }

    public void RemoveCond(Cond cond) {
        for (int i = 0; i < condsApplied.Length; i++) {
            if (condsApplied[i] != null && condsApplied[i].name == cond.name) {
                Debug.Log(condsApplied[i].name + " has been removed from " + Name + ".");
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
                    Debug.Log(condsApplied[i].name + " has been removed from " + Name + " after running out of time.");
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
