using UnityEngine;
using System.Collections;

public class GameOverScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void QuitGame() {
		Application.Quit ();
	}

	public void RestartGame() {
		Application.LoadLevel ("Setup");
	}

	public void GoToMainMenu (){
		Application.LoadLevel ("MainMenu");
	}
}
