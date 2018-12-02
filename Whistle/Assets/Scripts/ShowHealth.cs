using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowHealth : MonoBehaviour {

    public PlayerController character;
    private Slider slider;

	// Use this for initialization
	void Start () {
        slider = GetComponent<Slider>();

	}
	
	// Update is called once per frame
	void Update () {
        slider.value = character.CurrentHealth;
        slider.maxValue = character.MaxHealth;
    }
}
