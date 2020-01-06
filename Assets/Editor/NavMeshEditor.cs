using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(NavMesh))]
public class NavMeshEditor : Editor {

    //This whole thing here is used for displaying the NavMesh script all nicely in the Scene and Inspector. Don't touch it!!

    private NavMesh script;
    private bool safeToDraw;

    private void OnEnable() {
        safeToDraw = false;
    }
    
    public override void OnInspectorGUI() {
        serializedObject.Update();
        script = target as NavMesh;

        //EditorGUILayout.HelpBox("The navigation mesh has not been baked since the last update!", MessageType.Warning);

        float nodeSpacing = EditorGUILayout.FloatField("Node Spacing", script.nodeSpacing);
        BoundsInt nodeBounds = EditorGUILayout.BoundsIntField("Bounds", script.nodeBounds);
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        /*
        if (GUILayout.Button("Bake", GUILayout.Height(20), GUILayout.Width(250))) {
            
            _script.BakeGraph();
            EditorUtility.SetDirty(_script);
        }
        */
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        EditorGUILayout.Space();

        EditorGUI.indentLevel += 1;
        for (int i = 0; i < script.mesh.Length; i++) {
            script.mesh[i].display = EditorGUILayout.Foldout(script.mesh[i].display, script.mesh[i].name);
            if (script.mesh[i].display) {
                script.mesh[i].name = EditorGUILayout.TextField(script.mesh[i].name);
                EditorGUILayout.Space();
                script.mesh[i].flag = (NavMesh.NavFlag)EditorGUILayout.EnumPopup("Flag", script.mesh[i].flag);
                EditorGUILayout.Space();
                script.mesh[i].a = EditorGUILayout.Vector2Field("Point A", script.mesh[i].a);
                script.mesh[i].b = EditorGUILayout.Vector2Field("Point B", script.mesh[i].b);
                script.mesh[i].c = EditorGUILayout.Vector2Field("Point C", script.mesh[i].c);
                script.mesh[i].d = EditorGUILayout.Vector2Field("Point D", script.mesh[i].d);

                EditorGUILayout.Space();
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Delete", GUILayout.Width(150))) {
                    NavMesh.NavRect[] newRectList = new NavMesh.NavRect[script.mesh.Length - 1];
                    for (int e = 0; e < newRectList.Length; e++) {
                        if (e >= i) {
                            newRectList[e] = script.mesh[e + 1];
                        }
                        else {
                            newRectList[e] = script.mesh[e];
                        }
                        
                    }
                    script.mesh = newRectList;
                }

                GUILayout.EndHorizontal();

                EditorGUILayout.Space();
            }
        }

        EditorGUI.indentLevel -= 1;
        EditorGUILayout.Space();

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("New Rectangle", GUILayout.Height(30), GUILayout.Width(250))) {
            NavMesh.NavRect[] newRectList = new NavMesh.NavRect[script.mesh.Length + 1];
            for (int i = 0; i < newRectList.Length; i++) {
                if (i == script.mesh.Length) {
                    newRectList[i] = new NavMesh.NavRect("Rect " + i, true, NavMesh.NavFlag.Normal, new Vector2 (-1, 1), new Vector2(1, 1), new Vector2(-1, -1), new Vector2(1, -1));
                }
                else {
                    newRectList[i] = script.mesh[i];
                }
            }
            script.mesh = newRectList;
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        if (GUI.changed) {
            Undo.RegisterCompleteObjectUndo(script, "Mesh Change");
            script.nodeBounds = nodeBounds;
            script.nodeSpacing = nodeSpacing;
        }

        safeToDraw = true;
        serializedObject.ApplyModifiedProperties();
        SceneView.lastActiveSceneView.Repaint();
    }

    public void OnSceneGUI() {

        if (safeToDraw) {
            Vector3[] limits = {
                    script.nodeBounds.min,
                    new Vector2(script.nodeBounds.max.x, script.nodeBounds.min.y),
                    script.nodeBounds.max,
                    new Vector2(script.nodeBounds.min.x, script.nodeBounds.max.y)
                };
            Handles.DrawSolidRectangleWithOutline(limits, Color.clear, Color.yellow);
            for (int i = 0; i < script.mesh.Length; i++) {

                Vector3[] points = {
                    script.mesh[i].a,
                    script.mesh[i].b,
                    script.mesh[i].d,
                    script.mesh[i].c
                };

                EditorGUI.BeginChangeCheck();

                switch (script.mesh[i].flag) {
                    case NavMesh.NavFlag.Normal:
                        Handles.DrawSolidRectangleWithOutline(points, new Color(0.5f, 0.5f, 0.5f, 0.2f), Color.white);
                        break;
                    case NavMesh.NavFlag.Transient:
                        Handles.DrawSolidRectangleWithOutline(points, new Color(0.5f, 0.3f, 0.7f, 0.2f), Color.blue);
                        break;
                    case NavMesh.NavFlag.Flight:
                        Handles.DrawSolidRectangleWithOutline(points, new Color(0.2f, 0.8f, 0.7f, 0.2f), Color.green);
                        break;
                    case NavMesh.NavFlag.Dangerous:
                        Handles.DrawSolidRectangleWithOutline(points, new Color(0.8f, 0.4f, 0.4f, 0.2f), Color.red);
                        break;
                    case NavMesh.NavFlag.Nothing:
                        Handles.DrawSolidRectangleWithOutline(points, new Color(0.2f, 0.2f, 0.2f, 0.2f), Color.black);
                        break;
                    default:
                        Handles.DrawSolidRectangleWithOutline(points, new Color(0.5f, 0.5f, 0.5f, 0.2f), Color.white);
                        break;
                }

                if (script.mesh[i].flag != NavMesh.NavFlag.Nothing) {
                    script.mesh[i].a = Handles.Slider2D(script.mesh[i].a, Vector3.forward, Vector2.right, Vector2.up, 0.1f, Handles.DotHandleCap, 0.1f);
                    script.mesh[i].b = Handles.Slider2D(script.mesh[i].b, Vector3.forward, Vector2.right, Vector2.up, 0.1f, Handles.DotHandleCap, 0.1f);
                    script.mesh[i].c = Handles.Slider2D(script.mesh[i].c, Vector3.forward, Vector2.right, Vector2.up, 0.1f, Handles.DotHandleCap, 0.1f);
                    script.mesh[i].d = Handles.Slider2D(script.mesh[i].d, Vector3.forward, Vector2.right, Vector2.up, 0.1f, Handles.DotHandleCap, 0.1f);

                    script.mesh[i].Position = Handles.Slider2D(script.mesh[i].Position, Vector3.forward, Vector2.right, Vector2.up, 0.1f, Handles.DotHandleCap, 0.1f);
                }

                if (EditorGUI.EndChangeCheck()) {
                    script.mesh[i].display = true;
                }
            }
        }
        else {
            SceneView.RepaintAll();
        }

        
    }
}
