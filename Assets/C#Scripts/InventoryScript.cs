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
		filledSlots = new bool[slots.Length];
		isInventoryActive = false;
		inventoryMenu.SetActive (false);
		foreach (GameObject slot in slots) {
			slot.SetActive(true);
		}
	}
	
	// Update is called once per frame
	void Update () {
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

		for (int i = 0; i < filledSlots.Length; i++) {
			filledSlots[i] = false;
			slots[i].GetComponent<Image>().sprite = noItemImage;
		}

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

		for (int i = 0; i < filledSlots.Length; i++) {
			if(filledSlots[i] == false){
				slots[j].GetComponent<Image>().sprite = noItemImage;
			}
		}
	}

	public void ClearSlot(int index){
		slots[index].GetComponent<Image>().sprite = noItemImage;
		filledSlots [index] = false;
	}
}
