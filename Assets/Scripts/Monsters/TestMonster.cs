using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Whistle.Actors;

public class TestMonster : MonsterBase {

    private Rigidbody2D rb;

    protected override void Patrolling() {
        throw new System.NotImplementedException();
    }

    protected override void Alert() {
        throw new System.NotImplementedException();
    }

    protected override void Chasing() {
        throw new System.NotImplementedException();
    }

    protected override void Enraged() {
        throw new System.NotImplementedException();
    }

    protected override void OnStateChange(MonsterState state) {
        throw new System.NotImplementedException();
    }

    private void Awake() {
        DisplayName = "Test";
        rb = GetComponent<Rigidbody2D>();
        Controller = GetComponent<ActorController>();
    }


}
