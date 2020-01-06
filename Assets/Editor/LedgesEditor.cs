using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Ledges))]
public class LedgesEditor : Editor {

    Ledges _script;

    private bool _safeToDraw;

    private void OnEnable() {
        _safeToDraw = false;
    }

    public override void OnInspectorGUI() {
        serializedObject.Update();
        _script = target as Ledges;

        _script.LeftLedge = EditorGUILayout.Vector2Field("Left Ledge", _script.LeftLedge);
        _script.RightLedge = EditorGUILayout.Vector2Field("Right Ledge", _script.RightLedge);
        EditorGUILayout.Space();
        _script.Perimeter = EditorGUILayout.FloatField("Perimeter", _script.Perimeter);

        _safeToDraw = true;
        serializedObject.ApplyModifiedProperties();
        SceneView.lastActiveSceneView.Repaint();
    }

    public void OnSceneGUI() {

        if (_safeToDraw) {
            EditorGUI.BeginChangeCheck();

            _script.LeftLedge = Handles.Slider2D(2, _script.LeftLedge, _script.transform.localPosition, Vector3.forward, Vector2.right, Vector2.up, 0.075f, Handles.DotHandleCap, new Vector2(0.01f, 0.01f));
            _script.RightLedge = Handles.Slider2D(1, _script.RightLedge, _script.transform.localPosition, Vector3.forward, Vector2.right, Vector2.up, 0.075f, Handles.DotHandleCap, new Vector2(0.01f, 0.01f));

            Vector3 adjustedLeftLedge = _script.transform.localPosition + (Vector3)_script.LeftLedge;
            Vector3 adjustedRightLedge = _script.transform.localPosition + (Vector3)_script.RightLedge;

            Vector3[] leftRect = new Vector3[] {
                new Vector3(adjustedLeftLedge.x - _script.Perimeter, adjustedLeftLedge.y + _script.Perimeter, adjustedLeftLedge.z),
                new Vector3(adjustedLeftLedge.x + _script.Perimeter, adjustedLeftLedge.y + _script.Perimeter, adjustedLeftLedge.z),
                new Vector3(adjustedLeftLedge.x + _script.Perimeter, adjustedLeftLedge.y - _script.Perimeter, adjustedLeftLedge.z),
                new Vector3(adjustedLeftLedge.x - _script.Perimeter, adjustedLeftLedge.y - _script.Perimeter, adjustedLeftLedge.z)
            };
            Vector3[] rightRect = new Vector3[] {
                new Vector3(adjustedRightLedge.x - _script.Perimeter, adjustedRightLedge.y + _script.Perimeter, adjustedRightLedge.z),
                new Vector3(adjustedRightLedge.x + _script.Perimeter, adjustedRightLedge.y + _script.Perimeter, adjustedRightLedge.z),
                new Vector3(adjustedRightLedge.x + _script.Perimeter, adjustedRightLedge.y - _script.Perimeter, adjustedRightLedge.z),
                new Vector3(adjustedRightLedge.x - _script.Perimeter, adjustedRightLedge.y - _script.Perimeter, adjustedRightLedge.z)
            };

            Handles.color = new Color(0.2f, 0.4f, 1f);
            Handles.DrawWireDisc(adjustedLeftLedge, new Vector3(0, 0, 1), _script.Perimeter);
            Handles.color = new Color(0.8f, 0.2f, 0.2f);
            Handles.DrawWireDisc(adjustedRightLedge, new Vector3(0, 0, 1), _script.Perimeter);
        }
        else {
            SceneView.RepaintAll();
        }
    }
}