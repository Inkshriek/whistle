using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Whistle.Actors;

public class DamageArea : MonoBehaviour {

    //A DamageBox is simply a 2D invisible box that hurts things. It will trigger the Damage method of entities with IHealth on collision.
    //Each one has a limited lifespan and will delete itself afterwards, unless "lifespan" is set to less than 0 (in which it will be alive forever).

    public Vector2 size;
    public float value;
    public DamageType type;
    public bool continuous;
    public string target;
    public float lifespan;

    //The value is applied once if continuous is set to false, or repeatedly each second otherwise (relative to Time.deltaTime)
    //The tag is also checked to ensure the ideal target is damaged, though you may make it blank to target everything.

    public static DamageArea Spawn(Vector2 position, string ID) {
        //You should make DamageBoxes be prefabs in the Resources folder. This will pull the one being called and instantiate it.
        DamageArea box = Instantiate(Resources.Load("Attacks/" + ID, typeof(DamageArea)) as DamageArea);
        if (box == null) {
            Debug.LogWarning("Some class tried to instantiate a DamageBox that doesn't exist! (" + ID + ")");
        }
        else {
            box.transform.position += new Vector3(position.x, position.y, 0);
        }
        return box;
    }

	private void Update () {
        DrawBox();

        bool targetFound = false;
        RaycastHit2D[] check = Physics2D.BoxCastAll(transform.position, size, 0, Vector2.zero);
        foreach (RaycastHit2D find in check) {
            IHealth obj = find.collider.GetComponent<IHealth>();
            if ((find.collider.tag == target.Trim() || target == null || target.Trim() == "") && obj != null) {
                if (continuous) {
                    obj.Damage(value * (Mathf.Min(Time.deltaTime, lifespan)), type);
                }
                else {
                    obj.Damage(value, type);
                }
                targetFound = true;
            }
        }

        if (lifespan >= 0 && !continuous && targetFound == true) {
            Destroy(gameObject);
        }

        if (lifespan >= 0) {
            lifespan = Mathf.Max(lifespan - Time.deltaTime, 0);
            if (lifespan == 0) {
                Destroy(gameObject);
            }
        }
        else {
            //Lifespan is negative, so nothing will be done.
        }
	}

    private void DrawBox() {
        Vector3 boxLocation = transform.position;
        Debug.DrawLine(new Vector3(boxLocation.x - size.x / 2, boxLocation.y + size.y / 2, boxLocation.z), new Vector3(boxLocation.x + size.x / 2, boxLocation.y + size.y / 2, boxLocation.z), Color.red, 0.0f, false);
        Debug.DrawLine(new Vector3(boxLocation.x + size.x / 2, boxLocation.y + size.y / 2, boxLocation.z), new Vector3(boxLocation.x + size.x / 2, boxLocation.y - size.y / 2, boxLocation.z), Color.red, 0.0f, false);
        Debug.DrawLine(new Vector3(boxLocation.x + size.x / 2, boxLocation.y - size.y / 2, boxLocation.z), new Vector3(boxLocation.x - size.x / 2, boxLocation.y - size.y / 2, boxLocation.z), Color.red, 0.0f, false);
        Debug.DrawLine(new Vector3(boxLocation.x - size.x / 2, boxLocation.y - size.y / 2, boxLocation.z), new Vector3(boxLocation.x - size.x / 2, boxLocation.y + size.y / 2, boxLocation.z), Color.red, 0.0f, false);
    }
}
