using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Whistle.Characters;
using Whistle.Familiars;
using Whistle.Conditions;

public class GameController : MonoBehaviour {

    private static GameObject[] familiarsDatabase;

    [SerializeField] private Player player;
    [SerializeField] private List<Familiar> familiars; //The list of familiars the player presently has access to. These are intended to be prefabs.
    private Familiar currentFamiliar; //The familiar currently active.

    public static KeyCode jumpKey = KeyCode.Space;

    private void Awake() {
        Debug.Log("Initializing!");
        familiarsDatabase = new GameObject[] {
            Resources.Load("Familiars/Familiar_Feu") as GameObject
        };
        Debug.Log(familiarsDatabase.Length);
    }
    // Use this for initialization
    private void Start () {
        StartCoroutine(LoadGame());

        
		
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

    private IEnumerator LoadGame() {
        for (int i = 0; i < familiarsDatabase.Length; i++) {
            GameObject obj = Instantiate(familiarsDatabase[i], player.transform.position, Quaternion.identity);
            familiars.Add(obj.GetComponent<Familiar>());
        }

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
}
