using UnityEngine;
using System.Collections;

public class Item {

	public string Name { get; set; }

	public Item(string itemName){
		if(itemName.Contains("(Clone)")){
			Name = itemName.Substring(0, itemName.Length - 7);
		}
		else{
			Name = itemName;
		}
	}

	// Every item should have some sort of an effect when used.
	public void Use (Player player){
		switch (Name) {
		case "Heart":
			player.Health++;
			Player.setHUDhealth(player.Health);
			break;
		case "RupeeSmall":
			player.Currency++;
			Player.setHUDcurrency(player.Currency);
			break;
		case "RupeeMedium":
			player.Currency += 5;
			Player.setHUDcurrency(player.Currency);
			break;
		case "RupeeLarge":
			player.Currency += 10;
			Player.setHUDcurrency(player.Currency);
			break;
		case "HealthPotion":
			player.Health += 5;
			Player.setHUDhealth(player.Health);
			break;
		}
	}

	public void Steal (Sableye sableye){
		switch (Name) {
			case "RupeeSmall":
				sableye.Currency++;
				break;
			case "RupeeMedium":
				sableye.Currency += 5;
				break;
			case "RupeeLarge":
				sableye.Currency += 10;
				break;
		}
	}
}
