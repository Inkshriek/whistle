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

    public interface IBehavior {
        //This interface works with behavior and behavior management. Goes nice with AI.
        void ApplyBehavior(Behavior newBehavior);
        void ResetBehavior();
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
        Active
    }

    public enum MovementType {
        //A set of modes intended to be used for managing characters.
        Normal,  
        Flying
    }

    public delegate void Behavior();
}
