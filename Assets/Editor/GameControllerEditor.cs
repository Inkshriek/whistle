using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GameController))]
public class GameControllerEditor : Editor {

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        GameController controller = (GameController)target;

    }
}
