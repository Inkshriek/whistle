using UnityEngine;
using Whistle.Characters;

namespace Whistle.Conditions {

    public interface IConditions {
        //This interface works with conditions. For classes that use them (especially characters), you should probably implement this.
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

        public abstract void OverwriteEffect(Cond cond); //This is what the condition does when another condition intends to "overwrite" it, such as with the time left. Just about always should be the same kind.

        public abstract void RemoveEffect(); //This is what the condition does when it's removed. Meant for undoing its own effects so the character reverts to normal.



        /* ~~~ Quick Guide to Using Conditions ~~~
         * 
         * 
         * 
         * - Instantiating
         * 
         * Instantiate new conditions using the constructor of the condition in question, while calling the "AddCond" method of the thing you're assigning it to. Only classes that implement the IConditions interface will have this method.
         * Example:
         *      player.AddCond(new Speed(30));
         * 
         * 
         * 
         * - Making New Ones
         * 
         * It's a simple task to make one! In "ApplyInitialEffect" for instance, you'll first want to grab a component for the condition to be making changes to.
         * Example:
         *      ICharacter character = obj.GetComponent<ICharacter>();
         * 
         * If you're finding an attached character script this way, you'll then also want to check the type to ensure you're getting the character you want.
         * Example:
         *      if (character.GetType() == typeof(Player)) {
         *          (character as Player).CurrentHealth -= 20;
         *      }
         * 
         * From there, you can make all the changes you want through the condition, whether it be at first, or every frame. Just remember to undo with "RemoveEffect" so it doesn't cause unwanted permanent changes.
         * 
         * 
         * 
         * - Controllers
         * 
         * If you want to make new classes that implement IConditions, you will have to work out how to manage the class's list of conditions and remember to call the condition's methods appropriately.
         * Rules of Thumb:
         *      You should always remember to assign the class's attached GameObject to the condition's "obj" somewhere.
         *      When you have your "AddCond" called, call the "ApplyInitialEffect" method of the condition, and then add it to a list. 
         *      Or, if there is another condition of that kind already in the list and the condition is overwriteable, instead just simply call "OverwriteEffect" from the original with the new one as the parameter.
         *      Then, every frame where appropriate, tick down the condition's time left relative to Time.deltaTime and call its "ApplyContinuousEffect".
         *      In general, if the condition's time has reached 0 or less, you should remove it with "RemoveCond". 
         *      Whenever "RemoveCond" is called, you'll want to call the condition's "RemoveEffect" method, then delete it from your list.
         * 
         * 
         * 
         * Whoever is reading this, I hope you like this, cuz I SPENT TIME ON THIS.
         */
    }

    public class SpeedUp : Cond {

        public SpeedUp(float time) : base("Speed Up!", "Gotta go fast.", time) {
        }

        public override void ApplyInitialEffect() {
            Player character = obj.GetComponent<Player>();
            character.Speed *= 2;
        }

        public override void ApplyContinuousEffect() {

        }

        public override void OverwriteEffect(Cond cond) {
            time = Mathf.Max(time, cond.time);
        }

        public override void RemoveEffect() {
            Player character = obj.GetComponent<Player>();
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
            if (character.GetType() == typeof(Player)) {

                (character as Player).CurrentHealth -= 2 * Time.deltaTime;
            }
            else {
                Debug.LogError(obj.name + " is not a valid recipient for " + this.name);
            }
        }


        public override void OverwriteEffect(Cond cond) {
            time = Mathf.Max(time, cond.time);
        }

        public override void RemoveEffect() {


        }

    }
}