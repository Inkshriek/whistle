using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Whistle.Familiars;
using Whistle.Actors;
using System.Threading;

[RequireComponent(typeof(ActorController))]
public class TestFamiliar : Familiar {

    [SerializeField] private bool _currentFamiliar;

    // Use this for initialization
    void Start() {
        Player = FindObjectOfType<Player>();
        NavAgent.Skill skill;
        skill.canClimb = false;
        skill.canFly = false;
        skill.canSwim = false;
        AI = new NavAgent(NavMesh.SceneNav, skill);
        Active = true;
        Controller = GetComponent<ActorController>();

        DisplayName = "bitch I gotta pay bills and eat";
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
            AI.GenerateNewPath(transform.position, Player.transform.position);
        }
    }

    protected override void PrimaryAction() {
        throw new System.NotImplementedException();
    }



}