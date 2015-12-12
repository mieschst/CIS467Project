using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class OptionsMenuScript : MonoBehaviour {

	public KeyCode kcTEMP = KeyCode.Q;
	public Text ErrorText;
	public Text SuccessText;
	public Text DefaultKeyText;
	public GameObject OptionsMenu;

	// Use this for initialization
	void Start () 
	{
		DefaultKeyText.enabled = true;
		ErrorText.enabled = false;
		SuccessText.enabled = false;
	}
	
	// Update is called once per frame
	void Update () 
	{
		ScanForOptionsActive();
	}

	void ScanForOptionsActive()
	{
		//Triggers the options menu depending on whether the Options button was pressed.
		if (PauseScript.isOptionsActive) {
			OptionsMenu.SetActive (true);
			foreach (KeyCode pressKey in System.Enum.GetValues(typeof(KeyCode))) {
				if ((Input.GetKey (pressKey)) && (pressKey != KeyCode.Mouse0)) {
					kcTEMP = pressKey;
					DefaultKeyText.enabled = false;
				}
			}
		} else {
			OptionsMenu.SetActive (false);
		}
	}

	//Disable the options menu and return to the pause menu.
	public void ReturnToPauseMenu ()
	{
		DefaultKeyText.enabled = true;
		SuccessText.enabled = false;
		ErrorText.enabled = false;
		PauseScript.isOptionsActive = false;
	}


	//Reset the default controls if the user forgot their settings.
	public void ResetDefaultControls ()
	{
		Player.keyUP = KeyCode.UpArrow;
		Player.keyDOWN = KeyCode.DownArrow;
		Player.keyLEFT = KeyCode.LeftArrow;
		Player.keyRIGHT = KeyCode.RightArrow;
		Player.keyMOVE = KeyCode.A;
		Player.keyATTACK = KeyCode.S;
		Player.keyITEM = KeyCode.D;
		Player.keyCANCEL = KeyCode.Space;
		Player.keyPAUSE = KeyCode.Escape;
		Player.keyEXIT = KeyCode.Return;
		Player.keyPOTION = KeyCode.O;
		InventoryScript.keyINVENTORY = KeyCode.I;
		kcTEMP = KeyCode.Q;
		DefaultKeyText.enabled = true;
		SuccessText.enabled = false;
		ErrorText.enabled = false;
	}

	//Makes sure that the new input key doesn't overlap with an existing control.
	bool CheckKeyOverlap (KeyCode kcTEMP, string keyFlag)
	{
		bool isUnique = true;
		int sharedKeys = 0;

		if ((kcTEMP == Player.keyUP) && (keyFlag != "up")) {
			sharedKeys++;
		}
		if ((kcTEMP == Player.keyDOWN) && (keyFlag != "down")) {
			sharedKeys++;
		}
		if ((kcTEMP == Player.keyLEFT) && (keyFlag != "left")) {
			sharedKeys++;
		}
		if ((kcTEMP == Player.keyRIGHT) && (keyFlag != "right")) {
			sharedKeys++;
		}
		if ((kcTEMP == Player.keyMOVE) && (keyFlag != "move")) {
			sharedKeys++;
		}
		if ((kcTEMP == Player.keyATTACK) && (keyFlag != "attack")) {
			sharedKeys++;
		}
		if ((kcTEMP == Player.keyITEM) && (keyFlag != "item")) {
			sharedKeys++;
		}
		if ((kcTEMP == Player.keyCANCEL) && (keyFlag != "cancel")) {
			sharedKeys++;
		}
		if ((kcTEMP == Player.keyPAUSE) && (keyFlag != "pause")) {
			sharedKeys++;
		}
		if ((kcTEMP == Player.keyEXIT) && (keyFlag != "quit")) {
			sharedKeys++;
		}
		if ((kcTEMP == Player.keyPOTION) && (keyFlag != "potion")) {
			sharedKeys++;
		}
		if ((kcTEMP == InventoryScript.keyINVENTORY) && (keyFlag != "inventory")) {
			sharedKeys++;
		}

		if (sharedKeys > 0) {
			isUnique = false;
		}

		return isUnique;
	}
	
	//If this button is pressed then search for new input.
	public void SetUpButton ()
	{
		if (CheckKeyOverlap (kcTEMP, "up")) {
			Player.keyUP = kcTEMP;
			SuccessText.text = "SUCCESS: The 'UP' command was re-assigned to the key: " + kcTEMP;
			SuccessText.enabled = true;
			ErrorText.enabled = false;
		} else {
			SuccessText.enabled = false;
			ErrorText.text = "ERROR: The 'UP' command is already assigned to a key other than: " + kcTEMP; 
			ErrorText.enabled = true;
		}
	}
	//If this button is pressed then search for new input.
	public void SetDownButton ()
	{
		if (CheckKeyOverlap (kcTEMP, "down")) {
			Player.keyDOWN = kcTEMP;
			SuccessText.text = "SUCCESS: The 'DOWN' command was re-assigned to the key: " + kcTEMP;
			SuccessText.enabled = true;
			ErrorText.enabled = false;
		} else {
			SuccessText.enabled = false;
			ErrorText.text = "ERROR: The 'DOWN' command is already assigned to a key other than: " + kcTEMP;
			ErrorText.enabled = true;
		}
	}
	//If this button is pressed then search for new input.
	public void SetLeftButton ()
	{
		if (CheckKeyOverlap (kcTEMP, "left")) {
			Player.keyLEFT = kcTEMP;
			SuccessText.text = "SUCCESS: The 'LEFT' command was re-assigned to the key: " + kcTEMP;
			SuccessText.enabled = true;
			ErrorText.enabled = false;
		} else {
			SuccessText.enabled = false;
			ErrorText.text = "ERROR: The 'LEFT' command is already assigned to a key other than: " + kcTEMP;
			ErrorText.enabled = true;
		}
	}
	//If this button is pressed then search for new input.
	public void SetRightButton ()
	{
		if (CheckKeyOverlap (kcTEMP, "right")) {
			Player.keyRIGHT = kcTEMP;
			SuccessText.text = "SUCCESS: The 'RIGHT' command was re-assigned to the key: " + kcTEMP;
			SuccessText.enabled = true;
			ErrorText.enabled = false;
		} else {
			SuccessText.enabled = false;
			ErrorText.text = "ERROR: The 'RIGHT' command is already assigned to a key other than: " + kcTEMP;
			ErrorText.enabled = true;
		}
	}
	//If this button is pressed then search for new input.
	public void SetMoveButton ()
	{
		if (CheckKeyOverlap (kcTEMP, "move")) {
			Player.keyMOVE = kcTEMP;
			SuccessText.text = "SUCCESS: The 'MOVE' command was re-assigned to the key: " + kcTEMP;
			SuccessText.enabled = true;
			ErrorText.enabled = false;
		} else {
			SuccessText.enabled = false;
			ErrorText.text = "ERROR: The 'MOVE' command is already assigned to a key other than: " + kcTEMP;
			ErrorText.enabled = true;
		}
	}
	//If this button is pressed then search for new input.
	public void SetAttackButton ()
	{
		if (CheckKeyOverlap (kcTEMP, "attack")) {
			Player.keyATTACK = kcTEMP;
			SuccessText.text = "SUCCESS: The 'ATTACK' command was re-assigned to the key: " + kcTEMP;
			SuccessText.enabled = true;
			ErrorText.enabled = false;
		} else {
			SuccessText.enabled = false;
			ErrorText.text = "ERROR: The 'ATTACK' command is already assigned to a key other than: " + kcTEMP;
			ErrorText.enabled = true;
		}
	}
	//If this button is pressed then search for new input.
	public void SetCancelButton ()
	{
		if (CheckKeyOverlap (kcTEMP, "cancel")) {
			Player.keyCANCEL = kcTEMP;
			SuccessText.text = "SUCCESS: The 'CANCEL' command was re-assigned to the key: " + kcTEMP;
			SuccessText.enabled = true;
			ErrorText.enabled = false;
		} else {
			SuccessText.enabled = false;
			ErrorText.text = "ERROR: The 'CANCEL' command is already assigned to a key other than: " + kcTEMP;
			ErrorText.enabled = true;
		}
	}
	//If this button is pressed then search for new input.
	public void SetPauseButton ()
	{
		if (CheckKeyOverlap (kcTEMP, "pause")) {
			Player.keyPAUSE = kcTEMP;
			SuccessText.text = "SUCCESS: The 'PAUSE' command was re-assigned to the key: " + kcTEMP;
			SuccessText.enabled = true;
			ErrorText.enabled = false;
		} else {
			SuccessText.enabled = false;
			ErrorText.text = "ERROR: The 'PAUSE' command is already assigned to a key other than: " + kcTEMP;
			ErrorText.enabled = true;
		}
	}
	//If this button is pressed then search for new input.
	public void SetPotionButton ()
	{
		if (CheckKeyOverlap (kcTEMP, "potion")) {
			Player.keyPOTION = kcTEMP;
			SuccessText.text = "SUCCESS: The 'POTION' command was re-assigned to the key: " + kcTEMP;
			SuccessText.enabled = true;
			ErrorText.enabled = false;
		} else {
			SuccessText.enabled = false;
			ErrorText.text = "ERROR: The 'POTION' command is already assigned to a key other than: " + kcTEMP;
			ErrorText.enabled = true;
		}
	}
	//If this button is pressed then search for new input.
	public void SetInventoryButton ()
	{
		if (CheckKeyOverlap (kcTEMP, "inventory")) {
			InventoryScript.keyINVENTORY = kcTEMP;
			SuccessText.text = "SUCCESS: The 'INVENTORY' command was re-assigned to the key: " + kcTEMP;
			SuccessText.enabled = true;
			ErrorText.enabled = false;
		} else {
			SuccessText.enabled = false;
			ErrorText.text = "ERROR: The 'INVENTORY' command is already assigned to a key other than: " + kcTEMP;
			ErrorText.enabled = true;
		}
	}

}
