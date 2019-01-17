using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Whistle.Characters;

[RequireComponent(typeof(CharController))]
public class Glottis : MonoBehaviour, ICharacter {

    public string DisplayName { get; set; }
    public CharController Controller { get; set; }
    public CharacterMode Mode { get; set; }


    void Start () {

        Controller = GetComponent<CharController>();
    }
	
	void Update () {
        // Put in Glottis' behavior here. This happens once per frame.
    }
}
