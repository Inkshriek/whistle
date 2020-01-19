using System.Collections.Generic;
using UnityEngine;
using Whistle.Actors;

namespace Whistle.Conditions {

    public class Conditions : MonoBehaviour {
        //The Conditions class is a container of various modular effects and status intended to be read from and controlled by another class.
        //They essentially "modify" other classes, in a way, when used properly. There could be a condition that slows a character's movement speed, for example.
        //It's important to know they don't modify other classes by themselves. The class using these must apply them.

        private void Awake() {
            
        }

        private void Update() {
            
        }
    }

    public interface IConditions {
        //This interface works with conditions. For classes that use them (especially characters), you should probably implement this.

        Cond GetCond(string name);
        void AddCond(Cond cond);
        void RemoveCond(string name);
    }

    public abstract class Cond {
        public GameObject obj; //The GameObject this condition is supposed to be "attached" to, in a sense.

        public string name; //The name of the condition. Used for identification.
        public string description; //The description of the condition.
        public float time; //The time left on the condition. Used and ticked by controllers automatically.
        public bool overwriteable; //Do we want this condition to be overwriteable or not? Used by controllers to determine if they should overwrite preexisting conds in their list or always add new ones.
        public List<Modifier> mods = new List<Modifier>();

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

        public abstract void OverwriteEffect(Cond cond); //This is what the condition does when another condition intends to "overwrite" it, such as with the time left.

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
         * From there, you can make all the changes you want through the condition, whether it be at first, or every frame. Just remember to undo unwanted permanent changes using "RemoveEffect".
         *
         *
         *
         * - Controllers
         *
         * If you want to make new classes that implement IConditions, you will have to work out how to manage the class's list of conditions and remember to call the condition's methods appropriately.
         * Rules of Thumb:
         *      You should always remember to assign the GameObject to the condition's "obj" somewhere.
         *      When you have your "AddCond" called, call the "ApplyInitialEffect" method of the condition, and then add it to a list.
         *      Or, if there is another condition of that kind already in the list and the condition is overwriteable, instead just simply call "OverwriteEffect" from the original with the new one as the parameter.
         *      Then, every frame where appropriate, tick down the condition's time left relative to Time.deltaTime and call its "ApplyContinuousEffect".
         *      In general, if the condition's time has reached 0 or less, you should remove it with "RemoveCond".
         *      Whenever "RemoveCond" is called, you'll want to call the condition's "RemoveEffect" method, then delete it from your list.
         *
         *
         *
         * Whoever is reading this, I hope you like this, cuz I SPENT TIME ON IT.
         */
    }

    public struct Modifier {
        //Modifiers exist as additional elements to a condition. They're designed for when you want to change a particular aspect of something nondestructively.
        //Properties should make use of modifiers in their return value for this purpose, read from the class's condition list.
        //You should **ALWAYS** use them for numeric or boolean values, since conditions can stack, and the value can incidentally end up permanently changed.

        public Tag tag;
        public float mult;
        public float flat;
        public bool enable;

        //Type 1 - Numeric
        public Modifier(Tag tag, float mult, float flat) {
            this.tag = tag;
            this.mult = mult;
            this.flat = flat;
            enable = true;
        }

        //Type 2 - Boolean
        public Modifier(Tag tag, bool enable) {
            this.tag = tag;
            mult = 1;
            flat = 0;
            this.enable = enable;
        }

        //Type 3 - Mixed
        public Modifier(Tag tag, float mult, float flat, bool enable) {
            this.tag = tag;
            this.mult = mult;
            this.flat = flat;
            this.enable = enable;
        }

        public static float AdjustNumber(List<Cond> conditions, float value, Tag tag) {
            //This returns an adjusted number according to the modifiers of the ID, adding first and then multiplying.

            float result = value;
            float flat = 0;
            float mult = 1;

            for (int c = 0; c < conditions.Count; c++) {
                if (conditions[c] != null) {
                    for (int i = 0; i < conditions[c].mods.Count; i++) {
                        if (conditions[c].mods[i].tag == tag) {
                            flat += conditions[c].mods[i].flat;
                            mult *= conditions[c].mods[i].mult;
                        }
                    }
                }
            }

            result += flat;
            result *= mult;

            return result;
        }

        public static float AdjustNumber(Cond condition, float value, Tag tag) {
            //Same as above, though taking only one condition.

            float result = value;
            float flat = 0;
            float mult = 1;

            if (condition != null) {
                for (int i = 0; i < condition.mods.Count; i++) {
                    if (condition.mods[i].tag == tag) {
                        flat += condition.mods[i].flat;
                        mult *= condition.mods[i].mult;
                    }
                }
            }

            result += flat;
            result *= mult;

            return result;
        }

        public static bool AdjustBool(List<Cond> conditions, bool value, bool preference, Tag tag) {
            //This returns an adjusted bool according to the modifiers of the ID, preferring whatever is set to the "preference".

            bool result = value;

            for (int c = 0; c < conditions.Count; c++) {
                if (conditions[c] != null) {
                    for (int i = 0; i < conditions[c].mods.Count; i++) {
                        if (conditions[c].mods[i].tag == tag) {
                            if (conditions[c].mods[i].enable == preference) {
                                return preference;
                            }
                            else {
                                result = conditions[c].mods[i].enable;
                            }
                        }
                    }
                }
            }

            return result;
        }

        public static bool AdjustBool(Cond condition, bool value, bool preference, Tag tag) {
            //Yeah.

            bool result = value;

            if (condition != null) {
                for (int i = 0; i < condition.mods.Count; i++) {
                    if (condition.mods[i].tag == tag) {
                        if (condition.mods[i].enable == preference) {
                            return preference;
                        }
                        else {
                            result = condition.mods[i].enable;
                        }
                    }
                }
            }

            return result;
        }

        public enum Tag {
            //A set of tags intended for properties to access modifiers with. Mess with them as you see fit.
            Speed,
            JumpHeight,
            MaxHealth,
            Damage,
            Healing,
            PlayerBleed,
            Gravity
        }
    }

    public class SpeedUp : Cond {

        public SpeedUp(float time) : base("Speed Up!", "Gotta go fast.", time) { }

        public override void ApplyInitialEffect() {
            Modifier speedup = new Modifier(Modifier.Tag.Speed, 2, 0);
            mods.Add(speedup);
        }

        public override void ApplyContinuousEffect() {

        }

        public override void OverwriteEffect(Cond cond) {
            time = Mathf.Max(time, cond.time);
        }

        public override void RemoveEffect() {

        }

    }

    public class Poison : Cond {

        public Poison(float time) : base("Poison", "Oh fuck.", time) { }

        public override void ApplyInitialEffect() {

					
        }

        public override void ApplyContinuousEffect() {
            IActor character = obj.GetComponent<IActor>();
            if (character.GetType() == typeof(Player)) {

                (character as Player).Health -= 2 * Time.deltaTime;
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
