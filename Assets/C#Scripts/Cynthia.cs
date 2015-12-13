using UnityEngine;
using System.Collections;

public class Cynthia : Enemy {

// The amount of health the player has.
	Animator animator;

	int numFrames;

	public static Vector3 currentPosition;

    private AudioSource source;

    public GameObject attackEffect;

	public override void InitEnemy(int level, bool isHardMode){
		CalculateStats (level, isHardMode);

		state = 0;
		maxmoves = 1.0;
		moves = maxmoves;
        source = GetComponent<AudioSource>();
	}

	public override void CalculateStats(int level, bool isHardMode){
		this.Level = level;
		this.Health = 3;
		this.Attack = 1;
		this.Defense = 1;
		this.Speed = 1;
		this.Experience = 20 * level;

		int i = 0;
		int j = 0;

		for(i = 1; i < level; i++){
			if(i % 2 == 0){
				this.Health++;
				this.Attack++;
				this.Defense++;
				// If we are on normal mode, then just follow the normal enemy stat calculations.
				if (isHardMode == false) {
					for (j = 1; j < level; j++) {
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
					for (j = 1; j < level; j++) {
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
			} else {
				this.Attack++;
				this.Speed++;
			}
		}
	}

	// Use this for initialization
	void Start () {
		InitEnemy (1, GameManager.isHardMode);
		numFrames = 0;
		animator = GetComponent<Animator> ();
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
		//int direction = (int)(Random.value * 4);
		int direction;
		float playerEnemyXDiff = Player.currentPosition.x - this.transform.position.x;
		float playerEnemyYDiff = Player.currentPosition.y - this.transform.position.y;
		float absPlayerEnemyXDiff = Mathf.Abs(playerEnemyXDiff);
		float absPlayerEnemyYDiff = Mathf.Abs(playerEnemyYDiff);

		if(absPlayerEnemyYDiff > 1 || absPlayerEnemyXDiff > 1){
			//We aren't in range of the player to attack, we must advance!
			//Now, which way should we go...
			if(absPlayerEnemyYDiff > absPlayerEnemyXDiff){
				if(absPlayerEnemyXDiff != 0){
					direction = moveWestEast(playerEnemyXDiff);
				}else{
					direction = moveNorthSouth(playerEnemyYDiff);
				}
			}else {
				if(absPlayerEnemyYDiff != 0){
					direction = moveNorthSouth(playerEnemyYDiff);
				}else{
					direction = moveWestEast(playerEnemyXDiff);
				}
			}
		}else if(absPlayerEnemyXDiff == 1 && absPlayerEnemyYDiff == 1){
			if(playerEnemyXDiff == 1 && playerEnemyYDiff == 1){
				direction = 0;
			}else if(playerEnemyXDiff == 1 && playerEnemyYDiff == -1){
				direction = 3;
			}else if(playerEnemyXDiff == -1 && playerEnemyYDiff == -1){
				direction = 1;
			}else {
				direction = 2;
			}
		}else {
			if(absPlayerEnemyXDiff > 0){
				direction = moveWestEast(playerEnemyXDiff);
			} else{
				direction = moveNorthSouth(playerEnemyYDiff);
			}
		}
		
		switch (direction) {
		case 0: 
			endPosition = new Vector3 (startPosition.x, startPosition.y - movement);
			animator.Play ("cynthia_ 1");
			//animator.Play ("GarchompDown");
			break;
		case 1:
			endPosition = new Vector3 (startPosition.x, startPosition.y + movement);
			animator.Play ("cynthia_");
			//animator.Play ("GarchompUp");
			break;
		case 2:
			endPosition = new Vector3 (startPosition.x + movement, startPosition.y);
			animator.Play ("cynthia_ 3");
			//animator.Play ("GarchompRight");
			break;
		case 3:
			endPosition = new Vector3 (startPosition.x - movement, startPosition.y);
			animator.Play ("cynthia_ 2");
			//animator.Play ("GarchompLeft");
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

	int moveNorthSouth(float y){
		if(y > 0){
			return 1;
		}else{
			return 0;
		}
	}

	int moveWestEast(float x){
		if(x > 0){
			return 2;
		}else{
			return 3;
		}
	}

	void AttackPlayer(RaycastHit2D hitPlayer, int movementDirection){
		if (hitPlayer.collider.gameObject.tag.Equals ("Player")) {
            Vector3 target = new Vector3();
            source.Play();
			switch (movementDirection) {
			case 0:
				animator.Play ("GarchompDown");
                    target = new Vector3(this.transform.position.x , this.transform.position.y - 1);
                    Instantiate(attackEffect, target, Quaternion.identity);
                    break;
			case 1:
				animator.Play ("GarchompUp");
                    target = new Vector3(this.transform.position.x, this.transform.position.y + 1);
                    Instantiate(attackEffect, target, Quaternion.identity);
                    break;
			case 2:
				animator.Play ("GarchompRight");
                    target = new Vector3(this.transform.position.x + 1, this.transform.position.y);
                    Instantiate(attackEffect, target, Quaternion.identity);
                    break;
			case 3:
				animator.Play ("GarchompLeft");
                    target = new Vector3(this.transform.position.x - 1, this.transform.position.y);
                    Instantiate(attackEffect, target, Quaternion.identity);
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
