using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Whistle.Actors;

public class ActorSenses : MonoBehaviour {

    //This script can be read by other scripts to get information about what an actor (or some other entity) is "seeing" and "hearing".
    //This is most important to AI. Use it wisely.

    [SerializeField] [Range(0, 180)] public int VisionRadius;
    [SerializeField] [Range(0, 359)] public int VisionDirection;
    [SerializeField] public float VisionRange;
    [SerializeField] public float HearingRange;
    [SerializeField] public BoxCollider2D Target;

    private Transform _transform;
    private Transform _targettransform;

    public bool TargetVisible { get; private set; } //TODO: replace this with an event thing
    private Vector2 TargetPosition {
        get {
            return (Vector2)_targettransform.localPosition + Target.offset;
        }
    }
    private Vector2 ThisPosition {
        get {
            return _transform.localPosition;
        }
    }

    private void Awake() {
        _transform = GetComponent<Transform>();
        _targettransform = Target.gameObject.GetComponent<Transform>();
    }

    private void Update () {
        if (CheckVisionWithinRange() && CheckVisionObstacles()) {
            TargetVisible = true;
        }
        else {
            TargetVisible = false;
        }
        DrawCone();
    }

    private bool CheckVisionWithinRange() {
        Vector2 visionAngle = new Vector2(Mathf.Cos(Mathf.Deg2Rad * VisionDirection), Mathf.Sin(Mathf.Deg2Rad * VisionDirection));
        float angleDifference = (Vector2.Angle(TargetPosition - ThisPosition, visionAngle));
        float distance = Mathf.Abs(Vector2.Distance(ThisPosition, TargetPosition));

        if ((angleDifference <= (VisionRadius / 2)) && distance <= VisionRange) {
            return true;
        }
        else {
            return false;
        }
    }

    private bool CheckVisionObstacles() {
        RaycastHit2D check = Physics2D.Linecast(ThisPosition, TargetPosition);
        if (check) {
            return true;
        }
        else {
            return false;
        }
    }

    private void DrawCone() {
        Vector2 angleUpper = new Vector2(Mathf.Cos(Mathf.Deg2Rad * (VisionDirection + VisionRadius / 2)), Mathf.Sin(Mathf.Deg2Rad * (VisionDirection + VisionRadius / 2)));
        Vector2 angleLower = new Vector2(Mathf.Cos(Mathf.Deg2Rad * (VisionDirection - VisionRadius / 2)), Mathf.Sin(Mathf.Deg2Rad * (VisionDirection - VisionRadius / 2)));
        Debug.DrawRay(ThisPosition, angleUpper * VisionRange, Color.cyan);
        Debug.DrawRay(ThisPosition, angleLower * VisionRange, Color.cyan);

        Debug.DrawRay(ThisPosition, new Vector2(Mathf.Cos(Mathf.Deg2Rad * VisionDirection), Mathf.Sin(Mathf.Deg2Rad * VisionDirection)) * VisionRange, Color.cyan);
    }

    public void FlipVision() { 
    }
}
