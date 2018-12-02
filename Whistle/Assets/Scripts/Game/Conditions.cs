using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Whistle.Characters;

namespace Whistle.Conditions {

    public abstract class Cond {
        public string name;
        public string description;
        public float time;

        public Cond(string name, string description, float time) {
            this.name = name;
            this.description = description;
            this.time = time;
        }

        public abstract void ApplyInitialEffect(GameObject obj);

        public abstract void ApplyContinuousEffect(GameObject obj);

        public abstract void RemoveEffect(GameObject obj);
    }

    public class SpeedUp : Cond {

        public SpeedUp(float time) : base("Speed Up!", "Gotta go fast.", time) {
        }

        public override void ApplyInitialEffect(GameObject obj) {
            ICharacter character = obj.GetComponent<ICharacter>();
            character.Speed *= 2;
        }

        public override void ApplyContinuousEffect(GameObject obj) {

        }

        public override void RemoveEffect(GameObject obj) {
            ICharacter character = obj.GetComponent<ICharacter>();
            character.Speed /= 2;
        }
    }

    public class Poison : Cond {

        public Poison(float time) : base("Poison", "Oh fuck.", time) {
        }

        public override void ApplyInitialEffect(GameObject obj) {


        }

        public override void ApplyContinuousEffect(GameObject obj) {
            ICharacter character = obj.GetComponent<ICharacter>();
            if (character.GetType() == typeof(PlayerController)) {

                (character as PlayerController).CurrentHealth -= 2 * Time.deltaTime;
            }
            else {
                Debug.LogError(character.Name + " is not a valid recipient for " + this.name);
            }
        }

        public override void RemoveEffect(GameObject obj) {


        }
    }
}