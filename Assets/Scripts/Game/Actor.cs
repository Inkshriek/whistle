using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActorMethods : MonoBehaviour {
    //This is a set of helpful methods to help with managing actor behavior. No implementations are required.

    //This here allows you to temporarily make the actor "act" something out over a duration. Handy and automatic.
    public bool Busy { get; private set; }
    private Coroutine coroutine;
    public void StartBehavior(Whistle.Actors.Behavior method) {
        if (coroutine != null) {
            StopCoroutine(coroutine);
        }
        Busy = true;
        coroutine = StartCoroutine(method());
    }
    public void EndBehavior() {
        StopCoroutine(coroutine);
        Busy = false;
    }
    //If you're using this though, ALWAYS call EndBehavior to set Busy back to false at the end of a coroutine. That's what isn't automatic, so be sure to.
}

namespace Whistle.Actors {

    public interface IActor {
        //The most basic interface meant for all actors (characters) to use.
        string DisplayName { get; }
        ActorController Controller { get; }
        bool Active { get; set; }
    }

    public interface IHealth {
        //This interface works with "health" values on a basic level.
        float Health { get; set; }
        float MaxHealth { get; set; }
        void Damage(float value, DamageType type);
    }

    public interface IUseable {
        //This interface works with anything that is intended to be "useable" by the player, as in triggering something if the player presses a key while overlapping its collider.
        bool Useable { get; set; }
        void OnUse();
    }

    public enum MovementType {
        //A set of movement types used for defining what an actor is capable of doing. Meant for pathfinding and defining ActorController behavior.
        Normal,
        Flying
    }

    public enum DamageType {
        //A set of damage types. Used for determining how an actor reacts to certain forms of damage.
        Normal,
        Fire,
        Poison
    }

    public delegate IEnumerator Behavior();

}

