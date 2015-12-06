using UnityEngine;
using System.Collections;

public class Sableye : Enemy {
	Animator animator;

	int numFrames;

	public static Vector3 currentPosition;

    private AudioSource source;
    private bool rupeesOnBoard;

	public override void InitEnemy(int level, bool isHardMode){
		CalculateStats (level, isHardMode);

		state = 0;
		maxmoves = 1.0;
		moves = maxmoves;
        source = GetComponent<AudioSource>();

        rupeesOnBoard = true;
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

	public Vector3 findRupees(){
		int numItems = BoardManager.boardItems.childCount;
		Vector3 closestRupee = this.transform.position;
		for(int i=0;i<numItems;i++){
			Transform item = BoardManager.boardItems.GetChild(i);
			if(item.name.Contains("Rupee")){
				return item.position;
			}
		}
		rupeesOnBoard = false;
		return closestRupee;
	}

	public override void Move(){
		Vector3 startPosition = this.transform.position;
		Vector3 endPosition = this.transform.position;
		
		int movement = 1;
		int direction;
		Vector3 rupeePosition = findRupees();
		if(rupeesOnBoard){
			float sableyeRupeeXDiff = rupeePosition.x - this.transform.position.x;
			float sableyeRupeeYDiff = rupeePosition.y - this.transform.position.y;
			float absSableyeRupeeXDiff = Mathf.Abs(sableyeRupeeXDiff);
			float absSableyeRupeeYDiff = Mathf.Abs(sableyeRupeeYDiff);

			if(absSableyeRupeeYDiff > 1 || absSableyeRupeeXDiff > 1){
				if(absSableyeRupeeYDiff > absSableyeRupeeXDiff){
					if(absSableyeRupeeXDiff != 0){
						direction = moveWestEast(sableyeRupeeXDiff);
					}else{
						direction = moveNorthSouth(sableyeRupeeYDiff);
					}
				}else {
					if(absSableyeRupeeYDiff != 0){
						direction = moveNorthSouth(sableyeRupeeYDiff);
					}else{
						direction = moveWestEast(sableyeRupeeXDiff);
					}
				}
			}else if(absSableyeRupeeXDiff == 1 && absSableyeRupeeYDiff == 1){
				if(sableyeRupeeXDiff == 1 && sableyeRupeeYDiff == 1){
					direction = 0;
				}else if(sableyeRupeeXDiff == 1 && sableyeRupeeYDiff == -1){
					direction = 3;
				}else if(sableyeRupeeXDiff == -1 && sableyeRupeeYDiff == -1){
					direction = 1;
				}else {
					direction = 2;
				}
			}else {
				if(absSableyeRupeeXDiff > 0){
					direction = moveWestEast(sableyeRupeeXDiff);
				} else{
					direction = moveNorthSouth(sableyeRupeeYDiff);
				}
			}
		}
		else{
			direction = (int)(Random.value * 4);
		}
		
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
		//Doesn't quite work how I want it right now.
		/*else if(hit || hitUnit){
			int temp = 0;
			while(hit || hitUnit){
				temp++;
				if(temp == 4)
					break;

				switch(direction){
					case 0:
						endPosition = new Vector3 (startPosition.x + movement, startPosition.y);
						animator.Play ("SableyeRight");
						direction = 2;
						//animator.Play ("GarchompRight");
						break;
					case 1:
						endPosition = new Vector3 (startPosition.x - movement, startPosition.y);
						animator.Play ("SableyeLeft");
						direction = 3;
						//animator.Play ("GarchompLeft");
						break;
					case 2: 
						endPosition = new Vector3 (startPosition.x, startPosition.y - movement);
						animator.Play ("SableyeDown");
						direction = 0;
						break;
					case 3:
						endPosition = new Vector3 (startPosition.x, startPosition.y + movement);
						animator.Play ("SableyeUp");
						direction = 1;
						//animator.Play ("GarchompUp");
						break;
				}
				hit = Physics2D.Linecast (startPosition, endPosition, blockingLayer);
				hitUnit = Physics2D.Linecast (startPosition, endPosition, unitsLayer);
			}
			this.transform.position = endPosition;
		}*/
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
