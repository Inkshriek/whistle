using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Whistle.Conditions;
using Whistle.Characters;

[RequireComponent(typeof(CharController))]
public class Player : MonoBehaviour, ICharacter, IConditions, IHealth {

    public string DisplayName { get; set; }

    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;
    [SerializeField] private float crouchSpeed;
    [SerializeField] private float jumpHeight;
    [SerializeField] private float maxHealth;
    [SerializeField] private float effectiveHealth;
    [SerializeField] private float currentHealth;

    private Rigidbody2D rb;

    private List<Cond> conds;
    private bool jumping;

    public CharController Controller { get; private set; }

    public CharacterMode Mode { get; set; }
    public PlayerState State { get; set; }

    public float Speed {
        get {
            switch (State) {
                case PlayerState.Walking:
                    return Modifier.AdjustNumber(conds, walkSpeed, Modifier.Tag.Speed);

                case PlayerState.Crouching:
                    return Modifier.AdjustNumber(conds, crouchSpeed, Modifier.Tag.Speed);

                case PlayerState.Running:
                    return Modifier.AdjustNumber(conds, runSpeed, Modifier.Tag.Speed);

                default:
                    Debug.LogError("PlayerState is in an impossible position! May want to fix that quick.");
                    return 0;
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
            return effectiveHealth;
        }
        set {
            effectiveHealth = Mathf.Max(0, value);
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

    void Awake() {
        DisplayName = "Ichabod";
        conds = new List<Cond>();

        rb = GetComponent<Rigidbody2D>();
        Controller = GetComponent<CharController>();

        State = PlayerState.Walking;
        jumping = false;
    }

    void Update() {
        if (Mode == CharacterMode.Active) {
            if (Controller.isTouchingGround && Input.GetKeyDown(GameController.jumpKey)) {
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

            if (Controller.isTouchingGround) {
                Controller.Motion = new Vector2(Input.GetAxisRaw("Horizontal") * Speed, 0);
            }
            else {
                Controller.Motion = new Vector2(Input.GetAxisRaw("Horizontal") * walkSpeed, 0);
            }

            if (jumping && (!Input.GetKey(GameController.jumpKey) || rb.velocity.y < 0)) {
                rb.gravityScale *= 1.5f;
                jumping = false;
            }

            TickConds();
        }
    }

    public void TakeDamage(float value) {
        value = Modifier.AdjustNumber(conds, value, Modifier.Tag.Damage);
        CurrentHealth = Mathf.Max(CurrentHealth - value, 0);
    }

    public void HealDamage(float value) {
        value = Modifier.AdjustNumber(conds, value, Modifier.Tag.Damage);
        CurrentHealth = Mathf.Max(CurrentHealth - value, 0);
    }

    private void BleedHealth() {
        //This whole method here intends to "adjust" the effective health to, over time, reach the real current health.


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
