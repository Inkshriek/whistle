using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Whistle.Actors;

public class DamageBox : MonoBehaviour {

    //A DamageBox is simply a 2D invisible box that's spawned in when instantiated. It will trigger the Damage method of entities with IHealth on collision.
    //Each one has a limited lifespan and will delete itself afterwards, unless Lifespan is set to less than 0 (in which it will be alive forever).
    //Instantiate one with Create() whenever you need to script an attack.
    //You may also attach this to a GameObject to create the effect of a damaging area.

    public Vector2 Size;
    public Vector2 Offset;
    public float Value;
    public bool Continuous;
    public DamageType Type;
    public string Tag;

    [HideInInspector] public float Lifespan = -1;

    //The value is applied once if continuous is set to false, or repeatedly each second otherwise (relative to Time.deltaTime)
    //The tag is also checked to ensure the ideal target is damaged, though you may make it blank to target everything.

    public static GameObject Create(Transform parent, Vector2 origin, Vector2 size, float value, float lifespan, bool continuous, DamageType type, string tag) {
        GameObject obj = new GameObject();
        obj.transform.SetParent(parent);
        obj.transform.localPosition = origin;
        obj.name = "DamageBox!";
        DamageBox box = obj.AddComponent<DamageBox>();

        box.Size = size;
        box.Offset = Vector2.zero;
        box.Value = value;
        box.Lifespan = lifespan;
        box.Continuous = continuous;
        box.Type = type;
        box.Tag = tag;

        Debug.Log("New DamageBox created at " + origin);
        return obj;
    }

	private void Update () {
        DrawBox();

        bool targetFound = false;
        RaycastHit2D[] check = Physics2D.BoxCastAll((Vector2)transform.position + Offset, Size, 0, Vector2.zero);
        foreach (RaycastHit2D find in check) {
            IHealth obj = find.collider.GetComponent<IHealth>();
            if ((find.collider.tag == Tag.Trim() || Tag == null || Tag.Trim() == "") && obj != null) {
                if (Continuous) {
                    obj.Damage(Value * (Mathf.Min(Time.deltaTime, Lifespan)), Type);
                }
                else {
                    obj.Damage(Value, Type);
                }
                targetFound = true;
            }
        }

        if (Lifespan >= 0 && !Continuous && targetFound == true) {
            Destroy(gameObject);
        }

        if (Lifespan >= 0) {
            Lifespan = Mathf.Max(Lifespan - Time.deltaTime, 0);
            if (Lifespan == 0) {
                Destroy(gameObject);
            }
        }
        else {
            //Lifespan is negative, so nothing will be done.
        }
	}

    private void DrawBox() {
        Vector3 boxLocation = transform.position + (Vector3)Offset;
        Debug.DrawLine(new Vector3(boxLocation.x - Size.x / 2, boxLocation.y + Size.y / 2, boxLocation.z), new Vector3(boxLocation.x + Size.x / 2, boxLocation.y + Size.y / 2, boxLocation.z), Color.red, 0.0f, false);
        Debug.DrawLine(new Vector3(boxLocation.x + Size.x / 2, boxLocation.y + Size.y / 2, boxLocation.z), new Vector3(boxLocation.x + Size.x / 2, boxLocation.y - Size.y / 2, boxLocation.z), Color.red, 0.0f, false);
        Debug.DrawLine(new Vector3(boxLocation.x + Size.x / 2, boxLocation.y - Size.y / 2, boxLocation.z), new Vector3(boxLocation.x - Size.x / 2, boxLocation.y - Size.y / 2, boxLocation.z), Color.red, 0.0f, false);
        Debug.DrawLine(new Vector3(boxLocation.x - Size.x / 2, boxLocation.y - Size.y / 2, boxLocation.z), new Vector3(boxLocation.x - Size.x / 2, boxLocation.y + Size.y / 2, boxLocation.z), Color.red, 0.0f, false);
    }
}
