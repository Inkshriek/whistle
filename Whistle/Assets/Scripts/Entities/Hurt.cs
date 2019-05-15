using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Whistle.Actors;

public class Hurt : MonoBehaviour {

    public float amount;

    private void OnTriggerEnter2D(Collider2D collision) {
        IHealth thing = collision.gameObject.GetComponent<IHealth>();
        if (thing != null) {
            thing.Damage(amount);
            Debug.Log("Damaged someone!");
        }
    }
}
