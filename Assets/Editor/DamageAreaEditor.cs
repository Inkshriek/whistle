using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DamageArea))]
public class DamageAreaEditor : Editor {

    private DamageArea script;

    private void OnEnable() {
        script = target as DamageArea;
    }

    public void OnSceneGUI() {

        EditorGUI.BeginChangeCheck();

        //switch (script.type)
        Rect area = new Rect(script.transform.position.x - script.size.x / 2, script.transform.position.y - script.size.y / 2, script.size.x, script.size.y);

        Handles.DrawSolidRectangleWithOutline(area, Color.clear, Color.red);
    }
}