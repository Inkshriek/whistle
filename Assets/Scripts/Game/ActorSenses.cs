using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Whistle.Actors;

public class ActorSenses : MonoBehaviour {

    //This script can be read by other scripts to get information about what an actor (or some other entity) is "seeing" and "hearing".
    //This is most important to AI. Use it wisely.
    
    [SerializeField] private int visionRange;
    [SerializeField] private int visionDirection;
    [SerializeField] private float visionRadius;
    [SerializeField] private float hearingRadius;
    public BoxCollider2D target;

    private Transform trans;
    private Transform targettrans;

    public bool TargetVisible { get; private set; } //TODO: replace this with an event thing
    public int VisionRange {
        get {
            return visionRange;
        }
        set {
            value = Mathf.Clamp(value, 0, 180);
            visionRange = value;
        }
    }
    public int VisionDirection {
        get {
            return visionDirection;
        }
        set {
            value = value % 360;
            visionDirection = value;
        }
    }
    public float VisionRadius {
        get {
            return visionRadius;
        }
        set {
            value = Mathf.Clamp(value, 0f, 1000f);
            visionRadius = value;
        }
    }
    public float HearingRadius {
        get {
            return hearingRadius;
        }
        set {
            value = Mathf.Clamp(value, 0f, 1000f);
            hearingRadius = value;
        }
    }
    private Vector2 TargetPosition {
        get {
            return (Vector2)targettrans.localPosition + target.offset;
        }
    }
    private Vector2 ThisPosition {
        get {
            return trans.localPosition;
        }
    }

    private void Awake() {
        trans = GetComponent<Transform>();
        targettrans = target.gameObject.GetComponent<Transform>();
    }

    private void Update () {
        if (CheckVisionWithinRange() && CheckVisionObstacles()) {
            TargetVisible = true;
        }
        else {
            TargetVisible = false;
        }

    }

    private bool CheckVisionWithinRange() {
        Vector2 visionAngle = new Vector2(Mathf.Cos(Mathf.Deg2Rad * visionDirection), Mathf.Sin(Mathf.Deg2Rad * visionDirection));
        float angleDifference = (Vector2.Angle(TargetPosition - ThisPosition, visionAngle));
        float distance = Mathf.Abs(Vector2.Distance(ThisPosition, TargetPosition));

        if ((angleDifference <= (visionRange / 2)) && distance <= visionRadius) {
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

    public void FlipVision() {
        VisionDirection += 180;
    }
}
