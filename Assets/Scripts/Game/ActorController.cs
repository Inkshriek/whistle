using System.Collections.Generic;
using UnityEngine;
using Whistle.Actors;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class ActorController : MonoBehaviour {

    [SerializeField] public MovementType MovementType;
    [SerializeField] public float Friction;
    [SerializeField] public float AerialControl;
    [SerializeField] [Range(0, 90)] public float SlopeTolerance;
    [SerializeField] [Range(0, 1)] public float SnapdownRadius;
    [SerializeField] public int HorizontalRays;
    [SerializeField] public int VerticalRays;

    private Rigidbody2D _rb;
    private BoxCollider2D _col;
    private PhysicsMaterial2D _baseMaterial;
    private PhysicsMaterial2D _airMaterial;

    private bool _hasJumped;
    private int _layerMask;
    private RaycastHit2D _neargroundCheck;
    private float _forceSmoothed;

    public Vector2 Motion { get; set; }
    public bool IsTouchingGround { get; private set; }
    public float H { get; private set; }
    public float V { get; private set; }

    private List<ContactPoint2D> _validContacts;
    private float _ang;

    public void ApplyJump(float height) {
        _rb.velocity += new Vector2(0, height);
        _hasJumped = true;
    }


    void Start () {
        _rb = GetComponent<Rigidbody2D>();
        _col = GetComponent<BoxCollider2D>();

        _layerMask = 1;

        _baseMaterial = Resources.Load("Mat_ActorGround") as PhysicsMaterial2D;
        _airMaterial = Resources.Load("Mat_ActorAir") as PhysicsMaterial2D;
    }
	

	void FixedUpdate () {
        _validContacts = CheckGroundContacts();

        bool snapToGround = false;

        if (_validContacts.Count > 0) {
            _ang = Vector2.SignedAngle(transform.up, _validContacts[0].normal);

            if (!IsTouchingGround) {
                _forceSmoothed = _rb.velocity.x;
                _rb.velocity = Vector2.zero;
            }

            IsTouchingGround = true;
        }
        else {
            _ang = 0;

            if (IsTouchingGround) {
                _neargroundCheck = CheckColliders(VerticalRays, Vector2.down * SnapdownRadius, new Vector2(_col.bounds.min.x, _col.bounds.min.y), new Vector2(_col.bounds.max.x, _col.bounds.min.y));
                if (_neargroundCheck && Vector2.Angle(transform.up, _neargroundCheck.normal) < SlopeTolerance && !_hasJumped) {
                    snapToGround = true;
                }
                else {
                    _hasJumped = false;
                    IsTouchingGround = false;
                    _rb.velocity += new Vector2(_forceSmoothed, 0);
                }
            }
        }

        switch (MovementType) {
            case MovementType.Normal:
                if (IsTouchingGround) {
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
            _rb.position += new Vector2(_forceSmoothed * Time.deltaTime, _neargroundCheck.point.y - _col.bounds.min.y);
        }
        else {

            if (_forceSmoothed < force) {
                _forceSmoothed = Mathf.Min(_forceSmoothed + Friction, force);
            }
            else if (_forceSmoothed > force) {
                _forceSmoothed = Mathf.Max(_forceSmoothed - Friction, force);
            }

            _col.sharedMaterial = _baseMaterial;

            H = _forceSmoothed * Mathf.Cos(Mathf.Deg2Rad * _ang);
            V = _forceSmoothed * Mathf.Sin(Mathf.Deg2Rad * _ang);

            Vector2 move = new Vector2(H, V);

            

            Vector2 cornerPoint = new Vector2(_rb.position.x + Mathf.Sign(_forceSmoothed) * (_col.size.x / 2), _col.bounds.min.y);
            //RaycastHit2D sideCheck = Physics2D.Linecast(cornerPoint, move * Time.deltaTime + cornerPoint, layerMask);

            RaycastHit2D sideCheck = CheckColliders(HorizontalRays, move * Time.deltaTime, new Vector2(_rb.position.x + Mathf.Sign(_forceSmoothed) * (_col.size.x / 2), _col.bounds.min.y), new Vector2(_rb.position.x + Mathf.Sign(_forceSmoothed) * (_col.size.x / 2), _col.bounds.max.y));
            
            if (sideCheck && Vector2.Angle(transform.up, sideCheck.normal) > SlopeTolerance) {
                float distanceA = _col.bounds.SqrDistance(new Vector2(sideCheck.point.x - Mathf.Sign(_forceSmoothed) * 0.02f, sideCheck.point.y));
                float distanceB = _col.bounds.SqrDistance(move * Time.deltaTime + cornerPoint);
                float percentdiff = distanceA / distanceB;
                move *= Mathf.Max(percentdiff, 0);
                _forceSmoothed = 0;
            }

            _rb.position += move * Time.deltaTime;
        }
    }

    private void MoveOffGround(Vector2 force) {

        _col.sharedMaterial = _airMaterial;

        if (_rb.velocity.x < force.x && force.x > 0) {
            H = Mathf.Min(AerialControl, force.x - _rb.velocity.x);
        }
        else if (_rb.velocity.x > force.x && force.x < 0) {
            H = Mathf.Max(-AerialControl, force.x - _rb.velocity.x);
        }
        else {
            H = 0;
        }

        V = 0;


        Vector2 move = new Vector2(H, V);


        _rb.velocity += move;
    }

    private List<ContactPoint2D> CheckGroundContacts() {
        ContactPoint2D[] contactsAll = new ContactPoint2D[20];
        List<ContactPoint2D> contacts = new List<ContactPoint2D>();
        int contactsNum = _col.GetContacts(contactsAll);
        for (int i = 0; i < contactsNum; i++) {
            float ang = Vector2.Angle(transform.up, contactsAll[i].normal);
            if (ang < SlopeTolerance) {
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
            RaycastHit2D ray = Physics2D.Linecast(originPoint, direction + originPoint, _layerMask);

            if (ray) {
                float rayDistance = _col.bounds.SqrDistance(ray.point);
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
