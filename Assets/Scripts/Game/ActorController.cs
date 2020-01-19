using System.Collections.Generic;
using UnityEngine;
using Whistle.Actors;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
[DisallowMultipleComponent]
public class ActorController : MonoBehaviour {

    [SerializeField] public NavType movementType;
    [SerializeField] public float friction;
    [SerializeField] public float aerialControl;
    [SerializeField] [Range(0, 90)] public float slopeTolerance;
    [SerializeField] [Range(0, 1)] public float snapdownRadius;
    [SerializeField] public int horizontalRays;
    [SerializeField] public int verticalRays;

    public Vector2 InputMotion { get; set; }
    public bool IsTouchingGround { get; private set; }

    private Rigidbody2D rb;
    private BoxCollider2D col;
    private PhysicsMaterial2D baseMaterial;
    private PhysicsMaterial2D airMaterial;

    private bool hasJumped;
    private int layerMask;
    private RaycastHit2D neargroundCheck;
    private Vector2 forceSmoothed;
    private List<ContactPoint2D> groundContacts;
    private List<ContactPoint2D> wallContacts;
    private float ang;

    public delegate void ActorEvent(string message);
    public event ActorEvent Jumped;
    public event ActorEvent InAir;
    public event ActorEvent GroundTouched;

    protected bool debugMode = false;
    private Vector2 last;

    void Awake () {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<BoxCollider2D>();

        layerMask = 1;
        forceSmoothed = new Vector2(0, 0);
        hasJumped = false;

        baseMaterial = Resources.Load("Mat_ActorGround") as PhysicsMaterial2D;
        airMaterial = Resources.Load("Mat_ActorAir") as PhysicsMaterial2D;

        if (gameObject.GetComponents<ActorController>().Length > 1) {
            Debug.LogError("I'm not sure how you did it but you shouldn't be using more than one ActorController on a GameObject.");
        }
    }

	void FixedUpdate () {
        groundContacts = CheckGroundContacts();
        wallContacts = CheckWallContacts();

        bool snapToGround = false;

        if (groundContacts.Count > 0 && !hasJumped) {
            ang = Vector2.SignedAngle(transform.up, groundContacts[0].normal);

            if (!IsTouchingGround) {
                forceSmoothed.x = rb.velocity.x;
                rb.velocity = Vector2.zero;
                GroundTouched?.Invoke(name + " touched the ground!");
            }

            IsTouchingGround = true;
        }
        else {
            ang = 0;
            if (rb.velocity.y < 0) {
                hasJumped = false;
            }

            if (IsTouchingGround) {
                neargroundCheck = CheckColliders(verticalRays, Vector2.down * snapdownRadius, new Vector2(col.bounds.min.x, col.bounds.min.y), new Vector2(col.bounds.max.x, col.bounds.min.y));
                if (neargroundCheck && Vector2.Angle(transform.up, neargroundCheck.normal) < slopeTolerance) {
                    snapToGround = true;
                }
                else {
                    rb.velocity += new Vector2(forceSmoothed.x, 0);
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
                MoveOffGround(InputMotion, true);
                break;
            default:
                Debug.Log("For some reason this ActorController is being a little bitch and trying impossible movement types.");
                break;
        }

        if (debugMode == true) {
            if (last == null) {
                last = transform.position;
            }
            else {
                Debug.DrawLine(last, transform.position, Color.green, 5);
                last = transform.position;
            }
        }
    }

    private void MoveOnGround(float force, bool snapdown) {

        if (snapdown) {
            rb.position += new Vector2(forceSmoothed.x * Time.deltaTime, neargroundCheck.point.y - col.bounds.min.y);
        }
        else {

            if (forceSmoothed.x < force) {
                forceSmoothed.x = Mathf.Min(forceSmoothed.x + friction, force);
            }
            else if (forceSmoothed.x > force) {
                forceSmoothed.x = Mathf.Max(forceSmoothed.x - friction, force);
            }

            col.sharedMaterial = baseMaterial;


            Vector2 move = new Vector2(forceSmoothed.x * Mathf.Cos(Mathf.Deg2Rad * ang), forceSmoothed.x* Mathf.Sin(Mathf.Deg2Rad * ang));
            move *= Time.deltaTime;

            Vector2 cornerPoint = new Vector2(rb.position.x + Mathf.Sign(forceSmoothed.x) * (col.size.x / 2), col.bounds.min.y);
            RaycastHit2D sideCheck = CheckColliders(horizontalRays, move, new Vector2(rb.position.x + Mathf.Sign(forceSmoothed.x) * (col.size.x / 2), col.bounds.min.y), new Vector2(rb.position.x + Mathf.Sign(forceSmoothed.x) * (col.size.x / 2), col.bounds.max.y));

            /* TODO: Fix this issue.
             * Sometimes, for one single frame, the player moves into slopes instead of stopping as they should.
             * The "move" is so small the sideCheck returns nothing, yet you still move just enough to cause the weird "jittering".
             */

            if (sideCheck && Vector2.Angle(transform.up, sideCheck.normal) > slopeTolerance) {
                float distanceA = col.bounds.SqrDistance(new Vector2(sideCheck.point.x - Mathf.Sign(forceSmoothed.x) * 0.02f, sideCheck.point.y));
                float distanceB = col.bounds.SqrDistance(move + cornerPoint);
                float percentdiff = distanceA / distanceB;
                move *= Mathf.Max(percentdiff, 0);
                forceSmoothed.x = 0;
            }

            if (move.x > 0.02 || move.x < -0.02) {
                rb.position += move;
            }
        }
    }

    private void MoveOffGround(Vector2 force, bool isFlying) {

        col.sharedMaterial = airMaterial;

        if (rb.velocity.x < force.x && force.x > 0) {
            forceSmoothed.x = Mathf.Min(aerialControl, force.x - rb.velocity.x);
        }
        else if (rb.velocity.x > force.x && force.x < 0) {
            forceSmoothed.x = Mathf.Max(-aerialControl, force.x - rb.velocity.x);
        }
        else {
            forceSmoothed.x = 0;
        }

        if (forceSmoothed.y < force.y && force.y > 0) {
            forceSmoothed.y = Mathf.Min(aerialControl, force.y - rb.velocity.y);
        }
        else if (rb.velocity.y > force.y && force.y < 0) {
            forceSmoothed.y = Mathf.Max(-aerialControl, force.y - rb.velocity.y);
        }
        else {
            forceSmoothed.y = 0;
        }

        if (wallContacts.Count > 0) {
            forceSmoothed.x = 0;
        }

        rb.velocity += forceSmoothed;
    }

    public void ApplyJump(float height) {
        rb.velocity += new Vector2(forceSmoothed.x, height);
        IsTouchingGround = false;
        hasJumped = true;
        InAir?.Invoke(name + " is in the air! (had jumped)");
        Jumped?.Invoke(name + " jumped by " + height + " units!");
    }

    public void ApplyJump(Vector2 force) {
        rb.velocity += new Vector2(forceSmoothed.x + force.x, force.y);
        IsTouchingGround = false;
        hasJumped = true;
        InAir?.Invoke(name + " is in the air! (had jumped)");
        Jumped?.Invoke(name + " jumped by " + force.y + " units!");
    }

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


}
