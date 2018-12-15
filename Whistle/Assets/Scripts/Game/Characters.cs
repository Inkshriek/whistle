using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Whistle.Conditions;

namespace Whistle.Characters {

    public interface ICharacter {
        //The most basic interface meant for all characters to use.
        string Name { get; set; }
        float Speed { get; set; }
        float JumpHeight { get; set; }
        CharacterMode Mode { get; set; }
    }

    public interface IHealth {
        //This interface works with "health" values on a basic level.
        float CurrentHealth { get; set; }
        float MaxHealth { get; set; }
    }

    public enum PlayerState {
        //This is just a set of states for the player at any given time.
        Crouching,
        Walking,
        Running
    }

    public enum CharacterMode {
        //A set of modes intended to be used for managing characters.
        Inactive,
        Active,
        Cutscene
    }

    public abstract class Player : MonoBehaviour, ICharacter, IConditions, IHealth {

        private string charName = "Ichabod";

        [SerializeField] private float walkSpeed;
        [SerializeField] private float runSpeed;
        [SerializeField] private float crouchSpeed;
        [SerializeField] private float jumpHeight;
        [SerializeField] private float aerialControl;
        [SerializeField] [Range(0, 90)] private float slopeTolerance;
        [SerializeField] private float maxHealth;
        [SerializeField] private float effectiveHealth;
        [SerializeField] private float currentHealth;
        [SerializeField] private CharacterMode charMode;

        [HideInInspector] public float moveDirection;
        [HideInInspector] public bool isTouchingGround;
        [HideInInspector] public PlayerState state;
        [HideInInspector] public bool allowCrouch;
        [HideInInspector] public bool allowJump;
        [HideInInspector] public bool allowMidairJump;

        [HideInInspector] public float groundH;
        [HideInInspector] public float groundV;

        [HideInInspector] public float airHInitial;
        [HideInInspector] public float airH;
        [HideInInspector] public float airV;

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

        public float SlopeTolerance {
            get {
                return slopeTolerance;
            }
            set {
                slopeTolerance = Mathf.Clamp(value, 0f, 90f);
            }
        }

        public float AerialControl {
            get {
                return aerialControl;
            }
            set {
                aerialControl = value;
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

        public CharacterMode Mode {
            get {
                return charMode;
            }
            set {
                charMode = value;
            }
        }

        private Cond[] condsApplied = new Cond[12];

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
}
