﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HUDScript : MonoBehaviour {

	//These text objects display gameplay information to the player.
	public Text HealthText;
	public Text SkillText;
	public Text RupeeText;

	//This slider manages the player's health bar
	public Slider HealthSlider;
	public Image SliderBackground;
	public Image SliderForeground;

	// Use this for initialization
	void Start () {
		//The following block sets up the HUD
		HealthText = GameObject.Find ("HealthText").GetComponent<Text> ();
		HealthText.enabled = true;
		SkillText = GameObject.Find ("SkillText").GetComponent<Text> ();
		SkillText.enabled = true;
		RupeeText = GameObject.Find ("RupeeText").GetComponent<Text> ();
		RupeeText.enabled = true;
		//This part sets up the player's health bar
		HealthSlider.enabled = true;
		SliderBackground.enabled = true;
		SliderForeground.enabled = true;
	}
	
	// Update is called once per frame
	void Update () {

		CheckIfPaused ();
		AdjustPlayerHealthBar ();
	}

	void CheckIfPaused(){

		if (PauseScript.isKeysEnabled) {
			//Enables the HUD's labels
			HealthText.enabled = true;
			HealthText.text = "HEALTH: ";
			SkillText.enabled = true;
			SkillText.text = "PLAYER LEVEL: " + Player.playerLevel;
			RupeeText.enabled = true;
			RupeeText.text = "RUPEES: " + Player.currency;

			//Enables the HUD's player health bar
			HealthSlider.enabled = true;
			SliderBackground.enabled = true;
			SliderForeground.enabled = true;

		} else {
			//Disables the HUD's labels
			HealthText.enabled = false;
			SkillText.enabled = false;
			RupeeText.enabled = false;
			//Disables the HUD's player health bar
			HealthSlider.enabled = false;
			SliderBackground.enabled = false;
			SliderForeground.enabled = false;
		}
	}

	void AdjustPlayerHealthBar(){

		HealthSlider.maxValue = Player.maxhealth;
		
		if (Player.health <= 0) {
			HealthSlider.value = 0;
			SliderForeground.enabled = false;
		} else if (Player.health >= Player.maxhealth) {
			HealthSlider.value = HealthSlider.maxValue;
		} else {
			HealthSlider.value = Player.health;
		}
	}

}
