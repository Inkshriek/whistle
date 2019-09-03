using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Whistle.Conditions;
using Spine.Unity;
using Whistle.Actors;

[RequireComponent(typeof(ActorController))]
public class Player : ActorMethods, IActor, IConditions, IHealth {

    public string DisplayName { get; set; }

    [SerializeField] private float _walkSpeed;
    [SerializeField] private float _runSpeed;
    [SerializeField] private float _crouchSpeed;
    [SerializeField] private float _jumpHeight;
    [SerializeField] private float _health;
    [SerializeField] private float _healthDeficit;
    [SerializeField] private float _maxHealth;
    [SerializeField] private float _bleedRate;
    [SerializeField] private SkeletonAnimation _animation;

    private Rigidbody2D _rb;

    private List<Cond> _conds;
    private bool _jumping;

    public ActorController Controller { get; private set; }

    public bool Active { get; set; }
    public PlayerState Action { get; set; }

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
            switch (Action) {

                case PlayerState.Crouching:
                    return Modifier.AdjustNumber(_conds, _crouchSpeed, Modifier.Tag.Speed);

                case PlayerState.Running:
                    return Modifier.AdjustNumber(_conds, _runSpeed, Modifier.Tag.Speed);

                default:
                    return Modifier.AdjustNumber(_conds, _walkSpeed, Modifier.Tag.Speed);
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
            return Modifier.AdjustNumber(_conds, _jumpHeight, Modifier.Tag.JumpHeight);
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
            return Modifier.AdjustNumber(_conds, _maxHealth, Modifier.Tag.MaxHealth);
        }
        set {
            _maxHealth = Mathf.Max(0, value);
        }
    }

    public float BleedRate {
        get {
            return Modifier.AdjustNumber(_conds, _bleedRate, Modifier.Tag.PlayerBleed);
        }
        set {
            _bleedRate = Mathf.Abs(value);
        }
    }

    private void Awake() {
        DontDestroyOnLoad(this);

        DisplayName = "Ichabod";
        _conds = new List<Cond>();

        _rb = GetComponent<Rigidbody2D>();
        Controller = GetComponent<ActorController>();

        Active = true;
        Action = PlayerState.Walking;
        _jumping = false;
    }

    private void Update() {
        if (Active) {
            if (Controller.IsTouchingGround && Input.GetKeyDown(GameController.jumpKey)) {
                Controller.ApplyJump(JumpHeight);
                _jumping = true;
                _rb.gravityScale /= 1.5f;
            }

            if (Input.GetAxisRaw("Vertical") < 0) {
                Action = PlayerState.Crouching;
            }
            else if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) {
                Action = PlayerState.Running;
            }
            else {
                Action = PlayerState.Walking;
            }

            if (Controller.IsTouchingGround) {
                Controller.Motion = new Vector2(Input.GetAxisRaw("Horizontal") * Speed, 0);
            }
            else {
                Controller.Motion = new Vector2(Input.GetAxisRaw("Horizontal") * _walkSpeed, 0);
            }

            if (_jumping && (!Input.GetKey(GameController.jumpKey) || _rb.velocity.y < 0)) {
                _rb.gravityScale *= 1.5f;
                _jumping = false;
            }

            TickConds();
            BleedHealth();
        }

        if (_jumping) {
            _animation.AnimationName = "jump";
        }
        else if (Controller.Motion.x == 0) {
            if (Action == PlayerState.Crouching) {
                _animation.AnimationName = "crouch idle";
            }
            else {
                _animation.AnimationName = "idle";
            }
        }
        else if (Action == PlayerState.Walking) {
            _animation.AnimationName = "walk";
        }
        else if (Action == PlayerState.Running) {
            _animation.AnimationName = "run";
        }
        else if (Action == PlayerState.Crouching) {
            _animation.AnimationName = "crouch walk";
        }

        Vector3 animScale = _animation.transform.localScale;

        if (Controller.Motion.x != 0) {
            _animation.transform.localScale = new Vector3(Mathf.Abs(animScale.x) * Mathf.Sign(Controller.Motion.x), animScale.y, animScale.z);
        }
    }

    public void Damage(float value, DamageType type) {
        value = Modifier.AdjustNumber(_conds, value, Modifier.Tag.Damage);
        HealthDeficit += value;
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

    public Cond GetCond(string name) {
        throw new System.NotImplementedException();
    }

    public void AddCond(Cond cond) {
        cond.obj = gameObject;
        bool duplicateFound = false;
        for (int i = 0; i < _conds.Count; i++) {
            if (_conds[i] != null && _conds[i].name == cond.name && cond.overwriteable == true) {
                Debug.Log(_conds[i].name + " was reapplied to " + DisplayName + "!");
                _conds[i].OverwriteEffect(cond);
                duplicateFound = true;
                break;
            }
        }
        if (!duplicateFound) {
            _conds.Add(cond);
            Debug.Log(cond.name + " was applied to " + name + "!");
            cond.ApplyInitialEffect();
        }
    }

    public void RemoveCond(string name) {
        for (int i = 0; i < _conds.Count; i++) {
            if (_conds[i] != null && _conds[i].name == name) {
                Debug.Log(_conds[i].name + " has been removed from " + DisplayName + ".");
                _conds[i].RemoveEffect();
                _conds[i] = null;
                break;
            }
        }
    }

    protected void TickConds() {
        for (int i = 0; i < _conds.Count; i++) {
            if (_conds[i] != null) {
                _conds[i].time -= Time.deltaTime;
                if (_conds[i].time <= 0) {
                    Debug.Log(_conds[i].name + " has been removed from " + DisplayName + " after running out of time.");
                    _conds[i].RemoveEffect();
                    _conds[i] = null;
                }
                else {
                    _conds[i].ApplyContinuousEffect();
                }
            }
        }
    }
}
