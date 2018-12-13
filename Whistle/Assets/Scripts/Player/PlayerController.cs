using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Whistle.Conditions;
using Whistle.Characters;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class PlayerController : Player, ICharacter, IHealth, IConditions {

    [SerializeField] public float walkSpeed;
    [SerializeField] public float runSpeed;
    [SerializeField] public float crouchSpeed;
    [SerializeField] private float jumpHeight;
    [SerializeField] private float friction;
    [SerializeField] private float aerialControl;
    [SerializeField] [Range(0, 90)] public float slopeTolerance;
    [SerializeField] private float maxHealth;
    [SerializeField] private float effectiveHealth;
    [SerializeField] private float currentHealth;

    private Transform trans;
    private Rigidbody2D rb;
    private BoxCollider2D col;
    [SerializeField] private PhysicsMaterial2D matAir;
    [SerializeField] private PhysicsMaterial2D matGround;

    private string charname = "Ichabod";

    [HideInInspector] public float moveDirection;
    [HideInInspector] public bool isTouchingGround;
    [HideInInspector] public PlayerState state;
    [HideInInspector] public bool allowCrouch;
    [HideInInspector] public bool allowJump;
    [HideInInspector] public bool allowMidairJump;

    [HideInInspector] public float groundH;
    [HideInInspector] public float groundV;

    [HideInInspector] public float airHInitial;
    [HideInInspector] public float airH;
    [HideInInspector] public float airV;

    private Vector2 currentPosition;
    private RaycastHit2D overlapCheck;
    private Vector2 newVelocity;

    public float ActiveSpeed {
        get {
            switch (state) {
                case PlayerState.Walking:
                    return walkSpeed;

                case PlayerState.Crouching:
                    return crouchSpeed;

                case PlayerState.Running:
                    return runSpeed;

                default:
                    Debug.LogError("PlayerState is in an impossible position! May want to fix that quick.");
                    return 0;
            }
        }
    }

    public float Speed {
        get {
            return walkSpeed;
        }
        set {
            float multRun = runSpeed / walkSpeed;
            float multCrouch = crouchSpeed / walkSpeed;
            walkSpeed = value;
            runSpeed = value * multRun;
            crouchSpeed = value * multCrouch;
        }
    }

    public string Name {
        get {
            return charname;
        }
        set {
            charname = value;
        }
    }

    public float JumpHeight {
        get {
            return jumpHeight;
        }
        set {
            jumpHeight = value;
        }
    }

    public float CurrentHealth {
        get {
            return currentHealth;
        }
        set {
            currentHealth = Mathf.Max(0, value);
        }
    }

    public float EffectiveHealth {
        get {
            throw new System.NotImplementedException();
        }
        set {

        }
    }

    public float MaxHealth {
        get {
            return maxHealth;
        }
        set {
            maxHealth = value;
        }
    }


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
            rb.velocity = new Vector2(0, jumpHeight);
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
        float ang = Vector2.SignedAngle(transform.up, contacts[0].normal);
        float angR = Mathf.Deg2Rad * ang;

        RaycastHit2D sidecheck;

        if (contacts.Count > 1) {
            if (!isTouchingGround)
                rb.velocity = new Vector2(0, rb.velocity.y);

            isTouchingGround = true;
        }
        else {
            if (isTouchingGround) {
                airHInitial = rb.velocity.x;
                airH = airHInitial + moveDirection * ActiveSpeed;
            }
            isTouchingGround = false;
        }




        if (isTouchingGround) {
            //If you are touching the ground, the script will simulate walking.

            Debug.DrawLine(currentPosition, contacts[0].point);

            col.sharedMaterial = matGround;

            groundH = moveDirection * ActiveSpeed * Mathf.Cos(angR);
            groundV = moveDirection * ActiveSpeed * Mathf.Sin(angR);

            Vector2 targetPosition = new Vector2(groundH, groundV);
            Vector2 referencePoint = new Vector2(rb.velocity.x + currentPosition.x + moveDirection * col.size.x / 2, rb.velocity.y + currentPosition.y - col.size.y / 2); //Getting this because groundcheck.point is unreliable here.            
            sidecheck = Physics2D.Raycast(referencePoint, targetPosition, ActiveSpeed * Time.deltaTime);
            if (sidecheck && Vector2.Angle(transform.up,sidecheck.normal) > slopeTolerance) {

                Debug.DrawLine(referencePoint, sidecheck.point, Color.red);

                float distanceA = Vector2.Distance(referencePoint, new Vector2(sidecheck.point.x - Physics2D.defaultContactOffset * moveDirection, sidecheck.point.y));
                float distanceB = Vector2.Distance(referencePoint, contacts[0].point + targetPosition * Time.deltaTime);
                float percentdiff = distanceA / distanceB;
                targetPosition *= percentdiff;
            }

            rb.position += targetPosition * Time.deltaTime;
        }
        else {
            //If you are not touching the ground, the script will simulate being in the air.

            col.sharedMaterial = matAir;

            sidecheck = Physics2D.BoxCast(currentPosition, new Vector2(col.size.x, col.size.y), 0f, Vector2.right * Mathf.Sign(airH), Mathf.Infinity);

            float minSpd = -walkSpeed;
            float maxSpd = walkSpeed;
            float addedSpd = moveDirection * walkSpeed * Time.deltaTime * aerialControl;

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
            if (ang < slopeTolerance && airV <= 0) {
                contacts.Add(contactsAll[i]);
            }
        }
        contacts.Add(new ContactPoint2D());
        return contacts;
    }

    void OnCollisionEnter2D(Collision2D c) {

    }
}
