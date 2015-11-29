using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PauseScript : MonoBehaviour {

    public bool isPaused = false;
    public static bool isKeysEnabled = true;
	public static bool isOptionsActive = false;
    public GameObject PauseMenu;


	// Use this for initialization
	void Start ()
    {
		Time.timeScale = 1;
    }
	
	// Update is called once per frame
	void Update ()
    {
        ScanForKeyStroke();
	}

	//Handles pause functions by scanning for "Pause" key.
    void ScanForKeyStroke()
    {
		//Consider doing "&& (isOptionsActive == false)" in below "if" so player can't resume game while options menu is active.
        if((Input.GetKeyDown(Player.keyPAUSE)) && (!isOptionsActive))
        {
			//This will trigger when the pause key is pressed during gameplay.
            if (isPaused == false)
            {
                isPaused = true;
                ToggleAudio();
                Time.timeScale = 0;
                LockInput(true);
                PauseMenu.SetActive(true);
            }
			//This will trigger when the pause key is pressed in the pause menu.
            else
            {
                isPaused = false;
                ToggleAudio();
                Time.timeScale = 1;
                LockInput(false);
                PauseMenu.SetActive(false);
            }
        }
    }

	//Turns game music on or off depending on pause settings.
    void ToggleAudio()
    {
        if(isPaused == true)
        {
            foreach(var audio in GetComponents<AudioSource>())
            {
                audio.Pause();
            }
        }
        else
        {
            foreach (var audio in GetComponents<AudioSource>())
            {
                audio.UnPause();
            }
        }
    }

	//Locks the player from operating the game if they are in the pause menu.
    void LockInput(bool setInputLock)
    {
        if(setInputLock)
        {
            isKeysEnabled = false;
        }
        else
        {
            isKeysEnabled = true;
        }
    }

	//Resumes the game.
    public void ResumeGame()
    {
		if(!isOptionsActive)
		{
			isPaused = false;
			ToggleAudio();
			Time.timeScale = 1;
			LockInput(false);
			PauseMenu.SetActive(false);
		}
    }

	//This will set the options menu to active, thereby triggering the options script.
    public void OptionsMenu()
    {
		if (!isOptionsActive) {
			isOptionsActive = true; 
		}
    }

	//Sends the player to the title menu.
    public void ExitToMainMenu()
    {
		if (!isOptionsActive) {
			isPaused = false;
			ToggleAudio();
			LockInput(false);

			// Removes the player object.
			Destroy (FindObjectOfType<Player> ());
			Application.LoadLevel("MainMenu");
		}
    }

	//Quits the game.
    public void QuitGame()
    {
		if (!isOptionsActive) {
			isPaused = false;
			ToggleAudio();
			PauseMenu.SetActive(false);
			Application.Quit();
		}
    }
}