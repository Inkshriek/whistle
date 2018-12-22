using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Whistle.Characters;
using Whistle.Conditions;

namespace Whistle.Familiars {

    public abstract class Familiar : MonoBehaviour, ICharacter, IBehavior, IConditions {
        public string DisplayName { get; set; }

        public abstract CharController Controller { get; set; }
        public abstract CharacterMode Mode { get; set; }

        protected Behavior currentBehavior;
        protected AIAgent AI;
        public abstract Player Player { get; set; }
        public abstract float Speed { get; set; }
        public abstract bool Active { get; set; }

        protected virtual void Update() {
            if (!Active) {

            }

            if (Active) {
                currentBehavior();

                if (Input.GetAxisRaw("Familiar Ability") > 0) {
                    PrimaryAction();
                }
            }
        }

        public abstract void ApplyBehavior(Behavior behavior);
        public abstract void ResetBehavior();

        protected abstract void PrimaryAction();

        protected Cond condApplied;

        public Cond GetCond(string name) {
            throw new System.NotImplementedException();
        }

        public void AddCond(Cond cond) {
            cond.obj = gameObject;
            if (condApplied.name != cond.name) {
                condApplied = cond;
                condApplied.ApplyInitialEffect();
            }
        }

        public void RemoveCond(Cond cond) {
            if (condApplied.name != cond.name) {
                condApplied.RemoveEffect();
                condApplied = null;
            }
        }

        protected void TickCond() {
            if (condApplied != null) {
                condApplied.time -= Time.deltaTime;
                if (condApplied.time <= 0) {
                    condApplied.RemoveEffect();
                    condApplied = null;
                }
            }
        }
    }

    public class FamiliarAI {
        
    }
}
