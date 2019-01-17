using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Whistle.Characters;
using Whistle.Familiars;
using Whistle.Conditions;
using Whistle.Cutscenes;

public class GameController : MonoBehaviour {

    private static GameObject[] familiarsDatabase;

    [SerializeField] public string roomName; //The name of the room.
    [SerializeField] private Player player; //The player character. This gets put somewhere in the scene according to the transition points.
    [SerializeField] private TransitionPoint[] transitions; //A set of transition points the player can enter the scene in and out from. You should set these cuz this thing won't find them for you.

    private static List<Familiar> familiars; //The list of familiars the player presently has access to. These are intended to be prefabs.
    private static Familiar currentFamiliar; //The familiar currently active.

    public static bool cutsceneRunning = false;
    public static bool transitioningRoom = false;
    public static int nextTPoint = 0;

    public static KeyCode jumpKey = KeyCode.Space;

    public static GameController SceneGameController {
        //Assuming you need the specific one in the scene, anyway.
        get {
            return (GameController)FindObjectOfType(typeof(GameController));
        }
    }

    private void Awake() {
        Debug.Log("Initializing!");
        familiarsDatabase = new GameObject[] {
            Resources.Load("Familiars/Familiar_Feu") as GameObject
        };
        Debug.Log(familiarsDatabase.Length);

        cutsceneRunning = false;

        InitializeGame();
    }

    // Use this for initialization
    private void Start() {
        if (transitioningRoom) {
            try {
                transitions[nextTPoint].EnterRoomFromHere(player);
                nextTPoint = 0;
            }
            catch {
                Debug.Log("Apparently the transition point requested [" + nextTPoint + "] doesn't exist. Might wanna fix that.");
            }
        }
        else {
            player.Mode = CharacterMode.Active;
        }
    }

    public void InitializeGame() {
        StartCoroutine(Load());
	}
	
	// Update is called once per frame
	private void Update () {
		if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene("SampleScene");
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    private IEnumerator Load() {
        /*
        for (int i = 0; i < familiarsDatabase.Length; i++) {
            GameObject obj = Instantiate(familiarsDatabase[i], player.transform.position, Quaternion.identity);
            familiars.Add(obj.GetComponent<Familiar>());
        }
        */

        yield return new WaitForSeconds(3f);
    }

    public void ChangeFamiliar(int index) {
        if (currentFamiliar != null) {
            Destroy(currentFamiliar.gameObject);
        }

        try {
            currentFamiliar = Instantiate(familiars[index]);
        }
        catch {
            Debug.LogError("Familiar could not be instantiated! You may have entered an invalid index, or the element called is not a prefab.");
        }
    }

    public void StartCutscene(Cutscene scene) {
        StartCoroutine(scene());
        cutsceneRunning = true;
    }

    public static void MoveToRoom(string sceneName, int transitionPoint) {
        transitioningRoom = true;
        nextTPoint = transitionPoint;
        SceneManager.LoadScene(sceneName);
    }
}
