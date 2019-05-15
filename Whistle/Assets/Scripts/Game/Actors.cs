using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Whistle.Actors {

    public interface IActor {
        //The most basic interface meant for all actors (characters) to use.
        string DisplayName { get; }
        ActorController Controller { get; }
        ActorMode Mode { get; set; }
    }

    public interface IHealth {
        //This interface works with "health" values on a basic level. Probably best used with characters the player can damage.
        float Health { get; set; }
        float MaxHealth { get; set; }
        void Damage(float value);
        void Heal(float value);
    }

    public interface IUseable {
        //This interface works with anything that is intended to be "useable" by the player, as in triggering something if the player presses a key while overlapping its collider.
        bool Useable { get; set; }
        void OnUse();
    }

    public enum PlayerAction {
        //This is just a set of states intended exclusively for the player.
        Crouching,
        Walking,
        Running,
        Pushing,
        LedgeGrabbed
    }

    public enum ActorMode {
        //A set of modes intended to be used for switching a character's default behavior on and off.
        Inactive,
        Active
    }

    public enum MovementType {
        //A set of movement types used for defining what a character is capable of doing. Meant for pathfinding and defining CharController behavior.
        Normal,  
        Flying
    }
}
