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
		Application.LoadLevel (1);
	}

	public void GoToMainMenu (){
		Application.LoadLevel (0);
	}
}
