using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour {

    private static HUDController instance;

    [SerializeField] public Image fade;

	void Awake() {
        instance = this;
    }

	void Update () {

	}

    public static IEnumerator Fade(float duration, float tick, Color target) {
        Color old;
        try {
            old = instance.fade.color;
        }
        catch {
            Debug.Log("The Fade animation could not be started. Is the HUD missing from the current scene?");
            yield break;
        }
        float interp = 0;
        while (instance.fade.color != target) {
            interp += tick / duration;
            instance.fade.color = Color.Lerp(old, target, interp);

            yield return new WaitForSeconds(tick);
        }
    }
}
