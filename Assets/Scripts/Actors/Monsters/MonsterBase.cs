using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using Whistle.Actors;

[RequireComponent(typeof(ActorController))]
[RequireComponent(typeof(ActorSenses))]
public abstract class MonsterBase : ActorMethods, IActor, IHealth {

    public string DisplayName { get; set; }
    public ActorController Controller { get; protected set; }
    public bool Active { get; set; }

    [SerializeField] protected float health;
    public float Health {
        get {
            return health;
        }
        set {
            health = Mathf.Clamp(value, 0, MaxHealth);
        }
    }

    [SerializeField] protected float maxHealth;
    public float MaxHealth {
        get {
            return maxHealth;
        }
        set {
            maxHealth = Mathf.Max(0, value);
        }
    }

    protected enum MonsterState {
        Patrolling,
        Alert,
        Chasing,
        Enraged
    }

    protected NavAgent AI { get; set; }
    protected MonsterState State { get; set; }
    public ActorSenses Senses { get; protected set; }

    [SerializeField] protected float stunDuration;
    [SerializeField] protected SkeletonAnimation anim;

    //A monster is any type of regular enemy that Ichabod faces.
    //Monster AI functions off a system of states and events, and scripts that inherit this must control these states and define their functionality.

    //In general a monster patrols, seeks out the player, and chases them down when entering their line of sight. After losing sight for long enough, they'll cease and return to patrolling.
    //Monsters can also "hear" the player. If they hear a distinct sound, they should attempt to travel to the location of the noise.
    //The means to how a monster "finds" a player will often vary of course, as well as how they chase them and attack. Some may even be blind or deaf. Though this is how they ought to function as a baseline.
    //All monsters in the game cannot die. Instead, when reaching zero health, they should fall into an "enrage" status, which will make them invincible and extremely fast until the player escapes them. 
    //A monster can also be "stunned". The method to which a monster is stunned varies considerably, but inducing it is the main way Ichabod defends himself. Stunning will almost always inflict damage.

    //This class serves as the base for all monsters in the game. Be sure to implement it for each one.

    protected void BaseInitialize() {
        //Call this method in Start() or Awake() at the top.
        Controller = GetComponent<ActorController>();
        Senses = GetComponent<ActorSenses>();
        State = MonsterState.Patrolling;
    }

    protected void BaseUpdate() {
        //Call this method in Update() or FixedUpdate() at the top.
        if (stunDuration <= 0 && !Busy && Active) {
            switch (State) {
                case MonsterState.Patrolling:
                    Patrolling();
                    break;
                case MonsterState.Alert:
                    Alert();
                    break;
                case MonsterState.Chasing:
                    Chasing();
                    break;
                case MonsterState.Enraged:
                    Enraged();
                    break;
                default:
                    Debug.Log(DisplayName + " is in an invalid state! Might wanna fix that.");
                    break;
            }
        }

        stunDuration = Mathf.Max(stunDuration - Time.deltaTime, 0);
	}

    protected abstract void Patrolling();
    protected abstract void Alert();
    protected abstract void Chasing();
    protected abstract void Enraged();

    public virtual void Damage(float value, DamageType type) {
        Health -= value;
    }
}
