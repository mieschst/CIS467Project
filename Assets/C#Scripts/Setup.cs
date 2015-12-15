using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Setup : MonoBehaviour {

	public GameObject player;
	public GameObject audioManager;

	public InputField playerNameField;

	public Button normalBtn;
	public Button hardBtn;

	// Use this for initialization
	void Start () {
		// Sets the GameManager difficulty to 'Normal' as a default.
		GameManager.isHardMode = false;
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
		// Sets up the player before jumping to the new scene.
		Instantiate (player, new Vector3 (0, 0), Quaternion.identity);
		Instantiate (audioManager);
		// Checks if the user entered a name.
		if (playerNameField.text.Length > 0) {
			Player.myName = playerNameField.text;
		}
		// Tells the application to maintain the player gameobject when switching to the Game scene.
		DontDestroyOnLoad (FindObjectOfType<Player>());
		// Allows the AudioManager to persist through the scene change.
		DontDestroyOnLoad (FindObjectOfType<AudioManager>());
		// Loads the Game scene.
		Application.LoadLevel("Game");
	}
}
