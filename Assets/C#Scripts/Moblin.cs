﻿using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

public class Moblin : Unit {

	Animator moblinAnimator;

	int numFrames;

	const int FRAMES_PER_TURN = 60;

	public LayerMask blockingLayer;
	public LayerMask unitsLayer;

	public void InitMoblin(int level) {
		CalculateStats (level);
	}

	// Initializes key variables for the Moblin enemy.
	void Start () {
		InitMoblin (1);
		numFrames = 0;
		moblinAnimator = this.GetComponent<Animator> ();
	}

	public void CalculateStats(int level){
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

	public void CalculateDamageDealt(Unit player){
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
	}

	// Update is called once per frame
	void Update () {
		numFrames++;
		if (numFrames == FRAMES_PER_TURN) {
			Move ();
			numFrames = 0;
		}
	}
}
