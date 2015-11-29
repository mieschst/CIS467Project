using UnityEngine;
using System.Collections;

public class Sableye : Enemy {
	Animator animator;

	int numFrames;

	public static Vector3 currentPosition;

    private AudioSource source;

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
		//Sableye doesn't attack
	}

	public override void Move(){
		Vector3 startPosition = this.transform.position;
		Vector3 endPosition = this.transform.position;
		
		int movement = 1;
		int direction = (int)(Random.value * 4);
		
		switch (direction) {
		case 0: 
			endPosition = new Vector3 (startPosition.x, startPosition.y - movement);
			animator.Play ("SableyeDown");
			//animator.Play ("GarchompDown");
			break;
		case 1:
			endPosition = new Vector3 (startPosition.x, startPosition.y + movement);
			animator.Play ("SableyeUp");
			//animator.Play ("GarchompUp");
			break;
		case 2:
			endPosition = new Vector3 (startPosition.x + movement, startPosition.y);
			animator.Play ("SableyeRight");
			//animator.Play ("GarchompRight");
			break;
		case 3:
			endPosition = new Vector3 (startPosition.x - movement, startPosition.y);
			animator.Play ("SableyeLeft");
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
	}

	void OnTriggerEnter2D(Collider2D collider){

		if (collider.gameObject.tag.Equals ("Item")) {
			// Adds the item to the player's inventory.
			Item item = new Item(collider.gameObject.name);
			if(item.Name.Contains("Rupee")) {
				item.Steal(this);
				source.Play();
				// Removes the item from the game board.
				Destroy (collider.gameObject);
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
