using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Whistle.Characters {

    public interface ICharacter {
        //The most basic interface meant for all characters to use.
        string DisplayName { get; }
        CharController Controller { get; }
        CharacterMode Mode { get; set; }
    }

    public interface IHealth {
        //This interface works with "health" values on a basic level. Probably best used with characters the player can damage.
        float CurrentHealth { get; set; }
        float MaxHealth { get; set; }
        void TakeDamage(float value);
        void HealDamage(float value);
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
        Running
    }

    public enum CharacterMode {
        //A set of modes intended to be used for switching a character's default behavior on and off.
        Inactive,
        Active
    }

    public enum MovementType {
        //A set of movement types used for defining what a character is capable of doing. Meant for pathfinding.
        Normal,  
        Flying
    }
}
