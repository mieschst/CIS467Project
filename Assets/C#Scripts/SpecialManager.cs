using UnityEngine;
using System.Collections;

public class SpecialManager : MonoBehaviour {

	public GameObject specialMenu;

	void Start(){
		specialMenu.SetActive (true);
	}

	public void EquipBow(){
		Player.bowAttackEnabled = true;
		Player.bombAttackEnabled = false;
		Player.diggingClawsEnabled = false;
	}

	public void EquipBomb(){
		Player.bowAttackEnabled = false;
		Player.bombAttackEnabled = true;
		Player.diggingClawsEnabled = false;
	}

	public void EquipDiggingClaws(){
		Player.bowAttackEnabled = false;
		Player.bombAttackEnabled = false;
		Player.diggingClawsEnabled = true;
	}

	void ToggleSpecialMenu(){
		// Toggles the visibility of the special attack menu.
		if (Input.GetKeyDown (Player.keyPAUSE) || Player.playerInShop) {
			specialMenu.SetActive (false);
		} else if (Player.canMove) {
			specialMenu.SetActive (true);
		}
	}

	void Update(){
		ToggleSpecialMenu ();
	}
}
