using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthMeter : MonoBehaviour {

    [SerializeField] public Player player;
    [SerializeField] public RectTransform cog;
    [SerializeField] public RectTransform liquid;
    Vector2 normalSize;

    // Use this for initialization
    void Awake() {
        normalSize = liquid.sizeDelta;
    }
	
	// Update is called once per frame
	void Update () {
        liquid.sizeDelta = new Vector2(normalSize. x * (player.Health / 100), normalSize.y);
        cog.Rotate(new Vector3(0, 0, 0.2f));
    }
}
