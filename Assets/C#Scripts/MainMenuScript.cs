using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MainMenuScript : MonoBehaviour
{
	public Button startButton;
	public Button quitButton;
	public Button creditsButton;
	
	void MainGUI()
	{
		startButton = startButton.GetComponent<Button>();
		quitButton = quitButton.GetComponent<Button>();
		creditsButton = creditsButton.GetComponent<Button> ();
		
		startButton.enabled = true;
		quitButton.enabled = true;
		creditsButton.enabled = true;
	}
	
	public void StartGame()
	{
		Application.LoadLevel("Setup");
	}
	
	public void ExitGame()
	{
		Application.Quit();
	}
	
	public void ViewCredits()
	{
		Application.LoadLevel ("Credits");
	}
}
