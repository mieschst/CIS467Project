using UnityEngine;
using System.Collections;

public class Cynthia : Enemy {

// The amount of health the player has.
	Animator animator;

	int numFrames;

	public static Vector3 currentPosition;

	public override void InitEnemy(int level){
		CalculateStats (level);
	}

	public override void CalculateStats(int level){
		this.Level = level;
		this.Health = 3;
		this.Attack = 1;
		this.Defense = 1;
		this.Speed = 1;
		this.Experience = 10 * level;

		for(int i = 1; i < level; i++){
			if(i % 2 == 0){
				this.Health++;
				this.Attack++;
				this.Defense++;
			}
			else {
				this.Attack++;
				this.Speed++;
			}
		}
	}

	// Use this for initialization
	void Start () {
		InitEnemy (1);
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
		int direction = (int)(Random.value * 4);
		
		switch (direction) {
		case 0: 
			endPosition = new Vector3 (startPosition.x, startPosition.y - movement);
			animator.Play ("cynthia_ 1");
			break;
		case 1:
			endPosition = new Vector3 (startPosition.x, startPosition.y + movement);
			animator.Play ("cynthia_");
			break;
		case 2:
			endPosition = new Vector3 (startPosition.x + movement, startPosition.y);
			animator.Play ("cynthia_ 3");
			break;
		case 3:
			endPosition = new Vector3 (startPosition.x - movement, startPosition.y);
			animator.Play ("cynthia_ 2");
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
