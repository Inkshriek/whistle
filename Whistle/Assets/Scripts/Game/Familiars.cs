using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Whistle.Characters;
using Whistle.Conditions;

namespace Whistle.Familiars {

    public abstract class Familiar : MonoBehaviour, ICharacter, IConditions {
        public abstract string Name { get; set; }
        public abstract CharController Controller { get; set; }
        public abstract CharacterMode Mode { get; set; }

        public abstract FamiliarAIType AI { get; set; }
        public abstract Player Player { get; set; }
        public abstract float Speed { get; set; }
        public abstract bool Active { get; set; }

        protected virtual void Update() {
            if (!Active) {

            }

            if (Active) {
                if (Input.GetAxisRaw("Familiar Ability") > 0) {
                    PrimaryAction();
                }
            }
        }

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

    public enum FamiliarAIType {
        Normal,
        Flying
    }

}
