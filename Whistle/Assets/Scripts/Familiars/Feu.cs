using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Whistle.Familiars;
using Whistle.Characters;

[RequireComponent(typeof(CharController))]
public class Feu : Familiar {

    private string familiarName = "Feu";

    private FamiliarAIType ai;
    private Player player;
    private float speed;
    private CharController controller;

    [SerializeField] private bool active;
    [SerializeField] private CharacterMode charMode;

    public override string Name {
        get {
            return familiarName;
        }

        set {
            familiarName = value;
        }
    }

    public override CharController Controller {
        get {
            return controller;
        }

        set {
            controller = value;
        }
    }
    
    public override CharacterMode Mode {
        get {
            return charMode;
        }

        set {
            charMode = value;
        }
    }

    public override Player Player {
        get {
            return player;
        }

        set {
            player = value;
        }
    }

    public override float Speed {
        get {
            return speed;
        }

        set {
            speed = value;
        }
    }

    public override FamiliarAIType AI {
        get {
            return ai;
        }

        set {
            ai = value;
        }
    }

    public override bool Active {
        get {
            return active;
        }

        set {
            active = value;
        }
    }

    protected override void PrimaryAction() {
        throw new System.NotImplementedException();
    }

    // Use this for initialization
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {

    }
}