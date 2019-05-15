using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Whistle.Familiars;
using Whistle.Actors;
using System.Threading;

[RequireComponent(typeof(ActorController))]
public class Feu : Familiar {

    private Player player;
    private float speed;
    private ActorController controller;

    [SerializeField] private bool active;
    [SerializeField] private ActorMode charMode;

    public override ActorController Controller {
        get {
            return controller;
        }

        set {
            controller = value;
        }
    }
    
    public override ActorMode Mode {
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
        AI = new NavAgent(NavMesh.SceneNavMesh);
        Mode = ActorMode.Active;
        Controller = GetComponent<ActorController>();

        DisplayName = "Feu";
    }

    protected override void Behavior() {
        if (!AI.Operating) {
            if (AI.PathReady) {

                Vector2 direction;
                int index = AI.ParsePathForDirection(transform.position, out direction);

                while (direction.x == 0) {
                    if (direction.y != 0) {
                        index++;
                        AI.ParsePathForDirection(index, out direction);
                    }
                    else {
                        break;
                    }
                }

                Debug.Log(direction);

                if (direction.x != 0) {
                    Controller.Motion = new Vector2(Mathf.Sign(direction.x) * 5, 0);
                }
                else {
                    Controller.Motion = new Vector2(0, 0);
                }
                
                
            }

            AI.ResetPath();
            AI.GeneratePath(transform.position, player.transform.position);
        }
    }

    protected override void PrimaryAction() {
        throw new System.NotImplementedException();
    }



}