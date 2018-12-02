using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Whistle.Characters;
using Whistle.Conditions;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class MonsterController : MonoBehaviour, ICharacter, IConditions, IHealth {
    public string Name {
        get {
            return "test monster";
        }

        set {
            throw new System.NotImplementedException();
        }
    }

    public float Speed {
        get {
            throw new System.NotImplementedException();
        }

        set {
            throw new System.NotImplementedException();
        }
    }

    public float JumpHeight {
        get {
            throw new System.NotImplementedException();
        }

        set {
            throw new System.NotImplementedException();
        }
    }

    public float CurrentHealth {
        get {
            throw new System.NotImplementedException();
        }

        set {
            throw new System.NotImplementedException();
        }
    }

    public float EffectiveHealth {
        get {
            throw new System.NotImplementedException();
        }

        set {
            throw new System.NotImplementedException();
        }
    }
    public float MaxHealth {
        get {
            throw new System.NotImplementedException();
        }

        set {
            throw new System.NotImplementedException();
        }
    }

    private Cond[] condsApplied = new Cond[12];
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        TickConds();
	}

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
                    Debug.Log(condsApplied[i].name + " was applied to " + Name + "!");
                    condsApplied[i].ApplyInitialEffect(gameObject);
                    break;
                }
            }
        }
    }

    public void RemoveCond(Cond cond) {
        for (int i = 0; i < condsApplied.Length; i++) {
            if (condsApplied[i] != null && condsApplied[i].name == cond.name) {
                Debug.Log(condsApplied[i].name + " has been removed from " + Name + ".");
                condsApplied[i].RemoveEffect(gameObject);
                condsApplied[i] = null;
                RealignCondList();
                break;
            }
        }
    }

    private void TickConds() {
        for (int i = 0; i < condsApplied.Length; i++) {
            if (condsApplied[i] != null) {
                condsApplied[i].time -= Time.deltaTime;
                if (condsApplied[i].time <= 0) {
                    Debug.Log(condsApplied[i].name + " has been removed from " + Name + " after running out of time.");
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


    private void RealignCondList() {
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
