using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ShopScript : MonoBehaviour {

	Player player;
	public GameObject shopPanel;
	public Text experienceText;

	// Use this for initialization
	void Start () {
		player = FindObjectOfType<Player> ();
		shopPanel.SetActive (false);
	}

	public void ArrowButtonClicked(){
		// Checks to see if the player's basic item count is smaller than 1 less than their capacity.
		if (player.Currency >= 5 && Player.basicItemCount < Player.INVENTORY_CAPACITY - 1) {
			player.Currency -= 5;
			player.Inventory.Add(new Item("Arrow"));
			Player.basicItemCount++;
		}
	}

	public void BombButtonClicked(){
		// Checks to see if the player's basic item count is smaller than 1 less than their capacity.
		if (player.Currency >= 10 && Player.basicItemCount < Player.INVENTORY_CAPACITY - 1) {
			player.Currency -= 10;
			player.Inventory.Add(new Item("Bomb"));
			Player.basicItemCount++;
		}
	}

	public void ExperienceButtonClicked(){
		// Calculates the amount of experience the player will gain if they have enough money.
		int experienceGained = 40 + (Player.floorLevel * 10);
		// If the player has 50 rupees, then give them the experience amount calculated above.
		if (player.Currency >= 50) {
			player.Currency -= 50;
			GameObject expMoblin = new GameObject();
			expMoblin.AddComponent<Moblin>();
			expMoblin.GetComponent<Moblin>().Experience = experienceGained;
			player.DefeatEnemy(expMoblin.GetComponent<Moblin>());
			Destroy (expMoblin);
		}
	}

	public void DoneButtonClicked(){
		shopPanel.SetActive (false);
		// Allows the player the ability to move again.
		Player.canMove = true;
		// Allows the player the ability to move again.
		Player.playerInShop = false;
		GameObject.Find ("Merchant(Clone)").GetComponent<Animator> ().SetTrigger ("MerchantRockOn");
	}

	public void OpenShopScreen(){
		shopPanel.SetActive (true);
		// Prevents the player from moving around when in the shop.
		Player.playerInShop = true;
	}

	void Update(){
		experienceText.text = string.Empty + (40 + (Player.floorLevel * 10)) + " experience\n\nCost: 50 rupees";
	}
}
