using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ShopScript : MonoBehaviour {

	Player player;
	public GameObject shopPanel;

	// Use this for initialization
	void Start () {
		player = FindObjectOfType<Player> ();
		shopPanel.SetActive (false);
	}

	public void ArrowButtonClicked(){
		if (player.Currency >= 5) {
			player.Currency -= 5;
			player.Inventory.Add(new Item("Arrow"));
		}
	}

	public void BombButtonClicked(){
		if (player.Currency >= 10) {
			player.Currency -= 10;
			player.Inventory.Add(new Item("Bomb"));
		}
	}

	public void ExperienceButtonClicked(){
		if (player.Currency >= 50) {
			player.Currency -= 50;
			player.Experience += 50;
			GameObject expMoblin = new GameObject();
			expMoblin.AddComponent<Moblin>();
			expMoblin.GetComponent<Moblin>().Experience = 50;
			player.DefeatEnemy(expMoblin.GetComponent<Moblin>());
			Destroy (expMoblin);
		}
	}

	public void DoneButtonClicked(){
		shopPanel.SetActive (false);
		GameObject.Find ("Merchant(Clone)").GetComponent<Animator> ().SetTrigger ("MerchantRockOn");
	}

	public void OpenShopScreen(){
		shopPanel.SetActive (true);
	}
}
