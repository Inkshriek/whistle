using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Whistle.Actors;

[RequireComponent(typeof(ActorController))]
public class Glottis : MonoBehaviour, IActor {

    public string DisplayName { get; set; }
    public ActorController Controller { get; set; }
    public ActorMode Mode { get; set; }


    void Start () {

        Controller = GetComponent<ActorController>();
    }
	
	void Update () {
        // Put in Glottis' behavior here. This happens once per frame.
    }
}
