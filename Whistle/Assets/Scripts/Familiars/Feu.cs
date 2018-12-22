using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Whistle.Familiars;
using Whistle.Characters;
using System.Threading;

[RequireComponent(typeof(CharController))]
public class Feu : Familiar {

    private Player player;
    private float speed;
    private CharController controller;

    [SerializeField] private bool active;
    [SerializeField] private CharacterMode charMode;

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

    public override bool Active {
        get {
            return active;
        }

        set {
            active = value;
        }
    }

    // Use this for initialization
    void Start() {
        player = FindObjectOfType<Player>();
        AI = new AIAgent(NavMesh.SceneNavMesh);
        Mode = CharacterMode.Active;
        Controller = GetComponent<CharController>();

        currentBehavior = Behavior;

        DisplayName = "Feu";
    }

    public override void ApplyBehavior(Behavior behavior) {
        throw new System.NotImplementedException();
    }

    public override void ResetBehavior() {
        throw new System.NotImplementedException();
    }

    private void Behavior() {

        if (!AI.Operating) {
            if (AI.PathReady) {
                Vector2 direction;
                int index = AI.ParsePathForDirection(transform.position, out direction);


                while (direction.x == 0) {
                    if (direction.y > 0) {
                        index++;
                        AI.ParsePathForDirection(index, out direction);
                    }
                    else {
                        break;
                    }
                }


                Controller.Motion = new Vector2(Mathf.Sign(direction.x) * 5, 0);
            }

            AI.ResetPath();
            AI.GeneratePath(transform.position, player.transform.position);
        }
    }

    protected override void PrimaryAction() {
        throw new System.NotImplementedException();
    }



}