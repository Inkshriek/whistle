using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    [SerializeField] public Transform objectToFollow;
    [SerializeField] public Vector2 minConstraints;
    [SerializeField] public Vector2 maxConstraints;
    [SerializeField] public bool following;
    [SerializeField] public bool constrained;
	
	// Update is called once per frame
	void Update () {
        if (following && objectToFollow != null) {
            transform.position = new Vector3(objectToFollow.transform.position.x, objectToFollow.transform.position.y, transform.position.z);
        }
		if (constrained) {
            transform.position = new Vector3(Mathf.Clamp(transform.position.x, minConstraints.x, maxConstraints.x), Mathf.Clamp(transform.position.y, minConstraints.y, maxConstraints.y), transform.position.z);
        }
	}
}
