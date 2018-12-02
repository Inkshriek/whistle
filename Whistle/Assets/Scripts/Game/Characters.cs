using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Whistle.Conditions;

namespace Whistle.Characters {

    public interface ICharacter {
        string Name { get; set; }
        float Speed { get; set; }
        float JumpHeight { get; set; }
    }

    public interface INPC {

    }

    public interface IConditions {
        Cond GetCond(string name);
        void AddCond(Cond cond);
        void RemoveCond(Cond cond);
    }

    public interface IHealth {
        float CurrentHealth { get; set; }
        float MaxHealth { get; set; }
    }

    public abstract class Player : MonoBehaviour, IConditions {
        private Cond[] condsApplied = new Cond[12];

        public Cond GetCond(string name) {
            throw new System.NotImplementedException();
        }
        
        public void AddCond(Cond cond) {
            bool duplicateFound = false;
            for (int i = 0; i < condsApplied.Length; i++) {
                if (condsApplied[i] != null && condsApplied[i].name == cond.name) {
                    condsApplied[i].time = cond.time;
                    duplicateFound = true;
                    break;
                }
            }
            if (!duplicateFound) {
                for (int i = 0; i < condsApplied.Length; i++) {
                    if (condsApplied[i] == null) {
                        condsApplied[i] = cond;
                        Debug.Log(condsApplied[i].name + " was applied to " + name + "!");
                        condsApplied[i].ApplyInitialEffect(gameObject);
                        break;
                    }
                }
            }
        }

        public void RemoveCond(Cond cond) {
            for (int i = 0; i < condsApplied.Length; i++) {
                if (condsApplied[i] != null && condsApplied[i].name == cond.name) {
                    Debug.Log(condsApplied[i].name + " has been removed from " + name + ".");
                    condsApplied[i].RemoveEffect(gameObject);
                    condsApplied[i] = null;
                    RealignCondList();
                    break;
                }
            }
        }

        protected void TickConds() {
            for (int i = 0; i < condsApplied.Length; i++) {
                if (condsApplied[i] != null) {
                    condsApplied[i].time -= Time.deltaTime;
                    if (condsApplied[i].time <= 0) {
                        Debug.Log(condsApplied[i].name + " has been removed from " + name + " after running out of time.");
                        condsApplied[i].RemoveEffect(gameObject);
                        condsApplied[i] = null;
                        RealignCondList();
                    }
                    else {
                        condsApplied[i].ApplyContinuousEffect(gameObject);
                    }
                }
            }
        }

        protected void RealignCondList() {
            int arrayShift = 0;
            for (int i = 0; i < condsApplied.Length; i++) {
                if (condsApplied[i] == null) {
                    arrayShift += 1;
                }
                if (i + arrayShift > condsApplied.Length - 1) {
                    condsApplied[i] = null;
                }
                else {
                    condsApplied[i] = condsApplied[i + arrayShift];
                }
            }
        }
    }
}
