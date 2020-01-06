using UnityEngine;

public class Robot : MonoBehaviour {
	// Use this for initialization
	void Start () {
		Animator animator = GetComponent<Animator>();
		animator.SetFloat("Offset", Random.Range(0f, 1.0f));
	}
}
