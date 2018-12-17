using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Whistle.Characters;
using Whistle.Conditions;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class CharController : MonoBehaviour {

    [SerializeField] public MovementType movementType;
    [SerializeField] public float friction;
    [SerializeField] public float aerialControl;
    [SerializeField] [Range(0, 90)] public float slopeTolerance;
    [SerializeField] [Range(0, 1)] public float feetRadius;

    [HideInInspector] public bool isTouchingGround;
    [HideInInspector] public float H;
    [HideInInspector] public float V;

    private Transform trans;
    private Rigidbody2D rb;
    private BoxCollider2D col;
    private PhysicsMaterial2D baseMaterial;
    private PhysicsMaterial2D airMaterial;

    private bool hasJumped;
    private RaycastHit2D neargroundCheck;

    public Vector2 Motion { get; set; }

    List<ContactPoint2D> groundContacts;
    float ang;

    public void ApplyJump(float height) {
        rb.velocity = new Vector2(0, height);
        hasJumped = true;
    }


    void Start () {
        trans = GetComponent<Transform>();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<BoxCollider2D>();

        baseMaterial = Resources.Load("Mat_CharGround") as PhysicsMaterial2D;
        airMaterial = Resources.Load("Mat_CharAir") as PhysicsMaterial2D;
    }
	

	void FixedUpdate () {
        groundContacts = CheckGround();

        neargroundCheck = Physics2D.BoxCast(new Vector2(trans.position.x, col.bounds.min.y + col.size.y * (feetRadius / 2)), new Vector2(col.size.x, col.size.y * feetRadius), 0f, Vector2.down, 0.5f);
        bool snapToGround = false;

        if (groundContacts.Count > 0) {
            ang = Vector2.SignedAngle(transform.up, groundContacts[0].normal);

            if (!isTouchingGround) {
                rb.velocity = Vector2.zero;
            }

            isTouchingGround = true;
        }
        else {
            ang = 0;

            if (isTouchingGround) {
                if (neargroundCheck && Vector2.Angle(transform.up, neargroundCheck.normal) < slopeTolerance && !hasJumped) {
                    snapToGround = true;
                }
                else {
                    hasJumped = false;
                    isTouchingGround = false;
                }

                H = rb.velocity.x + Motion.x;
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

            col.sharedMaterial = baseMaterial;

            H = force * Mathf.Cos(Mathf.Deg2Rad * ang);
            V = force * Mathf.Sin(Mathf.Deg2Rad * ang);

            Vector2 move = new Vector2(H, V);

            Vector2 cornerPoint = new Vector2(rb.position.x + Mathf.Sign(force) * (col.size.x / 2), col.bounds.min.y);
            RaycastHit2D sideCheck = Physics2D.Linecast(cornerPoint, move * Time.deltaTime + cornerPoint);
            
            if (sideCheck && Vector2.Angle(transform.up, sideCheck.normal) > slopeTolerance) {

                float distanceA = col.bounds.SqrDistance(new Vector2(sideCheck.point.x - Mathf.Sign(force) * 0.02f, sideCheck.point.y));
                float distanceB = col.bounds.SqrDistance(move * Time.deltaTime + cornerPoint);
                float percentdiff = distanceA / distanceB;
                Debug.DrawLine(sideCheck.point, move * Time.deltaTime + cornerPoint);
                Debug.Log(percentdiff);
                move *= Mathf.Max(percentdiff, 0);
            }

            rb.position += move * Time.deltaTime;
        }
    }

    private void MoveOffGround(Vector2 force) {

        col.sharedMaterial = airMaterial;

        RaycastHit2D sideCheck = Physics2D.BoxCast(trans.position, new Vector2(col.size.x, col.size.y), 0f, Vector2.right * Mathf.Sign(H), Mathf.Infinity);

        Vector2 minSpd = -force;
        Vector2 maxSpd = force;
        float addedSpd = Motion.x * Time.deltaTime * aerialControl;

        if (addedSpd + H > maxSpd.x)
            addedSpd = maxSpd.x - H;
        else if (addedSpd + H < minSpd.x)
            addedSpd = minSpd.x - H;

        if ((H < maxSpd.x && force.x > 0) || (H > minSpd.x && force.x < 0))
            H += addedSpd;

        if (sideCheck && col.IsTouching(sideCheck.collider))
            H = 0;

        V = rb.velocity.y;

        Vector2 move = new Vector2(H, V);

        rb.velocity = move;
    }

    private List<ContactPoint2D> CheckGround() {
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
}
