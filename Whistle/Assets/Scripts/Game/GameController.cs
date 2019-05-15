using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Whistle.Actors;
using Whistle.Familiars;
using Whistle.Conditions;
using Whistle.Cutscenes;

public class GameController : MonoBehaviour {

    private static GameController instance;
    private static SceneData sceneData;
    private static GameObject[] familiarsDatabase;

    [SerializeField] private Player player; //The player character.

    private static List<Familiar> familiars; //The list of familiars the player presently has access to. These are intended to be prefabs.
    private static Familiar currentFamiliar; //The familiar currently active.

    public static bool cutsceneRunning = false;

    public static KeyCode jumpKey = KeyCode.Space;

    private void Awake() {
        SceneManager.sceneLoaded += OnSceneLoaded;
        Debug.Log("Initializing!");
        familiarsDatabase = new GameObject[] {
            Resources.Load("Familiars/Familiar_Feu") as GameObject
        };
        Debug.Log(familiarsDatabase.Length);

        cutsceneRunning = false;
        instance = this;

        DontDestroyOnLoad(this.gameObject);
    }

    // Use this for initialization
    private void Start() {

    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        sceneData = FindObjectOfType<SceneData>();
        InitializeRoom();
    }

    public void InitializeRoom() {
        
	}

    private void LoadGame() {

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
        cutsceneRunning = true;
    }
}
