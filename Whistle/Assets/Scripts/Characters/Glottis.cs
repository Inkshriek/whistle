using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Whistle.Characters;

[RequireComponent(typeof(CharController))]
public class Glottis : MonoBehaviour, ICharacter, IBehavior {

    public string DisplayName { get; set; }
    public CharController Controller { get; set; }
    public CharacterMode Mode { get; set; }
    private Behavior currentBehavior;


    void Start () {
        AIAgent AI = new AIAgent(NavMesh.SceneNavMesh);

        Controller = GetComponent<CharController>();
    }
	
	void Update () {
        currentBehavior();
	}


    public void ApplyBehavior(Behavior behavior) {
        currentBehavior = behavior;
    }

    public void ResetBehavior() {
        currentBehavior = Behavior;
    }


    // Put in Glottis' behavior here. This happens once per frame.
    public void Behavior() {

    }
}
