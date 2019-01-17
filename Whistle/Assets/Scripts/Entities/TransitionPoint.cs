using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(BoxCollider2D))]
public class TransitionPoint : MonoBehaviour {

    [SerializeField] public TransitionType type; //The type of the transition.
    [SerializeField] public string destinationRoom; //The room you're intending to go to.
    [SerializeField] public int destinationTPoint; //The transition point in the room you want Ichabod to enter from, by index.

    private HUDController HUD;

    private void Awake() {
        HUD = HUDController.SceneHUD;
    }

    public void EnterRoomFromHere(Player player) {
        StartCoroutine(TransitionIn(player));
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.CompareTag("Player") && !GameController.transitioningRoom) {
            StartCoroutine(TransitionOut(collision.gameObject.GetComponent<Player>()));
        }
    }

    private IEnumerator TransitionOut(Player player) {
        player.Mode = Whistle.Characters.CharacterMode.Inactive;
        player.State = Whistle.Characters.PlayerState.Walking;
        GameController.transitioningRoom = true;

        switch(type) {
            case TransitionType.Left:
                player.Controller.Motion = new Vector2(-2, 0);
                
                break;
            case TransitionType.Right:
                player.Controller.Motion = new Vector2(2, 0);

                break;
            default:
                Debug.Log("lol");

                break;
        }

        HUD.fade.color = new Color(0, 0, 0, 0);
        while (HUD.fade.color.a < 1) {
            HUD.fade.color = new Color(0, 0, 0, HUD.fade.color.a + 0.025f);
            yield return new WaitForSeconds(0.05f);
        }
        GameController.MoveToRoom(destinationRoom, destinationTPoint);
    }

    private IEnumerator TransitionIn(Player player) {

        player.transform.position = transform.position;
        player.Mode = Whistle.Characters.CharacterMode.Inactive;
        player.State = Whistle.Characters.PlayerState.Walking;

        switch (type) {
            case TransitionType.Left:
                player.Controller.Motion = new Vector2(2, 0);

                break;
            case TransitionType.Right:
                player.Controller.Motion = new Vector2(-2, 0);

                break;
            default:
                Debug.Log("lol");

                break;
        }

        HUD.fade.color = new Color(0, 0, 0, 1);
        while (HUD.fade.color.a > 0) {
            HUD.fade.color = new Color(0, 0, 0, HUD.fade.color.a - 0.025f);
            yield return new WaitForSeconds(0.05f);
        }
        player.Controller.Motion = Vector2.zero;
        player.Mode = Whistle.Characters.CharacterMode.Active;

        GameController.transitioningRoom = false;
    }

    public enum TransitionType {
        Left,
        Right,
        Whoknowsman
    }
}
