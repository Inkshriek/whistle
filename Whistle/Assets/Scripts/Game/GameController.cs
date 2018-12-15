using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Whistle.Characters;
using Whistle.Familiars;
using Whistle.Conditions;

public class GameController : MonoBehaviour {

    [SerializeField] private Player player;
    [SerializeField] private List<Familiar> familiars; //The list of familiars the player presently has access to. These are intended to be prefabs.
    private Familiar currentFamiliar; //The familiar currently active.


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
        for (int i = 0; i < familiars.Count; i++) {
            familiars[i] = Instantiate(familiars[i], player.transform.position, Quaternion.identity);
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
