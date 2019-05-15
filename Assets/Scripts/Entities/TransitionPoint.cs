using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Whistle.Actors;

[RequireComponent(typeof(BoxCollider2D))]
public class TransitionPoint : MonoBehaviour {

    [SerializeField] public TransitionType type; //The type of the transition.
    [SerializeField] public string destinationScene; //The scene you're intending to go to. Use its name from Assets.
    [SerializeField] public int destinationTPoint; //The transition point in the room you want Ichabod to enter from, by index.
    private bool transitioning;
    private bool roomChanged;

    private void Awake() {
        transitioning = false;
        roomChanged = false;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        if (scene.name == destinationScene) {
            roomChanged = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.CompareTag("Player") && !transitioning) {
            StartCoroutine(Transition(collision.gameObject.GetComponent<Player>()));
        }
    }

    public IEnumerator Transition(Player player) {
        DontDestroyOnLoad(this);

        player.Mode = ActorMode.Inactive;
        player.Action = PlayerAction.Running;

        switch (type) {
            case TransitionType.Left:
                player.Controller.Motion = new Vector2(-8, 0);

                break;
            case TransitionType.Right:
                player.Controller.Motion = new Vector2(8, 0);

                break;
            default:

                break;
        }

        StartCoroutine(HUDController.Fade(1f, 0.05f, new Color(0, 0, 0, 1)));
        yield return new WaitForSeconds(1);

        SceneManager.LoadScene(destinationScene);
        yield return new WaitUntil(() => roomChanged == true);

        SceneData sceneData = FindObjectOfType<SceneData>();
        try {
            sceneData.transitions[destinationTPoint].gameObject.SetActive(false);
            player.transform.position = sceneData.transitions[destinationTPoint].transform.position;
        }
        catch {
            Debug.LogError("An object with SceneData is missing from the scene. Sorry bud, but the transition is fucked.");
            yield break;
        }
        StartCoroutine(HUDController.Fade(1f, 0.05f, new Color(0, 0, 0, 0)));
        yield return new WaitForSeconds(1);

        player.Controller.Motion = Vector2.zero;
        player.Mode = ActorMode.Active;

        sceneData.transitions[destinationTPoint].gameObject.SetActive(true);
        Destroy(this.gameObject);
    }

    public enum TransitionType {
        Left,
        Right,
        None
    }
}
