using UnityEngine;
using System.Collections;

public class Cynthia : Unit {

// The amount of health the player has.

	string currentDirection = "south";

	Animator animator;

	BoardManager bm;

	public int rows;
	public int columns;

	public static Vector3 currentPosition;

	public void InitPlayer(int level, bool isHardMode){

		CalculateStats (level, isHardMode);

		state = 0;
		maxmoves = 1.0;
		moves = maxmoves;

		canWalk = true;
		canJump = true;
	}

	public void CalculateStats(int level, bool isHardMode){
		this.Level = level;
		this.Health = 3;
		this.Attack = 1;
		this.Defense = 1;
		this.Speed = 1;
		this.Experience = 20 * level;
		
		// If we are on normal mode, then just follow the normal enemy stat calculations.
		if (isHardMode == false) {
			for (int i = 1; i < level; i++) {
				if (i % 2 == 0) {
					this.Health++;
					this.Speed++;
					this.Defense++;
				} else {
					this.Attack++;
					this.Speed++;
				}
			}
		}
		// Otherwise, if we are on hard mode, then Cynthia will have enhanced health and speed stats.
		else {
			for (int i = 1; i < level; i++) {
				if (i % 2 == 0) {
					this.Health += 2;
					this.Speed++;
					this.Defense++;
				} else {
					this.Attack++;
					this.Speed += 2;
				}
			}
		}
	}

	// Sets the borders for the player movement.
	public void SetMoveLimits(int rows = 0, int columns = 0){
		this.rows = rows;
		this.columns = columns;
	}

	private Vector3 goNorth(Vector3 currentPosition) {
		currentPosition.y++;
		animator.Play ("cynthia_");
		if(currentPosition.y == columns - 1){
			//If you can't go north, choose a random direction and check if you can go that way
			switch (Random.Range(0,2)){
				case 0: 
					currentDirection = "south";
					break;
				case 1: 
					if(currentPosition.x != 0){
						currentDirection = "west";
					}else {
						currentDirection = "east";
					}
					break;
				case 2:
					if(currentPosition.x < rows - 1){
						currentDirection = "east";
					}else {
						currentDirection = "west";
					}
					break;
				default:
					currentDirection = "south";
					break;
			}
		}
		return currentPosition;
	}
	private Vector3 goSouth(Vector3 currentPosition) {
		currentPosition.y--;		
		animator.Play ("cynthia_ 1");
		if(currentPosition.y == 0){
			//If you can't go south, choose a random direction and check if you can go that way
			switch (Random.Range(0,2)){
				case 0: 
					currentDirection = "north";
					break;
				case 1: 
					if(currentPosition.x != 0){
						currentDirection = "west";
					}else {
						currentDirection = "east";
					}
					break;
				case 2:
					if(currentPosition.x < rows - 1){
						currentDirection = "east";
					}else {
						currentDirection = "west";
					}
					break;
				default:
					currentDirection = "north";
					break;
			}
		}
		return currentPosition;
	}
	private Vector3 goWest(Vector3 currentPosition) {
		currentPosition.x--;		
		animator.Play ("cynthia_ 2");
		if(currentPosition.x == 0){
			//If you can't go west, choose a random direction and check if you can go that way
			switch (Random.Range(0,2)){
				case 0: 
					currentDirection = "east";
					break;
				case 1: 
					if(currentPosition.y != 0){
						currentDirection = "south";
					}else {
						currentDirection = "north";
					}
					break;
				case 2:
					if(currentPosition.y < rows - 1){
						currentDirection = "north";
					}else {
						currentDirection = "south";
					}
					break;
				default:
					currentDirection = "east";
					break;
			}
		}		
		return currentPosition;
	}
	private Vector3 goEast(Vector3 currentPosition) {
		currentPosition.x++;		
		animator.Play ("cynthia_ 3");
		if(currentPosition.x == columns - 1){
			//If you can't go north, choose a random direction and check if you can go that way
			switch (Random.Range(0,2)){
				case 0: 
					currentDirection = "west";
					break;
				case 1: 
					if(currentPosition.y != 0){
						currentDirection = "south";
					}else {
						currentDirection = "north";
					}
					break;
				case 2:
					if(currentPosition.y < rows - 1){
						currentDirection = "north";
					}else {
						currentDirection = "south";
					}
					break;
				default:
					currentDirection = "west";
					break;
			}
		}
		return currentPosition;
	}

	private void collisionHandler(Vector3 otherCharacterPosition){
		float xPlus1 = currentPosition.x + (float)1.0;
		float xMinus1 = currentPosition.x - (float)1.0;
		float yPlus1 = currentPosition.y + (float)1.0;
		float yMinus1 = currentPosition.y - (float)1.0;
		//Debug.Log(otherCharacterPosition);
		//Debug.Log(currentPosition);
		switch(currentDirection) {
			case "south":
				if(otherCharacterPosition.x != currentPosition.x || otherCharacterPosition.y != yMinus1)
					currentPosition = goSouth(currentPosition);
				moves--;
		//Debug.Log(yMinus1);
				break;
			case "north":
				if(otherCharacterPosition.x != currentPosition.x || otherCharacterPosition.y != yPlus1)
					currentPosition = goNorth(currentPosition);
				moves--;
		//Debug.Log(yPlus1);
				break;
			case "west":
				if(otherCharacterPosition.x != xMinus1 || otherCharacterPosition.y != currentPosition.y)
					currentPosition = goWest(currentPosition);
				moves--;
		//Debug.Log(xMinus1);
				break;
			default:
				if(otherCharacterPosition.x != xPlus1  || otherCharacterPosition.y != currentPosition.y)
					currentPosition = goEast(currentPosition);
				moves--;
		//Debug.Log(xPlus1);
				break;
		}
	}

	// Use this for initialization
	void Start () {
		animator = GetComponent<Animator> ();

		// Ititializes the player stats.
		InitPlayer (1, GameManager.isHardMode);
		int xPos = 0;
		int yPos = 0;
		SetMoveLimits (9,15);
		while(xPos == 0 && yPos == 0){
			xPos = Random.Range(0,rows);
			yPos = Random.Range(0,columns);
		}
		this.transform.position = new Vector3(xPos,yPos,0);
	}

	// Update is called once per frame
	new void Update () {
		if (PauseScript.isKeysEnabled) {
			base.Update ();
			if (!Player.PLAYERS_TURN){
				Move ();
			}
		}
	}

	//sub function'd for inheritence compatibility
	public override void Move () {
		currentPosition = this.transform.position;
		//if(state < 4){
			collisionHandler(Player.currentPosition);
		//}

		this.transform.position = currentPosition;
	}
}
