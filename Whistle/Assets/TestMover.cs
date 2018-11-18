using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMover : MonoBehaviour {

    public float speed;
    private Rigidbody2D rb;
    private float motion;
    private int direction;

    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody2D>();
        motion = speed;
        direction = 1;
	}
	
	// Update is called once per frame
	void Update () {
        rb.MovePosition(rb.position + new Vector2(motion, 0));

        motion += speed * Time.deltaTime * direction;

        if (motion < -speed)
        {
            direction = 1;
            
        }
        else if (motion > speed)
        {
            direction = -1;
        }
	}
}
