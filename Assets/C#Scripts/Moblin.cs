using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

public class Moblin : Unit {

	Animator moblinAnimator;
	
	bool hardModeEnabled;

	int direction = 0;

	public LayerMask blockingLayer;
	public LayerMask unitsLayer;

	GameObject player = null;

	public void InitMoblin(int level, bool isHardMode) {
		CalculateStats (level, isHardMode);
	}

	// Initializes key variables for the Moblin enemy.
	void Start () {
		InitMoblin (1, GameManager.isHardMode);
		moblinAnimator = this.GetComponent<Animator> ();
	}

	public void CalculateStats(int level, bool isHardMode){
		this.Level = level;
		this.Health = 3;
		this.Attack = 1;
		this.Defense = 1;
		this.Speed = 1;
		this.Experience = 10 * level;

		// If we are on normal mode, then just follow the normal enemy stat calculations.
		if (isHardMode == false) {
			for (int i = 1; i < level; i++) {
				if (i % 2 == 0) {
					this.Health++;
					this.Attack++;
					this.Defense++;
				} else {
					this.Attack++;
					this.Speed++;
				}
			}
		}
		// Otherwise, if we are on hard mode, then the moblin will have enhanced health and attack stats.
		else {
			for (int i = 1; i < level; i++) {
				if (i % 2 == 0) {
					this.Health += 2;
					this.Attack += 2;
					this.Defense++;
				} else {
					this.Attack++;
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
		Vector3 startPosition = this.transform.position;
		Vector3 endPosition = this.transform.position;

		if (GameManager.isHardMode) {
			CheckLineOfSight ();
		}

		int movement = 1;
		if (player == null) {
			direction = (int)(Random.value * 4);
		} else {
			direction = ChasePlayer ();
		}
		
		switch (direction) {
		case 0: 
			endPosition = new Vector3 (startPosition.x, startPosition.y - movement);
			moblinAnimator.Play ("MoblinForwardIdle");
			break;
		case 1:
			endPosition = new Vector3 (startPosition.x, startPosition.y + movement);
			moblinAnimator.Play ("MoblinBackwardIdle");
			break;
		case 2:
			endPosition = new Vector3 (startPosition.x + movement, startPosition.y);
			moblinAnimator.Play ("MoblinRightIdle");
			break;
		case 3:
			endPosition = new Vector3 (startPosition.x - movement, startPosition.y);
			moblinAnimator.Play ("MoblinLeftIdle");
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
			AttackPlayer(hitUnit, direction);
		}
	}

	void AttackPlayer(RaycastHit2D hitPlayer, int movementDirection){
		if (hitPlayer.collider.gameObject.tag.Equals ("Player")) {
			switch (movementDirection) {
			case 0:
				moblinAnimator.SetTrigger ("MoblinAttackForward");
				break;
			case 1:
				moblinAnimator.SetTrigger ("MoblinAttackBackward");
				break;
			case 2:
				moblinAnimator.SetTrigger ("MoblinAttackRight");
				break;
			case 3:
				moblinAnimator.SetTrigger ("MoblinAttackLeft");
				break;
			}
			CalculateDamageDealt (hitPlayer.collider.gameObject.GetComponent<Player> ());
			if (hitPlayer.collider.gameObject.GetComponent<Player> ().Health <= 0) {
				Destroy (hitPlayer.collider.gameObject, 1.0F);
			}
		}
	}

	public void CheckLineOfSight(){
		int sight = 3;

		Vector3 startPosition = this.transform.position;
		Vector3 endPosition = this.transform.position;

		switch (direction) {
		case 0: 
			endPosition = new Vector3 (startPosition.x, startPosition.y - sight);
			break;
		case 1:
			endPosition = new Vector3 (startPosition.x, startPosition.y + sight);
			break;
		case 2:
			endPosition = new Vector3 (startPosition.x + sight, startPosition.y);
			break;
		case 3:
			endPosition = new Vector3 (startPosition.x - sight, startPosition.y);
			break;
		}

		this.GetComponent<BoxCollider2D> ().enabled = false;

		RaycastHit2D hitPlayer = Physics2D.Linecast (startPosition, endPosition, unitsLayer);

		this.GetComponent<BoxCollider2D> ().enabled = true;


		if (hitPlayer) {
			if (hitPlayer.collider.gameObject.tag.Contains ("Player")) {
				player = hitPlayer.collider.gameObject;
			}
		}
	}

	// Update is called once per frame
	void Update () {
		if (PauseScript.isKeysEnabled) {
			if (!Player.PLAYERS_TURN){
				Move ();
			}
		}
	}
}
