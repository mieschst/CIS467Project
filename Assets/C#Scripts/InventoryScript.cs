using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class InventoryScript : MonoBehaviour {

	//The boolean that checks if the player has the inventory viewable.
	public bool isInventoryActive;
	//The inventory menu object.
	public GameObject inventoryMenu;
	//Holds the key that activates the inventory. Can be reset by the option's menu button configuration. 
	public static KeyCode keyINVENTORY = KeyCode.I;
	//These are the background images of the six inventory slots that will be modified.
	public GameObject Item1;
	public GameObject Item2;
	public GameObject Item3;
	public GameObject Item4;
	public GameObject Item5;
	public GameObject Item6;
	//These six booleans tell us if an inventory slot is full or not.
	public static bool Item1Full = false;
	public static bool Item2Full = false;
	public static bool Item3Full = false;
	public static bool Item4Full = false;
	public static bool Item5Full = false;
	public static bool Item6Full = false;

	// Use this for initialization
	void Start () {
		isInventoryActive = false;
		inventoryMenu.SetActive (false);
		Item1.SetActive (false);
		Item2.SetActive (false);
		Item3.SetActive (false);
		Item4.SetActive (false);
		Item5.SetActive (false);
		Item6.SetActive (false);
	}
	
	// Update is called once per frame
	void Update () {
		toggleInventory ();
		setMenu ();
		toggleSlots ();
	}

	void toggleInventory(){
		if ((Input.GetKeyDown (keyINVENTORY)) && (PauseScript.isKeysEnabled)) {
			if(isInventoryActive == false){
				isInventoryActive = true;
			}else{
				isInventoryActive = false;
			}
		}
	}

	void setMenu(){
		if (PauseScript.isKeysEnabled) {
			if (isInventoryActive) {
				inventoryMenu.SetActive(true);
			} else {
				inventoryMenu.SetActive(false);
			}
		} else {
			isInventoryActive = false;
			inventoryMenu.SetActive(false);
		}
	}

	void toggleSlots(){
		if (Item1Full) {
			Item1.SetActive(true);
		} else {
			Item1.SetActive(false);
		}
		if (Item2Full) {
			Item2.SetActive(true);
		} else {
			Item2.SetActive(false);
		}
		if (Item3Full) {
			Item3.SetActive(true);
		} else {
			Item3.SetActive(false);
		}
		if (Item4Full) {
			Item4.SetActive(true);
		} else {
			Item4.SetActive(false);
		}
		if (Item5Full) {
			Item5.SetActive(true);
		} else {
			Item5.SetActive(false);
		}
		if (Item6Full) {
			Item6.SetActive(true);
		} else {
			Item6.SetActive(false);
		}
	}

	public static void updateInventory(List<Item> playerList){
		for(int i = 0; i<playerList.Count; i++){
			Item slot = playerList[i];
			switch(slot.Name)
			{
			case "HealthPotion":
				if(!Item1Full){
					Item1Full = true;
					break;
				}else if(!Item2Full){
					Item2Full = true;
					break;
				}else if(!Item3Full){
					Item3Full = true;
					break;
				}else if(!Item4Full){
					Item4Full = true;
					break;
				}else if(!Item5Full){
					Item5Full = true;
					break;
				}else{
					playerList.Remove (slot);
					break;
				}
			case "Key":
				if(!Item6Full){
					Item6Full = true;
					break;
				}else{
					playerList.Remove(slot);
					break;
				}
			default:
				playerList.Remove (slot);
				break;
			}
		}
	}

	public static void removeFromInventory(Item slot){
		switch (slot.Name) {
		case "HealthPotion":
			if(Item1Full){
				Item1Full = false;
				break;
			}else if(Item2Full){
				Item2Full = false;
				break;
			}else if(Item3Full){
				Item3Full = false;
				break;
			}else if(Item4Full){
				Item4Full = false;
				break;
			}else if(Item5Full){
				Item5Full = false;
				break;
			}else{
				break;
			}
		case "Key":
			if(Item6Full){
				Item6Full = false;
				break;
			}else{
				break;
			}
		default:
			break;
		}
	}
}
