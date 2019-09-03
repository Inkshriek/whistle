using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Whistle.Actors;

public class TestMonster : MonsterBase {

    private Rigidbody2D _rb;
    private BoxCollider2D _col;
    private Transform _transform;

    private float _speed = 5;

    protected override void Patrolling() {
        _animation.AnimationName = "walk";
        Controller.Motion = new Vector2(_speed, 0);
        RaycastHit2D check = Physics2D.BoxCast((Vector2)_transform.position + _col.offset, new Vector2(1, _col.size.y * _transform.localScale.y), 0, Vector2.right * Mathf.Sign(_speed), 1, 1);
        if (check) {
            _speed *= -1;
            _animation.transform.localScale = new Vector3(-_animation.transform.localScale.x, _animation.transform.localScale.y, _animation.transform.localScale.z);
        }

        if (Time.fixedTime % 10 == 0) {
            StartBehavior(IdleAnim);
        }

        if (Senses.TargetVisible) {
            State = MonsterState.Chasing;
        }

    }

    protected override void Alert() {

    }

    protected override void Chasing() {
        if (!AI.Operating) {
            if (AI.PathReady) {
                Debug.Log("it do the thing");
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

                _speed = Mathf.Sign(direction.x) * Mathf.Abs(_speed);
                if (direction.x != 0) {
                    Controller.Motion = new Vector2(_speed, 0);
                }
                else {
                    Controller.Motion = new Vector2(0, 0);
                }


            }

            AI.ResetPath();
            AI.GenerateNewPath(transform.position, Senses.Target.transform.position);
        }

        _animation.AnimationName = "run";
        RaycastHit2D check = Physics2D.BoxCast((Vector2)_transform.position + _col.offset, _col.size, 0f, Vector2.right * Mathf.Sign(_speed), 2, LayerMask.GetMask("Player"));
        if (check) {
            StartBehavior(AttackAnim);
        }

        _animation.transform.localScale = new Vector3(-Mathf.Sign(_speed) * Mathf.Abs(_animation.transform.localScale.x), _animation.transform.localScale.y, _animation.transform.localScale.z);
    }

    protected override void Enraged() {
    }

    private IEnumerator IdleAnim() {

        Controller.Motion = new Vector2(0, 0);
        _animation.AnimationName = "idle";
        for (float i = 0; i < 3; i += 0.1f) {
            if (Senses.TargetVisible) {
                State = MonsterState.Chasing;
                EndBehavior();
            }
            yield return new WaitForSeconds(0.1f);
        }
        EndBehavior();
    }

    private IEnumerator AttackAnim() {

        Controller.Motion = new Vector2(0, 0);
        _animation.AnimationName = "attack";
        DamageBox.Create(transform, new Vector2(2 * Mathf.Sign(_speed), 0), new Vector2(2, 2), 25, 1, false, DamageType.Normal, "Player");
        yield return new WaitForSeconds(1);
        EndBehavior();
        
    }


    private void Start() {
        DisplayName = "Test";
        Active = true;
        _rb = GetComponent<Rigidbody2D>();
        _col = GetComponent<BoxCollider2D>();
        _transform = GetComponent<Transform>();
        AI = new NavAgent(NavMesh.SceneNav, false, false, false);
        State = MonsterState.Patrolling;
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        
    }
}
