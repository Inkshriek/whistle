using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour {

    [SerializeField] public Image fade;

    public static HUDController SceneHUD {
        get {
            return (HUDController)FindObjectOfType(typeof(HUDController));
        }
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
