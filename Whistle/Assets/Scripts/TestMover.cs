using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMover : MonoBehaviour {

    public float speed;
    private Rigidbody2D rb;
    private float acceleration;
    private bool movingUp;

    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody2D>();
        acceleration = 1;
        movingUp = true;
	}
	
	// Update is called once per frame
	void Update () {
        rb.MovePosition(rb.position + new Vector2(0, speed * acceleration * Time.deltaTime));
        if (movingUp) {
            acceleration -= 0.5f * Time.deltaTime;
            if (acceleration < -1) {
                movingUp = false;
            }
        }
        else if (!movingUp) {
            acceleration += 0.5f * Time.deltaTime;
            if (acceleration > 1) {
                movingUp = true;
            }
        }

    }
}
