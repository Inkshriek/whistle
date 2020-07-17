using System.Collections.Generic;
using UnityEngine;
using Whistle.Actors;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
[DisallowMultipleComponent]
public class ActorController : MonoBehaviour {

    [SerializeField] private NavType movementType;
    [SerializeField] private float friction = 1;
    [SerializeField] private float aerialControl = 0.5f;
    [SerializeField] private float gravity = 1;
    [SerializeField] [Range(0, 90)] private float slopeTolerance = 60;
    [SerializeField] [Range(0, 1)] private float snapdownRadius = 0.2f;

    private Rigidbody2D rb;
    private BoxCollider2D col;

    private bool hasJumped = false;
    private int layerMask = 1;
    private RaycastHit2D groundCheck;
    private RaycastHit2D neargroundCheck;
    private RaycastHit2D sideCheck;
    private Vector2 force;
    private float ang;
    private Vector2 position;
    private float collisionOffset = 0.01f; //This should never be set to 0 or less
    private float terminalVelocityX = 40;
    private float terminalVelocityY = 40;

    public delegate void ActorEvent(string message);
    public event ActorEvent Jumped;
    public event ActorEvent InAir;
    public event ActorEvent GroundTouched;

    protected bool debugMode = false;
    private Vector2 last;

    public Vector2 Force {
        get {
            return force;
        }
        set {
            force = new Vector2 (
                Mathf.Clamp(value.x, -terminalVelocityX, terminalVelocityX), 
                Mathf.Clamp(value.y, -terminalVelocityY, terminalVelocityY)
            );
        }
    }
    public NavType MovementType { get { return movementType; } set { movementType = value; } }
    public float Friction { get { return friction; } set { friction = value; } }
    public float AerialControl { get { return aerialControl; } set { aerialControl = value; } }
    public float Gravity { get { return gravity; } set { gravity = value; } }
    public float SlopeTolerance { get { return slopeTolerance; } set { slopeTolerance = Mathf.Clamp(value, 0, 90); } }
    public float SnapdownRadius { get { return snapdownRadius; } set { snapdownRadius = Mathf.Clamp01(value); } }
    public Vector2 InputMotion { get; set; }
    public bool IsTouchingGround { get; private set; }

    void Awake () {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<BoxCollider2D>();
        force = new Vector2(0, 0);

        if (gameObject.GetComponents<ActorController>().Length > 1) {
            Debug.LogError("I'm not sure how you did it but you shouldn't be using more than one ActorController on a GameObject.");
        }
        if (!rb.isKinematic) {
            Debug.LogError("ActorControllers won't work properly if the attached rigidbody is not kinematic. Please change it to kinematic, asshole.");
        }
    }

	void FixedUpdate () {
        position = rb.position;
        groundCheck = Physics2D.Raycast(position, Vector2.down, 0.02f, layerMask);
        bool snapToGround = false;

        if (groundCheck.collider != null && Vector2.Angle(transform.up, groundCheck.normal) <= SlopeTolerance && !hasJumped) {
            ang = Vector2.SignedAngle(transform.up, groundCheck.normal);

            if (!IsTouchingGround) {
                GroundTouched?.Invoke(name + " touched the ground!");
                IsTouchingGround = true;
                force.y = 0;
            }
        }
        else {
            ang = 0;

            if (Force.y < 0) {
                hasJumped = false;
            }

            if (IsTouchingGround) {
                neargroundCheck = Physics2D.Raycast(position, Vector2.down, col.size.y * SnapdownRadius, layerMask);
                if (neargroundCheck && Vector2.Angle(transform.up, neargroundCheck.normal) < SlopeTolerance) {
                    snapToGround = true;
                }
                else {
                    IsTouchingGround = false;
                    InAir?.Invoke(name + " is in the air!");
                }
            }
        }

        switch (movementType) {
            case NavType.Normal:
                if (IsTouchingGround) {
                    //If you are touching the ground, the script will simulate walking. 
                    MoveOnGround(InputMotion.x, snapToGround);

                }
                else {
                    //If you are not touching the ground, the script will simulate being in the air.
                    MoveOffGround(InputMotion, false);
                }
                break;
            case NavType.Flying:
                //Since you can't really "walk" it just ignores MoveOnGround and permanently simulates being in the air.
                MoveOffGround(InputMotion, true);
                break;
            default:
                Debug.Log("For some reason this ActorController is being a little bitch and trying unknown movement types.");
                break;
        }

        rb.position = position; //Updating actual position with the internally saved one.
    }

