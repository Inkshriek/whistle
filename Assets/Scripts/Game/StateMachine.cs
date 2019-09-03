using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine {
    /*
    public List<State> StateSet { get; private set; }
    public int ActiveState { get; private set; }
    private Transition _stateChange;
    public Transition OnStateChange {
        set {
            _stateChange = value;
        }
    }

    public StateMachine() {
        StateSet = new List<State>();
        ActiveState = 0;
    }

    public State Current {
        get {
            return StateSet[ActiveState];
        }
    }

    public void Add(string name, Behavior behavior) {
        if (name == null || behavior == null) {
            return;
        }
        State namecheck = SearchByName(name);
        if (namecheck.Name != name) {
            StateSet.Add(new State {
                Name = name,
                Behavior = behavior
            });
        }
    }

    public void Delete(string name) {
        if (name == null) {
            return;
        }
        StateSet.Remove(SearchByName(name));
    }

    public void SwitchTo(string name) {
        if (name == null || _stateChange == null) {
            return;
        }
        _stateChange(StateSet[ActiveState].Name);
        ActiveState = StateSet.IndexOf(SearchByName(name));
    }

    private State SearchByName(string name) {
        if (StateSet.Count > 0) {
            foreach (State state in StateSet) {
                if (state.Name.ToLower() == name.ToLower()) {
                    return state;
                }
            }
        }
        return new State {
            Name = null,
            Behavior = null
        };
    }

    public struct State {
        public string Name;
        public Behavior Behavior;
    }

    public delegate int Behavior();
    public delegate void Transition(string previous);
    */
}
