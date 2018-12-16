using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Whistle.Conditions;
using Whistle.Characters;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class PlayerController : Player {

    private Transform trans;
    private Rigidbody2D rb;
    private BoxCollider2D col;
    [SerializeField] private PhysicsMaterial2D matGround;
    [SerializeField] private PhysicsMaterial2D matAir;

    private Vector2 currentPosition;
    private RaycastHit2D overlapCheck;

    void Start () {
        trans = GetComponent<Transform>();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<BoxCollider2D>();

        groundH = 0;
        groundV = 0;
        airHInitial = 0;
        airH = 0;
        airV = 0;

        state = PlayerState.Walking;
    }

    void Update() {
        if (isTouchingGround && Input.GetKeyDown(KeyCode.Space))
            rb.velocity = new Vector2(0, JumpHeight);
    }

    void FixedUpdate() {

        overlapCheck = Physics2D.BoxCast(currentPosition, col.size, 0f, Vector2.zero, 0f);

        //Get some values to make this a little easier to manage.
        currentPosition = new Vector2(trans.position.x, trans.position.y + col.offset.y);

        if (Input.GetAxisRaw("Vertical") < 0)
            state = PlayerState.Crouching;
        else if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            state = PlayerState.Running;
        else
            state = PlayerState.Walking;


        moveDirection = Input.GetAxisRaw("Horizontal");

        List<ContactPoint2D> contacts = CheckGround();
        float ang;

        RaycastHit2D sidecheck;
        bool snappingToGround = false;

        if (contacts.Count > 0) {
            ang = Vector2.SignedAngle(transform.up, contacts[0].normal);

            if (!isTouchingGround) {
                rb.velocity = new Vector2(0, rb.velocity.y);
            }

            isTouchingGround = true;
        }
        else {
            ang = 0;
            if (isTouchingGround) {
                RaycastHit2D neargroundCheck = Physics2D.BoxCast(currentPosition - new Vector2(0, col.size.y / 2 - col.size.y / 20), new Vector2(col.size.x, col.size.y / 10), 0f, Vector2.down, 0.5f);
                if (neargroundCheck && Vector2.Angle(transform.up, neargroundCheck.normal) < SlopeTolerance) {
                    snappingToGround = true;

                    rb.position += new Vector2(groundH * Time.deltaTime, neargroundCheck.point.y - (currentPosition.y - col.size.y / 2));
                }
                else {
                    isTouchingGround = false;
                }

                airHInitial = rb.velocity.x;
                airH = airHInitial + moveDirection * ActiveSpeed;
            }
        }

        float angR = Mathf.Deg2Rad * ang;


        if (isTouchingGround && !snappingToGround) {
            //If you are touching the ground, the script will simulate walking. 

            Debug.DrawLine(currentPosition, contacts[0].point);

            col.sharedMaterial = matGround;

            groundH = moveDirection * ActiveSpeed * Mathf.Cos(angR);
            groundV = moveDirection * ActiveSpeed * Mathf.Sin(angR);

            Vector2 targetPosition = new Vector2(groundH, groundV);
            Vector2 referencePoint = new Vector2(rb.velocity.x + currentPosition.x + moveDirection * col.size.x / 2, rb.velocity.y + currentPosition.y - col.size.y / 2); //Getting this because groundcheck.point is unreliable here.            
            sidecheck = Physics2D.Raycast(referencePoint, targetPosition, ActiveSpeed * Time.deltaTime + (Physics2D.defaultContactOffset * 5));
            /*
            if (sidecheck && Vector2.Angle(transform.up,sidecheck.normal) > SlopeTolerance) {

                Debug.DrawLine(referencePoint, sidecheck.point, Color.red);

                float distanceA = Vector2.Distance(referencePoint, new Vector2(sidecheck.point.x - (Physics2D.defaultContactOffset * moveDirection * 5), sidecheck.point.y));
                float distanceB = Vector2.Distance(referencePoint, contacts[0].point + targetPosition * Time.deltaTime);
                float percentdiff = distanceA / distanceB;
                targetPosition *= percentdiff;
            }
            */

            rb.position += targetPosition * Time.deltaTime;
        }
        else if (!isTouchingGround) {
            //If you are not touching the ground, the script will simulate being in the air.

            col.sharedMaterial = matAir;

            sidecheck = Physics2D.BoxCast(currentPosition, new Vector2(col.size.x, col.size.y), 0f, Vector2.right * Mathf.Sign(airH), Mathf.Infinity);

            float minSpd = -Speed;
            float maxSpd = Speed;
            float addedSpd = moveDirection * Speed * Time.deltaTime * AerialControl;

            if (addedSpd + airH > maxSpd)
                addedSpd = maxSpd - airH;
            else if (addedSpd + airH < minSpd)
                addedSpd = minSpd - airH;

            if ((airH < maxSpd && moveDirection > 0) || (airH > minSpd && moveDirection < 0))
                airH += addedSpd;

            if (sidecheck && col.IsTouching(sidecheck.collider))
                airH = 0;

            airV = rb.velocity.y;

            rb.velocity = new Vector2(airH, airV);
        }



        TickConds();
    }

    void LateUpdate()
    {

    }

    private List<ContactPoint2D> CheckGround() {
        ContactPoint2D[] contactsAll = new ContactPoint2D[20];
        List<ContactPoint2D> contacts = new List<ContactPoint2D>();
        int contactsNum = col.GetContacts(contactsAll);
        for (int i = 0; i < contactsNum; i++) {
            float ang = Vector2.Angle(transform.up, contactsAll[i].normal);
            if (ang < SlopeTolerance && airV <= 0) {
                contacts.Add(contactsAll[i]);
            }
        }
        return contacts;
    }

    void OnCollisionEnter2D(Collision2D c) {

    }

    
}
