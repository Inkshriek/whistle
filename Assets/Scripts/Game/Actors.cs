using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Whistle.Actors {

    public interface IActor {
        //The most basic interface meant for all actors (characters) to use.
        string DisplayName { get; }
        ActorController Controller { get; }
        bool Active { get; set; }
    }

    public interface IHealth {
        //This interface works with "health" values on a basic level. Probably best used with characters the player can damage.
        float Health { get; set; }
        float MaxHealth { get; set; }
        void Damage(float value, DamageType type);
    }

    public interface IUseable {
        //This interface works with anything that is intended to be "useable" by the player, as in triggering something if the player presses a key while overlapping its collider.
        bool Useable { get; set; }
        void OnUse();
    }

    public enum PlayerState {
        //This is just a set of states intended exclusively for the player.
        Crouching,
        Walking,
        Running,
        Pushing,
        LedgeGrabbed
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
}