    private void MoveOnGround(float input, bool snapdown) {

        if (Force.x < input) {
            Force = new Vector2(Mathf.Min(Force.x + Friction, input), Force.y);
        }
        else if (force.x > input) {
            Force = new Vector2(Mathf.Max(Force.x - Friction, input), Force.y);
        }

        if (snapdown) {
            position = neargroundCheck.point + new Vector2(0, collisionOffset);
        }
        
        Vector2 move = new Vector2(Force.x * Mathf.Cos(Mathf.Deg2Rad * ang), Force.x * Mathf.Sin(Mathf.Deg2Rad * ang));
        move *= Time.deltaTime;

        int embreak = 0; //Gotta prevent an infinite loop!
        bool done = false;
        //The point of this below is so we can move the player up a slope (or stop them outright) and never cause a clip in doing so.
        do {
            sideCheck = Physics2D.Linecast(position, move + position, layerMask);
            if (sideCheck.collider != null) {
                float distanceA = Vector2.Distance(position, move + position);
                float distanceB = Vector2.Distance(position, sideCheck.point);
                float percentdiff = Mathf.Max(distanceB / distanceA, 0);
                move = move * percentdiff - move.normalized * collisionOffset;

                if (Vector2.Angle(Vector2.up, sideCheck.normal) > SlopeTolerance) {
                    done = true;
                    force.x = 0;
                }
                else {
                    ang = Vector2.SignedAngle(transform.up, sideCheck.normal) - ang;
                    move = new Vector2(move.x * Mathf.Cos(Mathf.Deg2Rad * ang), move.x * Mathf.Sin(Mathf.Deg2Rad * ang));
                }
            }
            else {
                done = true;
            }

            if (move.x > collisionOffset || move.x < -collisionOffset) {
                position += move;
            }

            embreak += 1;
        }
        while (!done && embreak <= 50);
    }

    private void MoveOffGround(Vector2 input, bool isFlying) {

        if (Force.x < input.x && input.x > 0) {
            Force += new Vector2(Mathf.Min(AerialControl, input.x - Force.x), 0);
        }
        else if (Force.x > input.x && input.x < 0) {
            Force += new Vector2(Mathf.Max(-AerialControl, input.x - Force.x), 0);
        }

        if (Force.y < input.y && input.y > 0) {
            Force += new Vector2(0, Mathf.Min(AerialControl, input.y - Force.y));
        }
        else if (Force.y > input.y && input.y < 0) {
            Force += new Vector2(0, Mathf.Max(-AerialControl, input.y - Force.y));
        }

        Force -= new Vector2(0, Gravity);
        Vector2 move = Force;
        move *= Time.deltaTime;
        RaycastHit2D collisionCheck = Physics2D.Linecast(position, move + position, layerMask);
        if (collisionCheck.collider != null) {
            position = collisionCheck.point + collisionCheck.normal * collisionOffset;
            ang = Vector2.SignedAngle(transform.up, collisionCheck.normal);
            if (Mathf.Abs(ang) <= slopeTolerance) {
                force.y = 0;
            }
            else {
                float r = Mathf.Deg2Rad * ang;
                float dy = (Mathf.Sin(-r) * Force.x) + (Mathf.Cos(-r) * Force.y);
                float x = -(Mathf.Sin(r) * dy);
                float y = (Mathf.Cos(r) * dy);
                Force -= new Vector2(x, y);
            }
        }
        else {
            position += move;
        }
    }

    public void ApplyJump(float height) {
        Force = new Vector2(Force.x, height);
        IsTouchingGround = false;
        hasJumped = true;
        InAir?.Invoke(name + " is in the air! (had jumped)");
        Jumped?.Invoke(name + " jumped by " + height + " units!");
    }

    public void ApplyJump(Vector2 input) {
        Force = new Vector2(Force.x + input.x, input.y);
        IsTouchingGround = false;
        hasJumped = true;
        InAir?.Invoke(name + " is in the air! (had jumped)");
        Jumped?.Invoke(name + " jumped by " + input.y + " units!");
    }

    /*
    private List<ContactPoint2D> CheckGroundContacts() {
        ContactPoint2D[] contactsAll = new ContactPoint2D[20];
        List<ContactPoint2D> contacts = new List<ContactPoint2D>();
        int contactsNum = col.GetContacts(contactsAll);
        for (int i = 0; i < contactsNum; i++) {
            float ang = Vector2.Angle(transform.up, contactsAll[i].normal);
            if (ang < slopeTolerance) {
                contacts.Add(contactsAll[i]);
            }
        }
        return contacts;
    }

    private List<ContactPoint2D> CheckWallContacts() {
        ContactPoint2D[] contactsAll = new ContactPoint2D[20];
        List<ContactPoint2D> contacts = new List<ContactPoint2D>();
        int contactsNum = col.GetContacts(contactsAll);
        for (int i = 0; i < contactsNum; i++) {
            float ang = Vector2.Angle(transform.up, contactsAll[i].normal);
            if (ang >= slopeTolerance) {
                contacts.Add(contactsAll[i]);
            }
        }
        return contacts;
    }

    private RaycastHit2D CheckColliders(int raycastCount, Vector2 direction, Vector2 start, Vector2 end) {

        RaycastHit2D[] raycastArray = new RaycastHit2D[raycastCount];
        float closestDistance = Mathf.Infinity;
        RaycastHit2D bestValid = new RaycastHit2D();

        for (int i = 0; i < raycastCount; i++) {
            
            Vector2 originPoint = Vector2.Lerp(start, end, (float)i / (raycastCount - 1));
            Debug.DrawRay(originPoint, direction, Color.red);
            RaycastHit2D ray = Physics2D.Linecast(originPoint, direction + originPoint, layerMask);

            if (ray) {
                float rayDistance = col.bounds.SqrDistance(ray.point);
                if (rayDistance < closestDistance) {
                    bestValid = ray;
                }
                closestDistance = Mathf.Min(rayDistance, closestDistance);
            }
            raycastArray[i] = ray;
        }

        return bestValid;
    }
    */

}
