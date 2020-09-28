using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Interactable : MonoBehaviour {
    //An interactable is an entity that can be used by the player with their Use key.
    //It is designed to handle interaction from the player and interface with them when they come within range.
    //Other scripts can listen to events from this to do what they need to.
    //KEEP IN MIND: This is only compatible with a single player and will break with multiple players.
    
    private Vector2 checkCenter;
    private float checkRadius;
    private static int layerMask;
    private Player player;
    public bool InRange { get; private set; }
    public delegate void InteractEvent(string message, Vector2 position);
    public event InteractEvent EnterRange;
    public event InteractEvent ExitRange;
    public event InteractEvent Use;

    private void Awake() {
        layerMask = LayerMask.NameToLayer("Player");
        if (layerMask < 0) {
            Debug.LogWarning("A Player layer doesn't exist. For Interactables to work, it kinda really needs to.");
        }
        InRange = false;
    }

    private void Update() {
        Collider2D check = Physics2D.OverlapCircle(checkCenter, checkRadius, layerMask);
        if (check != null && !InRange) {
            InRange = true;
            player = check.gameObject.GetComponent<Player>();
            if (player != null) {
                EnterRange?.Invoke("Player has entered range of Interactable (" + name + ")", check.transform.position);
            }
            else {
                Debug.LogWarning("An unknown object in the Player layer has come near Interactable (" + name + ")");
            }
        }
        else if (check == null && InRange) {
            InRange = false;
            if (player != null) {
                ExitRange?.Invoke("Player has exited range of Interactable (" + name + ")", transform.position);
            }
        }

        if (Input.GetKeyDown(GameController.useKey) && player != null) {
            Use?.Invoke("Player has used Interactable (" + name + ")", player.transform.position);
        }
    }
    /// <summary>
    /// If the player is marked inactive by this Interactable, this will release them.
    /// </summary>
    public void ReleasePlayer() {
        player.Active = true;
    }
}
