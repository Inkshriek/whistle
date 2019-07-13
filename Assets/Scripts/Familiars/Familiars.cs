using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Whistle.Actors;
using Whistle.Conditions;

namespace Whistle.Familiars {

    public abstract class Familiar : MonoBehaviour, IActor, IConditions {
        public string DisplayName { get; set; }

        public ActorController Controller { get; set; }
        public bool Active { get; set; }

        protected NavAgent AI { get; set; }
        public Player Player { get; set; }
        public float Speed { get; set; }

        protected void Update() {
            if (!Active) {

            }

            if (Active) {
                Behavior();

                if (Input.GetAxisRaw("Familiar Ability") > 0) {
                    PrimaryAction();
                }
            }
        }

        protected abstract void Behavior();
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

        public void RemoveCond(string name) {
            if (condApplied.name == name) {
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
