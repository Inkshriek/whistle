using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Whistle.Conditions;

public class GiveBuff : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.CompareTag("Player")) {
            other.gameObject.GetComponent<PlayerController>().AddCond(new SpeedUp(10, true));
        }
    }
}
