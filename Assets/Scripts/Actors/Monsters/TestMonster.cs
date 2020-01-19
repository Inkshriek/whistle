using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Whistle.Actors;

public class TestMonster : MonsterBase {

    private Rigidbody2D rb;
    private BoxCollider2D col;
    private Transform trans;

    private float speed = 5;

    protected override void Patrolling() {
        anim.AnimationName = "walk";
        Controller.InputMotion = new Vector2(speed, 0);
        RaycastHit2D check = Physics2D.BoxCast((Vector2)trans.position + col.offset, new Vector2(1, col.size.y * trans.localScale.y), 0, Vector2.right * Mathf.Sign(speed), 1, 1);
        if (check) {
            speed *= -1;
            anim.transform.localScale = new Vector3(-anim.transform.localScale.x, anim.transform.localScale.y, anim.transform.localScale.z);
            Senses.FlipVision();
        }

        if (Time.fixedTime % 10 == 0) {
            StartAction(IdleAnim);
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

                speed = Mathf.Sign(direction.x) * Mathf.Abs(speed);
                if (direction.x != 0) {
                    Controller.InputMotion = new Vector2(speed * 2, 0);
                }
                else {
                    Controller.InputMotion = new Vector2(0, 0);
                }


            }

            AI.ResetPath();
            AI.GenerateNewPath(transform.position, Senses.target.transform.position);
        }

        anim.AnimationName = "run";
        RaycastHit2D check = Physics2D.BoxCast((Vector2)trans.position + col.offset, col.size, 0f, Vector2.right * Mathf.Sign(speed), 2, LayerMask.GetMask("Player"));
        if (check) {
            StartAction(AttackAnim);
        }

        anim.transform.localScale = new Vector3(-Mathf.Sign(speed) * Mathf.Abs(anim.transform.localScale.x), anim.transform.localScale.y, anim.transform.localScale.z);
    }

    protected override void Enraged() {
    }

    private IEnumerator IdleAnim() {

        Controller.InputMotion = new Vector2(0, 0);
        anim.AnimationName = "idle";
        for (float i = 0; i < 3; i += 0.1f) {
            if (Senses.TargetVisible) {
                State = MonsterState.Chasing;
                FinishAction();
            }
            yield return new WaitForSeconds(0.1f);
        }
        FinishAction();
    }

    private IEnumerator AttackAnim() {

        Controller.InputMotion = new Vector2(0, 0);
        anim.AnimationName = "attack";
        //DamageArea.Spawn(transform, new Vector2(2 * Mathf.Sign(speed), 0), new Vector2(2, 2), 25, 1, false, DamageType.Normal);
        DamageArea.Spawn((Vector2)transform.position + new Vector2(2 * Mathf.Sign(speed), 0), "Slash");
        yield return new WaitForSeconds(1);
        FinishAction();
        
    }

    private void Awake() {
        BaseInitialize();

        DisplayName = "Test";
        Active = true;
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<BoxCollider2D>();
        trans = GetComponent<Transform>();
        AI = new NavAgent(NavMesh.SceneNav, false, false, false);
    }

    private void Update() {
        BaseUpdate();
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        
    }
}
