using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ActorSenses))]
public class ActorSensesEditor : Editor {

    private ActorSenses script;
    private bool safeToDraw;

    private void OnEnable() {
        safeToDraw = false;
    }

    public override void OnInspectorGUI() {
        serializedObject.Update();
        script = target as ActorSenses;

        script.VisionRange = EditorGUILayout.IntSlider("Vision Range", script.VisionRange, 0, 180);
        script.VisionDirection = EditorGUILayout.IntSlider("Vision Direction", script.VisionDirection, 0, 359);
        script.VisionRadius = EditorGUILayout.FloatField("Vision Radius", script.VisionRadius);
        script.HearingRadius = EditorGUILayout.FloatField("Hearing Radius", script.HearingRadius);
        EditorGUILayout.Space();
        script.target = (BoxCollider2D)EditorGUILayout.ObjectField("Target", script.target, typeof(BoxCollider2D), true);
        if (GUI.changed) {
            Undo.RegisterCompleteObjectUndo(script, "Actor Senses Change");
        }

        safeToDraw = true;
        serializedObject.ApplyModifiedProperties();
        SceneView.lastActiveSceneView.Repaint();
    }

    public void OnSceneGUI() {

        if (safeToDraw) {
            EditorGUI.BeginChangeCheck();

            Vector2 angleUpper = new Vector2(Mathf.Cos(Mathf.Deg2Rad * (script.VisionDirection + script.VisionRange / 2)), Mathf.Sin(Mathf.Deg2Rad * (script.VisionDirection + script.VisionRange / 2)));
            Vector2 angleLower = new Vector2(Mathf.Cos(Mathf.Deg2Rad * (script.VisionDirection - script.VisionRange / 2)), Mathf.Sin(Mathf.Deg2Rad * (script.VisionDirection - script.VisionRange / 2)));

            Handles.color = Color.yellow;
            Handles.DrawDottedLine(script.transform.localPosition, (Vector3)angleUpper * script.VisionRadius + script.transform.localPosition, 2f);
            Handles.DrawDottedLine(script.transform.localPosition, (Vector3)angleLower * script.VisionRadius + script.transform.localPosition, 2f);
            Handles.DrawWireArc(script.transform.localPosition, new Vector3(0,0,1), angleLower, script.VisionRange, script.VisionRadius);

            Handles.color = Color.white;
            Handles.DrawWireDisc(script.transform.localPosition, new Vector3(0, 0, 1), script.HearingRadius);
        }
        else {
            SceneView.RepaintAll();
        }
    }
}