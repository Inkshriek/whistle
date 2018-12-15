using UnityEngine;
using Whistle.Characters;

namespace Whistle.Conditions {

    public interface IConditions {
        //This interface works with conditions. For classes that use them (especially characters and the player), you should probably implement this.
        Cond GetCond(string name);
        void AddCond(Cond cond);
        void RemoveCond(Cond cond);
    }

    public abstract class Cond {
        public GameObject obj; //The GameObject this condition is supposed to be "attached" to, in a sense.

        public string name; //The name of the condition. Used for identification.
        public string description; //The description of the condition.
        public float time; //The time left on the condition. Used and ticked by controllers automatically.
        public bool overwriteable; //Do we want this condition to be overwriteable or not? Used by controllers to determine if they should overwrite preexisting conds in their list or always add new ones.

        public Cond(string name, string description, float time) {
            this.name = name;
            this.description = description;
            this.time = time;
            this.overwriteable = true;
        }

        public Cond(string name, string description, float time, bool overwriteable) {
            this.name = name;
            this.description = description;
            this.time = time;
            this.overwriteable = overwriteable;
        }

        public abstract void ApplyInitialEffect(); //The initial effect of the condition. This is what occurs when it is first applied.

        public abstract void ApplyContinuousEffect(); //The continuous effect of the condition. This is what occurs every "tick", which is generally per frame.

        public abstract void OverwriteEffect(Cond cond); //This is what the condition does when another condition (the same type, hopefully) intends to "overwrite" it.

        public abstract void RemoveEffect(); //This is what the condition does when it's removed. Meant for undoing its own effects so the character reverts to normal.
    }

    public class SpeedUp : Cond {

        public SpeedUp(float time) : base("Speed Up!", "Gotta go fast.", time) {
        }

        public override void ApplyInitialEffect() {
            ICharacter character = obj.GetComponent<ICharacter>();
            character.Speed *= 2;
        }

        public override void ApplyContinuousEffect() {

        }

        public override void OverwriteEffect(Cond cond) {
            time = Mathf.Max(time, cond.time);
        }

        public override void RemoveEffect() {
            ICharacter character = obj.GetComponent<ICharacter>();
            character.Speed /= 2;
        }

    }

    public class Poison : Cond {

        public Poison(float time) : base("Poison", "Oh fuck.", time) {
        }

        public override void ApplyInitialEffect() {


        }

        public override void ApplyContinuousEffect() {
            ICharacter character = obj.GetComponent<ICharacter>();
            if (character.GetType() == typeof(PlayerController)) {

                (character as PlayerController).CurrentHealth -= 2 * Time.deltaTime;
            }
            else {
                Debug.LogError(character.Name + " is not a valid recipient for " + this.name);
            }
        }


        public override void OverwriteEffect(Cond cond) {
            time = Mathf.Max(time, cond.time);
        }

        public override void RemoveEffect() {


        }

    }
}