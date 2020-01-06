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
        public Player Player { get; set; }

        protected void FixedUpdate() {
            if (!Active) {

            }
            else {
                Behavior();

                if (Input.GetAxisRaw("Familiar Ability") > 0) {
                    FamiliarAbility();
                }
            }
        }

        protected abstract void Behavior();
        protected abstract void FamiliarAbility();

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
