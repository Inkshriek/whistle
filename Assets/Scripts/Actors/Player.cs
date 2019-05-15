using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Whistle.Conditions;
using Whistle.Actors;

[RequireComponent(typeof(ActorController))]
public class Player : MonoBehaviour, IActor, IConditions, IHealth {

    public string DisplayName { get; set; }

    [SerializeField] private float _walkSpeed;
    [SerializeField] private float _runSpeed;
    [SerializeField] private float _crouchSpeed;
    [SerializeField] private float _jumpHeight;
    [SerializeField] private float _health;
    [SerializeField] private float _healthDeficit;
    [SerializeField] private float _maxHealth;
    [SerializeField] private float _bleedRate;

    private Rigidbody2D rb;

    private List<Cond> conds;
    private bool jumping;

    public ActorController Controller { get; private set; }

    public ActorMode Mode { get; set; }
    public PlayerAction Action { get; set; }

    public float Speed {
        get {
            switch (Action) {

                case PlayerAction.Crouching:
                    return Modifier.AdjustNumber(conds, _crouchSpeed, Modifier.Tag.Speed);

                case PlayerAction.Running:
                    return Modifier.AdjustNumber(conds, _runSpeed, Modifier.Tag.Speed);

                default:
                    return Modifier.AdjustNumber(conds, _walkSpeed, Modifier.Tag.Speed);
            }
        }
        set {
            float multRun = _runSpeed / _walkSpeed;
            float multCrouch = _crouchSpeed / _walkSpeed;
            _walkSpeed = value;
            _runSpeed = value * multRun;
            _crouchSpeed = value * multCrouch;
        }
    }

    public float JumpHeight {
        get {
            return Modifier.AdjustNumber(conds, _jumpHeight, Modifier.Tag.JumpHeight);
        }
        set {
            _jumpHeight = value;
        }
    }

    public float Health {
        get {
            return _health;
        }
        set {
            _health = Mathf.Clamp(value, 0, MaxHealth);
        }
    }

    public float HealthDeficit {
        get {
            return _healthDeficit;
        }
        set {
            _healthDeficit = value;
        }
    }

    public float MaxHealth {
        get {
            return Modifier.AdjustNumber(conds, _maxHealth, Modifier.Tag.MaxHealth);
        }
        set {
            _maxHealth = Mathf.Max(0, value);
        }
    }

    public float BleedRate {
        get {
            return Modifier.AdjustNumber(conds, _bleedRate, Modifier.Tag.PlayerBleed);
        }
        set {
            _bleedRate = Mathf.Abs(value);
        }
    }

    void Awake() {
        DontDestroyOnLoad(this);

        DisplayName = "Ichabod";
        conds = new List<Cond>();

        rb = GetComponent<Rigidbody2D>();
        Controller = GetComponent<ActorController>();

        Mode = ActorMode.Active;
        Action = PlayerAction.Walking;
        jumping = false;
    }

    void Update() {
        if (Mode == ActorMode.Active) {
            if (Controller.isTouchingGround && Input.GetKeyDown(GameController.jumpKey)) {
                Controller.ApplyJump(JumpHeight);
                jumping = true;
                rb.gravityScale /= 1.5f;
            }

            if (Input.GetAxisRaw("Vertical") < 0) {
                Action = PlayerAction.Crouching;
            }
            else if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) {
                Action = PlayerAction.Running;
            }
            else {
                Action = PlayerAction.Walking;
            }

            if (Controller.isTouchingGround) {
                Controller.Motion = new Vector2(Input.GetAxisRaw("Horizontal") * Speed, 0);
            }
            else {
                Controller.Motion = new Vector2(Input.GetAxisRaw("Horizontal") * _walkSpeed, 0);
            }

            if (jumping && (!Input.GetKey(GameController.jumpKey) || rb.velocity.y < 0)) {
                rb.gravityScale *= 1.5f;
                jumping = false;
            }

            TickConds();
            BleedHealth();
        }
    }

    public void Damage(float value) {
        value = Modifier.AdjustNumber(conds, value, Modifier.Tag.Damage);
        HealthDeficit += value;
    }

    public void Heal(float value) {
        value = Modifier.AdjustNumber(conds, value, Modifier.Tag.Healing);
        HealthDeficit -= value;
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
            //It wants me to do this apparently
        }
        
        HealthDeficit -= adjustment;
        Health -= adjustment;
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
