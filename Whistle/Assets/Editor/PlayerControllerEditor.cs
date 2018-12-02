using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Whistle.Characters;

[CustomEditor(typeof(PlayerController))]
public class PlayerControllerEditor : Editor {
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        PlayerController player = (PlayerController)target;
        if (player.GetComponents<ICharacter>().Length > 1) {
            EditorGUILayout.HelpBox("Don't add more than one character controller script to a GameObject! This will cause unprecedented problems, trust me.", MessageType.Warning);
        }
    }
}
