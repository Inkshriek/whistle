using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    private Transform trans;
    private Rigidbody2D rb;
    private BoxCollider2D[] col;
    private BoxCollider2D colBody;
    private BoxCollider2D colFeet;

    private PhysicsMaterial2D colMat;

    public float speed;
    public float jumpHeight;
    public float aerialDrag;
    [Range(0, 90)] public float slopeTolerance;

    private float moveDirection;
    private float motionHorizontal;
    private float motionVertical;
    private bool isTouchingGround;

    private float velocityInitialJump;
    private float velocityHorizontal;
    private float velocityVertical;

    private Vector2 currentPosition;
    private Vector2 targetPosition;

    void Start () {
        trans = GetComponent<Transform>();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponents<BoxCollider2D>();
        colBody = col[0];
        colFeet = col[1];

        colMat = colFeet.sharedMaterial;

        motionHorizontal = 0;
        motionVertical = 0;
        velocityInitialJump = 0;
        velocityHorizontal = 0;
        velocityVertical = 0;
}

    void FixedUpdate() {
        //Get some values to make this a little easier to manage.
        currentPosition = trans.position;
        

        //Handle horizontal motion.
        moveDirection = Input.GetAxisRaw("Horizontal");


        //Handle vertical motion.
        /* forceVertical -= gravity * Time.deltaTime;
        if (Input.GetKey(KeyCode.Space)) {
            forceVertical = jumpHeight * Time.deltaTime;
        }

        //Get an end position based off where the two forces will inevitably take us.
        targetPosition = new Vector2 (currentPosition.x + forceHorizontal, currentPosition.y + forceVertical);

        //Do some mathing if the vertical force is less than 0.
        if (forceVertical < 0)
        {
            //Check for the ground using a BoxCast.
            RaycastHit2D groundcheck = Physics2D.BoxCast(currentPosition, col.size, 0f, Vector2.down, Mathf.Abs(forceVertical));
            Debug.DrawRay(feet, Vector2.down * Mathf.Abs(forceVertical));
            if (groundcheck)
            {
                forceVertical = groundcheck.point.y - feet.y;
            }
        }
        else if (forceVertical > 0)
        {
            RaycastHit2D ceilingcheck = Physics2D.BoxCast(currentPosition, col.size, 0f, Vector2.up, Mathf.Abs(forceVertical));
            if (ceilingcheck)
            {
                forceVertical = ceilingcheck.point.y - (feet.y + col.size.y);
            }
        }
        */


        RaycastHit2D groundcheck = Physics2D.BoxCast(currentPosition, colFeet.size, 0f, Vector2.down, Mathf.Infinity);
        if (groundcheck && colFeet.IsTouching(groundcheck.collider)) {
            Debug.Log("Ground touched!");

            isTouchingGround = true;
        }
        else {
            if (isTouchingGround)
            {
                if (groundcheck.rigidbody != null)
                {
                    velocityInitialJump = groundcheck.rigidbody.velocity.x;
                }
                else
                {
                    velocityInitialJump = 0;

                }
                velocityHorizontal = velocityInitialJump + moveDirection * speed;
            }
            isTouchingGround = false;
        }
        

        if (isTouchingGround) {
            //If you are touching the ground, the script will simulate walking.

            Debug.DrawLine(currentPosition, groundcheck.point);
            float ang = Vector2.SignedAngle(transform.up, groundcheck.normal);
            float angR = Mathf.Deg2Rad * ang;

            if (ang > slopeTolerance || ang < -slopeTolerance) {
                colFeet.sharedMaterial = colBody.sharedMaterial;
                motionHorizontal = moveDirection * speed * Mathf.Cos(angR);
                motionVertical = 0;
            }
            else
            {
                if (Input.GetKey(KeyCode.Space))
                {

                    rb.velocity = new Vector2(rb.velocity.x, jumpHeight);
                }
                colFeet.sharedMaterial = colMat;
                motionVertical = moveDirection * speed * Mathf.Sin(angR);
                motionHorizontal = moveDirection * speed * Mathf.Cos(angR);
            }

            Vector2 newPosition = new Vector2(motionHorizontal, motionVertical);
            rb.position += newPosition * Time.fixedDeltaTime;

            Debug.Log("Angle: " + ang + ", Force: " + newPosition);
            Debug.Log("Velocity: " + rb.velocity);
        }
        else {
            //If you are not touching the ground, the script will simulate being in the air.

            colFeet.sharedMaterial = colBody.sharedMaterial;

            velocityHorizontal += moveDirection * speed / 10;
            velocityHorizontal = Mathf.Min(speed + velocityInitialJump, Mathf.Max(-speed + velocityInitialJump, velocityHorizontal));

            rb.velocity = new Vector2(velocityHorizontal, rb.velocity.y);
            Debug.Log("Velocity: " + rb.velocity);
        }
        

        Debug.Log(rb.velocity);

        motionVertical = 0;
       
    }

    void LateUpdate()
    {
        
    }

    void jump()
    {
        //??????????
    }

    
}
