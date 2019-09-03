using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(NavMesh))]
public class NavMeshEditor : Editor {

    //This whole thing here is used for displaying the NavMesh script all nicely in the Scene and Inspector. Don't touch it!!

    NavMesh _meshScript;
    SerializedProperty _nodeSpacing;
    SerializedProperty _nodeBounds;

    private bool _safeToDraw;

    private void OnEnable() {

        _safeToDraw = false;
    }
    
    public override void OnInspectorGUI() {
        serializedObject.Update();
        _meshScript = target as NavMesh;

        EditorGUI.indentLevel += 1;
        for (int i = 0; i < _meshScript.Mesh.Length; i++) {
            _meshScript.Mesh[i].display = EditorGUILayout.Foldout(_meshScript.Mesh[i].display, _meshScript.Mesh[i].name);
            if (_meshScript.Mesh[i].display) {
                _meshScript.Mesh[i].name = EditorGUILayout.TextField(_meshScript.Mesh[i].name);
                EditorGUILayout.Space();
                _meshScript.Mesh[i].flag = (NavMesh.NavRectFlag)EditorGUILayout.EnumPopup("Flag", _meshScript.Mesh[i].flag);
                EditorGUILayout.Space();
                _meshScript.Mesh[i].a = EditorGUILayout.Vector2Field("Point A", _meshScript.Mesh[i].a);
                _meshScript.Mesh[i].b = EditorGUILayout.Vector2Field("Point B", _meshScript.Mesh[i].b);
                _meshScript.Mesh[i].c = EditorGUILayout.Vector2Field("Point C", _meshScript.Mesh[i].c);
                _meshScript.Mesh[i].d = EditorGUILayout.Vector2Field("Point D", _meshScript.Mesh[i].d);

                EditorGUILayout.Space();
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Delete", GUILayout.Width(150))) {
                    NavMesh.NavRect[] newRectList = new NavMesh.NavRect[_meshScript.Mesh.Length - 1];
                    for (int e = 0; e < newRectList.Length; e++) {
                        if (e >= i) {
                            newRectList[e] = _meshScript.Mesh[e + 1];
                        }
                        else {
                            newRectList[e] = _meshScript.Mesh[e];
                        }
                        
                    }
                    _meshScript.Mesh = newRectList;
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
            NavMesh.NavRect[] newRectList = new NavMesh.NavRect[_meshScript.Mesh.Length + 1];
            for (int i = 0; i < newRectList.Length; i++) {
                if (i == _meshScript.Mesh.Length) {
                    newRectList[i] = new NavMesh.NavRect("Rect " + i, true, NavMesh.NavRectFlag.Normal, new Vector2 (-1, 1), new Vector2(1, 1), new Vector2(-1, -1), new Vector2(1, -1));
                }
                else {
                    newRectList[i] = _meshScript.Mesh[i];
                }
            }
            _meshScript.Mesh = newRectList;
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        _safeToDraw = true;
        serializedObject.ApplyModifiedProperties();
        SceneView.lastActiveSceneView.Repaint();
    }

    public void OnSceneGUI() {

        if (_safeToDraw) {
            for (int i = 0; i < _meshScript.Mesh.Length; i++) {

                Vector3[] points = { _meshScript.Mesh[i].a, _meshScript.Mesh[i].b, _meshScript.Mesh[i].d, _meshScript.Mesh[i].c };

                EditorGUI.BeginChangeCheck();

                switch (_meshScript.Mesh[i].flag) {
                    case NavMesh.NavRectFlag.Normal:
                        Handles.DrawSolidRectangleWithOutline(points, new Color(0.5f, 0.5f, 0.5f, 0.2f), Color.white);
                        break;
                    case NavMesh.NavRectFlag.Transient:
                        Handles.DrawSolidRectangleWithOutline(points, new Color(0.5f, 0.3f, 0.7f, 0.2f), Color.blue);
                        break;
                    case NavMesh.NavRectFlag.Flight:
                        Handles.DrawSolidRectangleWithOutline(points, new Color(0.2f, 0.8f, 0.7f, 0.2f), Color.green);
                        break;
                    case NavMesh.NavRectFlag.Dangerous:
                        Handles.DrawSolidRectangleWithOutline(points, new Color(0.8f, 0.4f, 0.4f, 0.2f), Color.red);
                        break;
                    default:
                        Handles.DrawSolidRectangleWithOutline(points, new Color(0.5f, 0.5f, 0.5f, 0.05f), Color.black);
                        break;
                }

                if (_meshScript.Mesh[i].flag != NavMesh.NavRectFlag.Nothing) {
                    _meshScript.Mesh[i].a = Handles.Slider2D(_meshScript.Mesh[i].a, Vector3.forward, Vector2.right, Vector2.up, 0.1f, Handles.DotHandleCap, 0.1f);
                    _meshScript.Mesh[i].b = Handles.Slider2D(_meshScript.Mesh[i].b, Vector3.forward, Vector2.right, Vector2.up, 0.1f, Handles.DotHandleCap, 0.1f);
                    _meshScript.Mesh[i].c = Handles.Slider2D(_meshScript.Mesh[i].c, Vector3.forward, Vector2.right, Vector2.up, 0.1f, Handles.DotHandleCap, 0.1f);
                    _meshScript.Mesh[i].d = Handles.Slider2D(_meshScript.Mesh[i].d, Vector3.forward, Vector2.right, Vector2.up, 0.1f, Handles.DotHandleCap, 0.1f);

                    _meshScript.Mesh[i].Position = Handles.Slider2D(_meshScript.Mesh[i].Position, Vector3.forward, Vector2.right, Vector2.up, 0.1f, Handles.DotHandleCap, 0.1f);
                }

                if (EditorGUI.EndChangeCheck()) {
                    _meshScript.Mesh[i].display = true;
                }
            }
        }
        else {
            SceneView.RepaintAll();
        }

        
    }
}
