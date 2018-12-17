using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Whistle.Conditions;

namespace Whistle.Characters {

    public interface ICharacter {
        //The most basic interface meant for all characters to use.
        string Name { get; set; }
        CharController Controller { get; }
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

    public enum MovementType {
        //A set of modes intended to be used for managing characters.
        Normal,
        Flying,
        Swimming
    }
}
