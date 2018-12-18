using System.Collections.Generic;
using UnityEngine;
using Whistle.Characters;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class CharController : MonoBehaviour {

    [SerializeField] public MovementType movementType;
    [SerializeField] public float friction;
    [SerializeField] public float aerialControl;
    [SerializeField] [Range(0, 90)] public float slopeTolerance;
    [SerializeField] [Range(0, 1)] public float snapdownRadius;
    [SerializeField] public int horizontalRays;
    [SerializeField] public int verticalRays;

    [HideInInspector] public bool isTouchingGround;
    [HideInInspector] public float H;
    [HideInInspector] public float V;
    [HideInInspector] private float forceSmoothed;

    private Transform trans;
    private Rigidbody2D rb;
    private BoxCollider2D col;
    private PhysicsMaterial2D baseMaterial;
    private PhysicsMaterial2D airMaterial;

    private bool hasJumped;
    private int layerMask;
    private RaycastHit2D neargroundCheck;

    public Vector2 Motion { get; set; }

    List<ContactPoint2D> validContacts;
    float ang;

    public void ApplyJump(float height) {
        rb.velocity = new Vector2(0, height);
        hasJumped = true;
    }


    void Start () {
        trans = GetComponent<Transform>();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<BoxCollider2D>();

        layerMask = 1 << 8;
        layerMask = ~layerMask;

        baseMaterial = Resources.Load("Mat_CharGround") as PhysicsMaterial2D;
        airMaterial = Resources.Load("Mat_CharAir") as PhysicsMaterial2D;
    }
	

	void FixedUpdate () {
        validContacts = CheckGroundContacts();

        bool snapToGround = false;

        if (validContacts.Count > 0) {
            ang = Vector2.SignedAngle(transform.up, validContacts[0].normal);

            if (!isTouchingGround) {
                forceSmoothed = rb.velocity.x;
                rb.velocity = Vector2.zero;
            }

            isTouchingGround = true;
        }
        else {
            ang = 0;

            if (isTouchingGround) {
                neargroundCheck = CheckColliders(verticalRays, Vector2.down * snapdownRadius, new Vector2(col.bounds.min.x, col.bounds.min.y), new Vector2(col.bounds.max.x, col.bounds.min.y));
                if (neargroundCheck && Vector2.Angle(transform.up, neargroundCheck.normal) < slopeTolerance && !hasJumped) {
                    snapToGround = true;
                }
                else {
                    hasJumped = false;
                    isTouchingGround = false;
                    rb.velocity += new Vector2(forceSmoothed, 0);
                }
            }
        }

        switch (movementType) {
            case MovementType.Normal:
                if (isTouchingGround) {
                    //If you are touching the ground, the script will simulate walking. 
                    MoveOnGround(Motion.x, snapToGround);

                }
                else {
                    //If you are not touching the ground, the script will simulate being in the air.
                    MoveOffGround(Motion);
                }
                break;
            case MovementType.Flying:
                MoveOffGround(Motion);
                break;
            default:
                Debug.Log("oof");
                break;
        }
    }

    private void MoveOnGround(float force, bool snapdown) {
        if (snapdown) {
            Debug.Log("Snapping!");
            rb.position += new Vector2(Motion.x * Time.deltaTime, neargroundCheck.point.y - col.bounds.min.y);
        }
        else {

            if (forceSmoothed < force) {
                forceSmoothed = Mathf.Min(forceSmoothed + friction, force);
            }
            else if (forceSmoothed > force) {
                forceSmoothed = Mathf.Max(forceSmoothed - friction, force);
            }

            col.sharedMaterial = baseMaterial;

            H = forceSmoothed * Mathf.Cos(Mathf.Deg2Rad * ang);
            V = forceSmoothed * Mathf.Sin(Mathf.Deg2Rad * ang);

            Vector2 move = new Vector2(H, V);

            

            Vector2 cornerPoint = new Vector2(rb.position.x + Mathf.Sign(forceSmoothed) * (col.size.x / 2), col.bounds.min.y);
            //RaycastHit2D sideCheck = Physics2D.Linecast(cornerPoint, move * Time.deltaTime + cornerPoint, layerMask);

            RaycastHit2D sideCheck = CheckColliders(horizontalRays, move * Time.deltaTime, new Vector2(rb.position.x + Mathf.Sign(forceSmoothed) * (col.size.x / 2), col.bounds.min.y), new Vector2(rb.position.x + Mathf.Sign(forceSmoothed) * (col.size.x / 2), col.bounds.max.y));
            
            if (sideCheck && Vector2.Angle(transform.up, sideCheck.normal) > slopeTolerance) {
                float distanceA = col.bounds.SqrDistance(new Vector2(sideCheck.point.x - Mathf.Sign(forceSmoothed) * 0.02f, sideCheck.point.y));
                float distanceB = col.bounds.SqrDistance(move * Time.deltaTime + cornerPoint);
                float percentdiff = distanceA / distanceB;
                move *= Mathf.Max(percentdiff, 0);
            }

            rb.position += move * Time.deltaTime;
        }
    }

    private void MoveOffGround(Vector2 force) {

        col.sharedMaterial = airMaterial;

        if (rb.velocity.x < force.x && force.x > 0) {
            H = Mathf.Min(aerialControl, force.x - rb.velocity.x);
        }
        else if (rb.velocity.x > force.x && force.x < 0) {
            H = Mathf.Max(-aerialControl, force.x - rb.velocity.x);
        }
        else {
            H = 0;
        }

        V = 0;


        Vector2 move = new Vector2(H, V);


        rb.velocity += move;
    }

    private List<ContactPoint2D> CheckGroundContacts() {
        ContactPoint2D[] contactsAll = new ContactPoint2D[20];
        List<ContactPoint2D> contacts = new List<ContactPoint2D>();
        int contactsNum = col.GetContacts(contactsAll);
        for (int i = 0; i < contactsNum; i++) {
            float ang = Vector2.Angle(transform.up, contactsAll[i].normal);
            if (ang < slopeTolerance && (V <= 0 || isTouchingGround)) {
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
