﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Whistle.Actors;
using Whistle.Conditions;

public class GiveDebuff : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter2D(Collider2D other) {
        string tag = other.gameObject.tag;
        switch (tag) {
            case "Player":
                other.gameObject.GetComponent<IConditions>().AddCond(new Poison(10));
                break;
            case "Monster":
                other.gameObject.GetComponent<IConditions>().AddCond(new Poison(5));
                break;
            default:
                Debug.Log("Well that's a fine howdy-do!");
                break;
        }
    }
}
