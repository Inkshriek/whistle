using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Whistle.Familiars;

public class Feu : Familiar {

    private string familiarName = "Feu";


    public override string Name {
        get {
            return familiarName;
        }

        set {
            familiarName = value;
        }
    }

    public override FamiliarState State {
        get {
            throw new System.NotImplementedException();
        }

        set {
            throw new System.NotImplementedException();
        }
    }
    public override GameObject Player {
        get {
            throw new System.NotImplementedException();
        }

        set {
            throw new System.NotImplementedException();
        }
    }
    public override string Speed {
        get {
            throw new System.NotImplementedException();
        }

        set {
            throw new System.NotImplementedException();
        }
    }

    public override void Activate() {
        throw new System.NotImplementedException();
    }

    public override void Deactivate() {
        throw new System.NotImplementedException();
    }

    protected override void MovePosition() {
        base.MovePosition();
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