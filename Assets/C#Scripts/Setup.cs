﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Setup : MonoBehaviour {

	public GameObject player;

	public InputField playerNameField;

	public Button normalBtn;
	public Button hardBtn;

	// Use this for initialization
	void Start () {
		playerNameField = FindObjectOfType<InputField> ();
		normalBtn = normalBtn.GetComponent<Button> ();
		hardBtn = hardBtn.GetComponent<Button> ();
	}

	public void SelectNormalMode(){
		GameManager.isHardMode = false;
	}

	public void SelectHardMode(){
		GameManager.isHardMode = true;
	}

	public void SetupGame() {
		if (playerNameField.text.Length > 0) {
			player.GetComponent<Player> ().myName = playerNameField.text;
		}
		// Sets up the player before jumping to the new scene.
		Instantiate (player, new Vector3 (0, 0), Quaternion.identity);
		// Tells the application to maintain the player gameobject when switching to the Game scene.
		DontDestroyOnLoad (FindObjectOfType<Player>());
		Application.LoadLevel("Game");
	}
}
