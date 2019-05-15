using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(NavMesh))]
public class NavMeshEditor : Editor {

    //This whole thing here is used for displaying the NavMesh script all nicely in the Scene and Inspector. Don't touch it!!

    NavMesh meshScript;
    SerializedProperty nodeSpacing;
    SerializedProperty nodeBounds;

    private bool safeToDraw;

    private void OnEnable() {

        safeToDraw = false;
    }
    
    public override void OnInspectorGUI() {
        serializedObject.Update();
        meshScript = target as NavMesh;

        EditorGUI.indentLevel += 1;
        for (int i = 0; i < meshScript.mesh.Length; i++) {
            meshScript.mesh[i].display = EditorGUILayout.Foldout(meshScript.mesh[i].display, meshScript.mesh[i].name);
            if (meshScript.mesh[i].display) {
                meshScript.mesh[i].name = EditorGUILayout.TextField(meshScript.mesh[i].name);
                EditorGUILayout.Space();
                meshScript.mesh[i].flag = (NavMesh.NavRectFlag)EditorGUILayout.EnumPopup("Flag", meshScript.mesh[i].flag);
                EditorGUILayout.Space();
                meshScript.mesh[i].a = EditorGUILayout.Vector2Field("Point A", meshScript.mesh[i].a);
                meshScript.mesh[i].b = EditorGUILayout.Vector2Field("Point B", meshScript.mesh[i].b);
                meshScript.mesh[i].c = EditorGUILayout.Vector2Field("Point C", meshScript.mesh[i].c);
                meshScript.mesh[i].d = EditorGUILayout.Vector2Field("Point D", meshScript.mesh[i].d);

                EditorGUILayout.Space();
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Delete", GUILayout.Width(150))) {
                    NavMesh.NavRect[] newRectList = new NavMesh.NavRect[meshScript.mesh.Length - 1];
                    for (int e = 0; e < newRectList.Length; e++) {
                        if (e >= i) {
                            newRectList[e] = meshScript.mesh[e + 1];
                        }
                        else {
                            newRectList[e] = meshScript.mesh[e];
                        }
                        
                    }
                    meshScript.mesh = newRectList;
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
            NavMesh.NavRect[] newRectList = new NavMesh.NavRect[meshScript.mesh.Length + 1];
            for (int i = 0; i < newRectList.Length; i++) {
                if (i == meshScript.mesh.Length) {
                    newRectList[i] = new NavMesh.NavRect("Rect " + i, true, NavMesh.NavRectFlag.Normal, new Vector2 (-1, 1), new Vector2(1, 1), new Vector2(-1, -1), new Vector2(1, -1));
                }
                else {
                    newRectList[i] = meshScript.mesh[i];
                }
            }
            meshScript.mesh = newRectList;
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        safeToDraw = true;
        serializedObject.ApplyModifiedProperties();
        SceneView.lastActiveSceneView.Repaint();
    }

    public void OnSceneGUI() {

        if (safeToDraw) {
            for (int i = 0; i < meshScript.mesh.Length; i++) {

                Vector3[] points = { meshScript.mesh[i].a, meshScript.mesh[i].b, meshScript.mesh[i].d, meshScript.mesh[i].c };

                EditorGUI.BeginChangeCheck();

                switch (meshScript.mesh[i].flag) {
                    case NavMesh.NavRectFlag.Normal:
                        Handles.DrawSolidRectangleWithOutline(points, new Color(0.5f, 0.5f, 0.5f, 0.2f), Color.white);
                        break;
                    case NavMesh.NavRectFlag.Transient:
                        Handles.DrawSolidRectangleWithOutline(points, new Color(0.5f, 0.3f, 0.7f, 0.2f), Color.blue);
                        break;
                    case NavMesh.NavRectFlag.Dangerous:
                        Handles.DrawSolidRectangleWithOutline(points, new Color(0.8f, 0.4f, 0.4f, 0.2f), Color.red);
                        break;
                    default:
                        Handles.DrawSolidRectangleWithOutline(points, new Color(0.5f, 0.5f, 0.5f, 0.05f), Color.black);
                        break;
                }

                if (meshScript.mesh[i].flag != NavMesh.NavRectFlag.Nothing) {
                    meshScript.mesh[i].a = Handles.Slider2D(meshScript.mesh[i].a, Vector3.forward, Vector2.right, Vector2.up, 0.1f, Handles.DotHandleCap, 0.1f);
                    meshScript.mesh[i].b = Handles.Slider2D(meshScript.mesh[i].b, Vector3.forward, Vector2.right, Vector2.up, 0.1f, Handles.DotHandleCap, 0.1f);
                    meshScript.mesh[i].c = Handles.Slider2D(meshScript.mesh[i].c, Vector3.forward, Vector2.right, Vector2.up, 0.1f, Handles.DotHandleCap, 0.1f);
                    meshScript.mesh[i].d = Handles.Slider2D(meshScript.mesh[i].d, Vector3.forward, Vector2.right, Vector2.up, 0.1f, Handles.DotHandleCap, 0.1f);

                    meshScript.mesh[i].Position = Handles.Slider2D(meshScript.mesh[i].Position, Vector3.forward, Vector2.right, Vector2.up, 0.1f, Handles.DotHandleCap, 0.1f);
                }

                if (EditorGUI.EndChangeCheck()) {
                    meshScript.mesh[i].display = true;
                }
            }
        }
        else {
            SceneView.RepaintAll();
        }

        
    }
}
