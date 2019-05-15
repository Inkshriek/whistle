using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneData : MonoBehaviour {

    [SerializeField] public string roomName; //The name of the room. This is for display purposes only.
    [SerializeField] public TransitionPoint[] transitions; //A set of transition points the player can enter the scene in and out from. You should set these cuz this thing won't find them for you.
}