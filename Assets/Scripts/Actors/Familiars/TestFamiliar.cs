using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Whistle.Familiars;
using System.Threading;
using Whistle.Actors;

[RequireComponent(typeof(ActorController))]
public class TestFamiliar : Familiar {

    private NavAgent AI;
    private BoxCollider2D col;

    // Use this for initialization
    void Start() {
        Player = FindObjectOfType<Player>();
        AI = new NavAgent(NavMesh.SceneNav, false, false, false);
        Active = true;
        Controller = GetComponent<ActorController>();
        col = GetComponent<BoxCollider2D>();

        DisplayName = "bitch I gotta pay bills and eat";
    }

    protected override void Behavior() {
        if (AI.PathReady) {
            //This part will move them left or right depending on which way will lead them closer to the player.
            Vector2 direction;
            int index = AI.GetNextDirection(transform.position, out direction);

            while (direction.x == 0) {
                if (direction.y != 0) {
                    index++;
                    AI.GetNextDirection(index, out direction);
                }
                else {
                    break;
                }
            }

            if (direction.x != 0) {
                if (Vector2.Distance(Player.gameObject.transform.position, transform.position) > 5) {
                    Controller.InputMotion = new Vector2(Mathf.Sign(direction.x) * 10, 0);
                }
                else {
                    Controller.InputMotion = new Vector2(Mathf.Sign(direction.x) * 5, 0);
                }
            }
            else {
                Controller.InputMotion = new Vector2(0, 0);
            }

            //Checking if they need to jump.
            if (!Physics2D.OverlapBox(new Vector2(col.bounds.center.x + Mathf.Sign(Controller.InputMotion.x) * col.bounds.extents.x, col.bounds.min.y), col.size, 0) && Controller.IsTouchingGround) {
                Controller.ApplyJump(10);
                Debug.Log("test jump");
            }
        }
        AI.GenerateNewPath(transform.position, Player.transform.position, false);
    }

    protected override void FamiliarAbility() {
        throw new System.NotImplementedException();
    }



}