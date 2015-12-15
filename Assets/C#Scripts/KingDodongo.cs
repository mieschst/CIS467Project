using UnityEngine;
using System.Collections;

public class KingDodongo : Unit {
	
	Animator kingDodongoAnimator;
	
	int direction = 0;
	
	public LayerMask blockingLayer;
	public LayerMask unitsLayer;

    public GameObject attackEffect;

    GameObject player;
	
	public void InitKingDodongo(bool isHardMode) {
		CalculateStats (CalculateLevel(), isHardMode);
	}
	
	int CalculateLevel (){
		return (GameManager.isHardMode) ? (int)Mathf.Ceil (Player.floorLevel / 1.3F) : (int)Mathf.Ceil (Player.floorLevel / 1.1F);
	}
	
	// Initializes key variables for the King Dodongo enemy.
	void Start () {
		InitKingDodongo (GameManager.isHardMode);
		kingDodongoAnimator = this.GetComponent<Animator> ();
		player = FindObjectOfType<Player> ().gameObject;
	}
	
	public void CalculateStats(int level, bool isHardMode){
		this.Level = level;
		this.Health = 3;
		this.Attack = 1;
		this.Defense = 1;
		this.Speed = 1;
		this.Experience = 30 * level;
		
		// If we are on normal mode, then just follow the normal enemy stat calculations.
		if (isHardMode == false) {
			for (int i = 1; i < level; i++) {
				if (i % 2 == 0) {
					this.Health += 2;
					this.Attack += 2;
					this.Defense ++;
				} else {
					this.Attack++;
					this.Defense++;
					this.Speed++;
				}
			}
		}
		// Otherwise, if we are on hard mode, then King Dodongo will have enhanced stats.
		else {
			for (int i = 1; i < level; i++) {
				if (i % 2 == 0) {
					this.Health += 2;
					this.Attack += 2;
					this.Defense += 2;
				} else {
					this.Attack++;
					this.Defense++;
					this.Speed++;
				}
			}
		}
	}
	
	public void CalculateDamageDealt(Unit player){
		// If the enemy's attack stat is greater than the player's defense, then set the new damage amount.
		// The enemy's attack must be at least 2 more than the player's defense for the damage to be more
		// than 1.
		int damage = (this.Attack > player.Defense) ? this.Attack - player.Defense : 1;
		player.Health -= damage;
	}
	
	int ChasePlayer(){
		int moveDirection = -1;
		
		float playerX = player.transform.position.x;
		float playerY = player.transform.position.y;
		float enemyX = this.transform.position.x;
		float enemyY = this.transform.position.y;
		
		if (playerX > enemyX) {
			moveDirection = 2;
		} else if (playerX < enemyX) {
			moveDirection = 3;
		} else if (playerY > enemyY) {
			moveDirection = 1;
		} else if (playerY < enemyY) {
			moveDirection = 0;
		}
		
		return moveDirection;
	}
	
	public override void Move(){
		if (!Player.PLAYERS_TURN) {
			Vector3 startPosition = this.transform.position;
			Vector3 endPosition = this.transform.position;
		
			int movement = 1;
			direction = ChasePlayer ();
		
			switch (direction) {
			case 0: 
				endPosition = new Vector3 (startPosition.x, startPosition.y - movement);
				kingDodongoAnimator.Play ("KingDodongoForwardIdle");
				break;
			case 1:
				endPosition = new Vector3 (startPosition.x, startPosition.y + movement);
				kingDodongoAnimator.Play ("KingDodongoBackwardIdle");
				break;
			case 2:
				endPosition = new Vector3 (startPosition.x + movement, startPosition.y);
				kingDodongoAnimator.Play ("KingDodongoRightIdle");
				break;
			case 3:
				endPosition = new Vector3 (startPosition.x - movement, startPosition.y);
				kingDodongoAnimator.Play ("KingDodongoLeftIdle");
				break;
			}
		
			BoxCollider2D boxCollider = this.GetComponent<BoxCollider2D> ();
		
			boxCollider.enabled = false;
		
			RaycastHit2D hit = Physics2D.Linecast (startPosition, endPosition, blockingLayer);
			RaycastHit2D hitUnit = Physics2D.Linecast (startPosition, endPosition, unitsLayer);
		
			boxCollider.enabled = true;
		
			if (!hit && !hitUnit) {
				this.transform.position = endPosition;
			}
			if (hitUnit) {
				AttackPlayer (hitUnit, direction);
			}
		}
	}
	
	void AttackPlayer(RaycastHit2D hitPlayer, int movementDirection){
		if (hitPlayer.collider.gameObject.tag.Equals ("Player")) {
            Vector3 target = new Vector3();
			switch (movementDirection) {
			case 0:
				kingDodongoAnimator.SetTrigger ("KingDodongoAttackForward");
                    target = new Vector3(this.transform.position.x, this.transform.position.y - 1);
                    Instantiate(attackEffect, target, Quaternion.identity);
                    break;
			case 1:
				kingDodongoAnimator.SetTrigger ("KingDodongoAttackBackward");
                    target = new Vector3(this.transform.position.x, this.transform.position.y + 1);
                    Instantiate(attackEffect, target, Quaternion.identity);
                    break;
			case 2:
				kingDodongoAnimator.SetTrigger ("KingDodongoAttackRight");
                    target = new Vector3(this.transform.position.x + 1, this.transform.position.y);
                    Instantiate(attackEffect, target, Quaternion.identity);
                    break;
			case 3:
				kingDodongoAnimator.SetTrigger ("KingDodongoAttackLeft");
                    target = new Vector3(this.transform.position.x - 1, this.transform.position.y);
                    Instantiate(attackEffect, target, Quaternion.identity);
                    break;
			}
			CalculateDamageDealt (hitPlayer.collider.gameObject.GetComponent<Player> ());
			if (hitPlayer.collider.gameObject.GetComponent<Player> ().Health <= 0) {
				Destroy (hitPlayer.collider.gameObject, 1.0F);
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (PauseScript.isKeysEnabled) {
			Move ();
		}
	}
}
