using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CreditsScript : MonoBehaviour {

	float timeLeft = 15.0f;
	public Text ScrollText;

	void Update () {
		Time.timeScale = 1;
		ScrollText = GameObject.Find ("ScrollText").GetComponent<Text> ();

		string credits = "DUNGEON CRAWLER CREDITS\n\n\n\n";
		credits += "User Interface Designer:\n Steven\n\n";
		credits += "AI Manager:\n Jacob\n\n";
		credits += "Programmer:\n Nick\n\n";
		credits += "Game Logic Designer:\n Billy\n\n";
		credits += "Map Designer:\n Ryan\n\n\n\n";
		credits += "All Legend of Zelda and Pokemon related assets are property of Nintendo";
		ScrollText.text = credits;

		timeLeft -= Time.deltaTime;
		if ((Input.GetKeyDown (KeyCode.Mouse0)) || (timeLeft <= 0)) {
			Application.LoadLevel (0);
		}
	}
}
