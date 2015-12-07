using UnityEngine;
using System.Collections;

public class SpecialManager : MonoBehaviour {

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

}
