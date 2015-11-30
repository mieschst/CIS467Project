using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

public class Moblin : Enemy {

	Animator moblinAnimator;

	int numFrames;

	bool hardModeEnabled;

	public override void InitEnemy(int level, bool isHardMode) {
		CalculateStats (level, isHardMode);
	}

	public override void CalculateStats(int level, bool isHardMode){
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

	// Initializes key variables for the Moblin enemy.
	void Start () {
		InitEnemy (1, GameManager.isHardMode);
		numFrames = 0;
		moblinAnimator = this.GetComponent<Animator> ();
	}

	public override void CalculateDamageDealt(Unit player){
		// If the enemy's attack stat is greater than the player's defense, then set the new damage amount.
		// The enemy's attack must be at least 2 more than the player's defense for the damage to be more
		// than 1.
		int damage = (this.Attack > player.Defense) ? this.Attack - player.Defense : 1;
		player.Health -= damage;
	}

	public override void Move(){
		Vector3 startPosition = this.transform.position;
		Vector3 endPosition = this.transform.position;
		
		int movement = 1;
		int direction = (int)(Random.value * 4);
		
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
				Destroy (hitPlayer.collider.gameObject, 1F);
			}
		}
	}

	// Update is called once per frame
	void Update () {
		if (PauseScript.isKeysEnabled) {
			numFrames++;
			if (numFrames == FRAMES_PER_TURN) {
				Move ();
				numFrames = 0;
			}
		}
	}
}
