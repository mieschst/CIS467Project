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
	public GameObject[] slots;
	//These six booleans tell us if an inventory slot is full or not.

	public Sprite healthPotion;
	public Sprite bomb;
	public Sprite arrow;
	public Sprite key;
	public Sprite noItemImage;

	public static bool[] filledSlots;

	// Use this for initialization
	void Start () {
		// Sets up the filled slot booleans to all be false.
		filledSlots = new bool[slots.Length];
		isInventoryActive = false;
		// Hides the inventory menu.
		inventoryMenu.SetActive (false);
		// Sets all of the slots to be active initially. Since the menu is hidden, these will not be displayed initially.
		foreach (GameObject slot in slots) {
			slot.SetActive(true);
		}
	}
	
	// Update is called once per frame
	void Update () {
		// Toggles the visibility of the inventory menu.
		toggleInventory ();
		setMenu ();
	}

	void toggleInventory(){
		if ((Input.GetKeyDown (keyINVENTORY)) && (PauseScript.isKeysEnabled)) {
			// Disables or enables the inventory.
			isInventoryActive = !isInventoryActive;
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

	public void updateInventory(List<Item> playerList){
		int j = 0;

		// Sets all of the slots to be empty.
		for (int i = 0; i < filledSlots.Length; i++) {
			filledSlots[i] = false;
			slots[i].GetComponent<Image>().sprite = noItemImage;
		}

		// Cycle through the player's inventory and update the inventory slots with sprite images and setting
		// the filled slot booleans appropriately.
		foreach (Item item in playerList) {
			switch(item.Name){
			case "HealthPotion":
				slots[j].GetComponent<Image>().sprite = healthPotion;
				filledSlots[j] = true;
				break;
			case "Key":
				slots[j].GetComponent<Image>().sprite = key;
				filledSlots[j] = true;
				break;
			case "Bomb":
				slots[j].GetComponent<Image>().sprite = bomb;
				filledSlots[j] = true;
				break;
			case "Arrow":
				slots[j].GetComponent<Image>().sprite = arrow;
				filledSlots[j] = true;
				break;
			default:
				slots[j].GetComponent<Image>().sprite = noItemImage;
				filledSlots[j] = false;
				break;
			}
			j++;
		}

		// Sets any leftover slots to have empty sprite images.
		for (int i = 0; i < filledSlots.Length; i++) {
			if(filledSlots[i] == false){
				slots[j].GetComponent<Image>().sprite = noItemImage;
			}
		}
	}

	public void ClearSlot(int index){
		// Sets the slot at position 'index' to have a blank picture.
		slots[index].GetComponent<Image>().sprite = noItemImage;
		// Sets the filled slot boolean at position 'index' to false.
		filledSlots [index] = false;
	}
}
