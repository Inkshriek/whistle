using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Whistle.Actors;

[RequireComponent(typeof(ActorController))]
public abstract class MonsterBase : MonoBehaviour, IActor, IHealth {


    public string DisplayName { get; set; }
    public ActorController Controller { get; protected set; }
    public bool Active { get; set; }

    [SerializeField] protected float _health;
    public float Health {
        get {
            return _health;
        }
        set {
            _health = Mathf.Clamp(value, 0, MaxHealth);
        }
    }

    [SerializeField] protected float _maxHealth;
    public float MaxHealth {
        get {
            return _maxHealth;
        }
        set {
            _maxHealth = Mathf.Max(0, value);
        }
    }

    [SerializeField] protected float _stunTime;

    public enum MonsterState {
        //A set of states that monsters use.
        Patrolling,
        Alert,
        Chasing,
        Enraged
    }

    private MonsterState _status;
    public MonsterState State {
        get {
            return _status;
        }
        set {
            _status = value;
            OnStateChange(value);
        }
    }

    //A monster is any type of regular enemy that Ichabod faces.
    //Monster AI functions off a system of states and events, and scripts that inherit this must control these states and define their functionality.

    //In general a monster patrols, seeks out the player, and chases them down when entering their line of sight. After losing sight for long enough, they'll cease and return to patrolling.
    //Monsters can also "hear" the player. If they hear a distinct sound, they should attempt to travel to the location of the noise.
    //The means to how a monster "finds" a player will often vary of course, as well as how they chase them and attack. Some may even be blind or deaf. Though this is how they ought to function as a baseline.
    //All monsters in the game cannot die. Instead, when reaching zero health, they should fall into an "enrage" status, which will make them invincible and extremely fast until the player escapes them. 
    //A monster can also be "stunned". The method to which a monster is stunned varies considerably, but inducing it is the main way Ichabod defends himself. Stunning will almost always inflict damage.

    //This class serves as the base for all monsters in the game. Be sure to implement it for each one.

    // Update is called once per frame
    private void Update () {
        if (_stunTime > 0 && !_acting && Active) {
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
                    break;
            }
        }

        _stunTime = Mathf.Max(_stunTime - Time.deltaTime, 0);
	}

    protected abstract void Patrolling();
    protected abstract void Alert();
    protected abstract void Chasing();
    protected abstract void Enraged();

    protected abstract void OnStateChange(MonsterState state);

    //Actions are here as a way of temporarily stopping the monster's behavior while they do something, like attack or climb something. Use all this to make it easy to do.
    public delegate IEnumerator Action();
    private bool _acting;
    private IEnumerator coroutine;
    public void StartAction(Action method) {
        StopCoroutine(coroutine);
        _acting = true;
        coroutine = method();
        StartCoroutine(coroutine);
    }
    public void EndAction() {
        StopCoroutine(coroutine);
        _acting = false;
    }
    //If you're starting an action, always be sure to call EndAction() at the end of the IEnumerator, so that _acting will be set to false and the monster can behave as normal afterwards.

    public virtual void Damage(float value, DamageType type) {
        Health -= value;
    }
}
