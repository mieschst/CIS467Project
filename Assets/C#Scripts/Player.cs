using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;
using System.Collections.Generic;

public class Player : Unit {
	
	/*
	 * Static Input Keys
	 * 4 Directional Keys (keyUP, keyDOWN, keyLEFT, keyRIGHT)
	 * 4 Command Keys (keyMOVE, keyATTACK, keyITEM, keyCANCEL)
	 * 2 System Keys (keyPAUSE, keyEXIT)
	 */

	// Up
	public	static KeyCode keyUP = KeyCode.UpArrow;
	// Down
	public	static KeyCode keyDOWN = KeyCode.DownArrow;
	// Left
	public static KeyCode keyLEFT = KeyCode.LeftArrow;
	// Right
	public static KeyCode keyRIGHT = KeyCode.RightArrow;
	
	// Enter walking mode and walk
	public static KeyCode keyMOVE = KeyCode.A;
	// Enter targeting mode and attack
	public static KeyCode keyATTACK = KeyCode.S;
	// Open Inventory to use items?
	public static KeyCode keyITEM = KeyCode.D;
	// Exit other modes, including inventory
	public static KeyCode keyCANCEL = KeyCode.Space;
	
	//The following two keys may belong somewhere else, I just wanted to set a framework
	// Bring up Pause menu
	public static KeyCode keyPAUSE = KeyCode.Escape;
	// Closes the program immediately, saving any states if neccesary
	public static KeyCode keyEXIT = KeyCode.Return;

	// The multiplier for calculating level experience requirements.
	const int EXPERIENCE_FACTOR = 10;

	//These variables are accessed by the HUD
	// The amount of health the player has.
	public static int health;
	// The max amount of health the player can have.
	public static int maxhealth;
	// The player's current level.
	public static int playerLevel;
	// The amount of rupees that the player currently has.
	public static int currency;

	public LayerMask blockingLayer;
	public LayerMask unitsLayer;

	int maxHealth;

	Animator animator;

	BoardManager bm;

//	public int rows;
//	public int columns;

	public GridAura gridSpot;

	GridAura gridInstance;
	// A string variable that we can change while playing the game or outside Play mode.
	public static string myName;

	public GameObject bomb;

	int[] stats;
	
	public static void setHUDhealth(int pHealth)
	{
		health = pHealth;
	}

	public static void setHUDmaxhealth(int pMaxHealth)
	{
		maxhealth = pMaxHealth;
	}

	public static void setHUDplayerlevel(int pPlayerLevel)
	{
		playerLevel = pPlayerLevel;
	}

	public static void setHUDcurrency(int pCurrency)
	{
		currency = pCurrency;
	}

	public static Vector3 currentPosition;

	public static Player player = null;

	public void InitPlayer(){

		//DontDestroyOnLoad (this.gameObject);

		if (player == null) {
			player = this;
		}

		if (myName == null) {
			myName = "Link";
		}

		this.Level = 1;
		this.Health = 4;
		this.Attack = 1;
		this.Defense = 1;
		this.Speed = 1;

		this.Experience = 0;
		this.Currency = 0;

		maxHealth = this.Health;

		stats = new int [] { Health, Attack, Defense, Speed };

		this.Inventory = new List<Item> ();

		state = 0;
		maxmoves = 1.0;

		moves = maxmoves;
		canWalk = true;
		canJump = true;

		setHUDhealth (this.Health);
		setHUDmaxhealth (maxHealth);
		setHUDplayerlevel (this.Level);
		setHUDcurrency (this.Currency);
	}
	
	// Use this for initialization
	void Start () {
		animator = GetComponent<Animator> ();
        TargettedCamera.setTarget(this);
		// Ititializes the player stats.
		InitPlayer ();
	}

	public void CanMove(bool isJump = false){
		Vector3 startPosition = this.transform.position;
		Vector3 endPosition = this.transform.position;

		// If jump is true, then the movement space is 2, otherwise the player can move 1 space.
		int movement = isJump ? 2 : 1;

		if (Input.GetKeyDown (KeyCode.RightArrow)) {
			endPosition = new Vector3 (startPosition.x + movement, startPosition.y);
			animator.Play ("PlayerRightIdle");
		} else if (Input.GetKeyDown (KeyCode.LeftArrow)) {
			endPosition = new Vector3 (startPosition.x - movement, startPosition.y);
			animator.Play ("PlayerLeftIdle");
		} else if (Input.GetKeyDown (KeyCode.UpArrow)) {
			endPosition = new Vector3 (startPosition.x, startPosition.y + movement);
			animator.Play ("PlayerBackwardIdle");
		} else if (Input.GetKeyDown (KeyCode.DownArrow)) {
			endPosition = new Vector3 (startPosition.x, startPosition.y - movement);
			animator.Play ("PlayerForwardIdle");
		} 
		
		BoxCollider2D boxCollider = this.GetComponent<BoxCollider2D> ();
		
		boxCollider.enabled = false;
		
		RaycastHit2D hit = Physics2D.Linecast (startPosition, endPosition, blockingLayer);
		RaycastHit2D hitUnit = Physics2D.Linecast (startPosition, endPosition, unitsLayer);

		boxCollider.enabled = true;

		if (!hit && !hitUnit) {
			this.transform.position = endPosition;
		} else if (hitUnit) {
			if(Input.GetKey(keyATTACK)){
				UseBowAttack(hitUnit);
			}
			else if(Input.GetKey (KeyCode.B)){
				UseBomb(hitUnit);
			}
			else {
				UseSwordAttack (hitUnit);
			}
		} else if (hit) {
			UnlockDoor(hit);
		}
	}

	public override void Move(){

		// Store position to prevent crazy additive movement
		this.transform.position = this.transform.position;
		
		// Checks for directional presses once, and converts them to ints
		// (for future method usage such as movement)
		
		// Store the directional presses of the player
		int x = 0;
		int y = 0;
		
		// Exclusive up
		if (PauseScript.isKeysEnabled && Input.GetKey (keyUP) & ! Input.GetKey (keyDOWN)) {
			y = 1;
		}
		// Exclusive down
		if (PauseScript.isKeysEnabled && Input.GetKey (keyDOWN) & ! Input.GetKey (keyUP)) {
			y = -1;
		}
		// Exclusive right
		if (PauseScript.isKeysEnabled && Input.GetKey (keyRIGHT) & ! Input.GetKey (keyLEFT)) {
			x = 1;
		}
		// Exclusive left
		if (PauseScript.isKeysEnabled && Input.GetKey (keyLEFT) & ! Input.GetKey (keyRIGHT)) {
			x = -1;
		}
		
		// ACTION STATES
		
		// Movement Decision State
		if (state == 1) {
			
			// Creates Jump-Range Grids
			gridInstance = Instantiate(gridSpot, new Vector3((this.transform.position.x + 2*x), (this.transform.position.y + 2*y), 0F), Quaternion.identity) as GridAura;
			
			gridInstance.creator = this.transform.position;
			
			// Moves the player to the appropriate grid.
			if (PauseScript.isKeysEnabled && Input.GetKeyDown (keyMOVE)) {
				walk (x,y,false);
			}
			// Jumps the player to the appropriate grid.
			if (PauseScript.isKeysEnabled && Input.GetKeyDown (keyATTACK)) {
				jump (x,y);
			}
			// Skips the turn.
			if (PauseScript.isKeysEnabled && Input.GetKeyDown (keyCANCEL)) {
				moves = 0;
			}
			
		}

		// Attack Decision State
		if (state == 2) {
			
			// Activates item in slot 1
			if (Input.GetKeyDown (keyMOVE)) {
				// item use code
			}
			// Attacks the target grid with the main weapon (sword?)
			if (Input.GetKeyDown (keyATTACK)) {
				//AttackEnemy ();
			}
			// Activates item in slot 2
			if (Input.GetKeyDown (keyITEM)) {
				// item use code
			}
			
		}
		
		// All Active States
		if (state < 3) {
			
			// if it is your turn, and not in neutral state, show reticule
			// Places the targetting icon
			if (state > 0) {
				
				gridInstance = Instantiate(gridSpot, new Vector3((this.transform.position.x + x), (this.transform.position.y + y), 0F), Quaternion.identity) as GridAura;
				
				gridInstance.creator = this.transform.position;
				
			}
			
			// Returns the player to the neutral state.
			if (Input.GetKeyDown (keyCANCEL)) {
				state = 0;
			}
			
			// Checks if facing purely left.
			if ( (x == -1) & (y == 0) ) {
				animator.Play ("PlayerLeftIdle");
			} 
			// Checks if facing purely right.
			if ( (x == 1) & (y == 0) ) {
				animator.Play ("PlayerRightIdle");
			}
			// Checks if facing purely down.
			if ( (x == 0) & (y == -1) ) {
				animator.Play ("PlayerForwardIdle");
			} 
			// Checks if facing purely up.
			if ( (x == 0) & (y == 1) ) {
				animator.Play ("PlayerBackwardIdle");
			}
		}
		
		// Not My Turn State
		
		if (state == 4) {
			
			//not your turn cleanup
			
		}
		
		// State Checks
		
		if (state == 0) {
			
			// Check for move state
			if ((PauseScript.isKeysEnabled && Input.GetKeyDown (keyMOVE))) {
				state = 1;
			}
			
			// Check for attack state NOT READY
			//		if (Input.GetKeyDown (keyATTACK)) {
			//			state = 2;
			//		}
			
			//		// Check for item state NOT READY
			//		if (Input.GetKeyDown (keyITEM)) {
			//			state = 3;
			//		}
		}
		
		canWalk = true;
		canJump = true;

	}

	// Update is called once per frame
	new void Update () {
//		base.Update ();
//		Move ();

		CanMove (Input.GetKey(KeyCode.D));
		// Check each frame if the player's health has changed.
		setHUDhealth (this.Health);
//		currentPosition = this.transform.position;
	}

	//moves the character occording to the inputs
	void walk(int x, int y,bool jump){

		state = 3;

		//place to go
		Vector3 destination = new Vector3((this.transform.position.x + x),(this.transform.position.y + y), 0);

		//check if goal is a valid location
		if (jump & ! canJump) {
			//collision, do nothing
			//play bump noise
		}
		if (! jump & ! canWalk) {
			//collision, do nothing
			//play bump noise
		}
		//valid location, move to goal
		else {
			this.transform.position = destination;
			moves--;
		}

		state = 1;
		return;

	}

	//temporary jump fix, more interesting behavior to come
	void jump(int x, int y){

		Vector3 prepos = this.transform.position;

		x *= 2;
		y *= 2;

		walk (x, y, true);

		if (this.transform.position != prepos) {

			moves--;

		}
		
	}

	// Allows the player to do damage with their sword attack.
	void UseSwordAttack (RaycastHit2D hitUnit) {
		if (hitUnit.collider.gameObject.tag.Equals ("Enemy")) {
			if (Input.GetKeyDown (KeyCode.RightArrow)) {
				animator.SetTrigger ("PlayerSwordRight");
			} else if (Input.GetKeyDown (KeyCode.LeftArrow)) {
				animator.SetTrigger ("PlayerSwordLeft");
			} else if (Input.GetKeyDown (KeyCode.UpArrow)) {
				animator.SetTrigger ("PlayerSwordBackward");
			} else if (Input.GetKeyDown (KeyCode.DownArrow)) {
				animator.SetTrigger ("PlayerSwordForward");
			} 

			string enemyType;
			if(hitUnit.collider.gameObject.name.Contains("(Clone)")){
				// Gets rid of the (Clone) in the object name.
				enemyType = hitUnit.collider.gameObject.name.Substring(0,hitUnit.collider.gameObject.name.Length - 7);
			}
			else{
				enemyType = hitUnit.collider.gameObject.name;
			}
			switch(enemyType){
			case "Cynthia":
				CalculateDamageDealt(hitUnit.collider.gameObject.GetComponent<Cynthia>());
				if(hitUnit.collider.gameObject.GetComponent<Cynthia>().Health <= 0){
					DefeatEnemy(hitUnit.collider.gameObject.GetComponent<Cynthia>());
					Destroy (hitUnit.collider.gameObject);
				}
				break;
			case "Moblin":
				CalculateDamageDealt(hitUnit.collider.gameObject.GetComponent<Moblin>());
				if(hitUnit.collider.gameObject.GetComponent<Moblin>().Health <= 0){
					DefeatEnemy(hitUnit.collider.gameObject.GetComponent<Moblin>());
					Destroy (hitUnit.collider.gameObject);
				}
				break;
			}
		}
	}

	// Allows the player to do damage with their bow attack.
	void UseBowAttack(RaycastHit2D hitUnit){
		if (hitUnit.collider.gameObject.tag.Equals ("Enemy")) {
			if (Input.GetKeyDown (KeyCode.RightArrow)) {
				animator.SetTrigger ("PlayerBowRight");
			} else if (Input.GetKeyDown (KeyCode.LeftArrow)) {
				animator.SetTrigger ("PlayerBowLeft");
			} else if (Input.GetKeyDown (KeyCode.UpArrow)) {
				animator.SetTrigger ("PlayerBowBackward");
			} else if (Input.GetKeyDown (KeyCode.DownArrow)) {
				animator.SetTrigger ("PlayerBowForward");
			} 
			string enemyType;
			if(hitUnit.collider.gameObject.name.Contains("(Clone)")){
				// Gets rid of the (Clone) in the object name.
				enemyType = hitUnit.collider.gameObject.name.Substring(0,hitUnit.collider.gameObject.name.Length - 7);
			}
			else{
				enemyType = hitUnit.collider.gameObject.name;
			}
			switch(enemyType){
			case "Cynthia":
				CalculateDamageDealt(hitUnit.collider.gameObject.GetComponent<Cynthia>());
				if(hitUnit.collider.gameObject.GetComponent<Cynthia>().Health <= 0){
					DefeatEnemy(hitUnit.collider.gameObject.GetComponent<Cynthia>());
					Destroy (hitUnit.collider.gameObject);
				}
				break;
			case "Moblin":
				CalculateDamageDealt(hitUnit.collider.gameObject.GetComponent<Moblin>());
				if(hitUnit.collider.gameObject.GetComponent<Moblin>().Health <= 0){
					DefeatEnemy(hitUnit.collider.gameObject.GetComponent<Moblin>());
					Destroy (hitUnit.collider.gameObject);
				}
				break;
			}
		}
	}


	public void UseBomb(RaycastHit2D hitUnit){
		int bombDamage = 5;

		bool hasBomb = false;

		foreach (Item item in Inventory) {
			if(item.Name.Equals ("Bomb")){
				Inventory.Remove (item);
				hasBomb = true;
				break;
			}
		}
		
		if (hitUnit.collider.gameObject.tag.Equals ("Enemy") && hasBomb) {
			// Spawns a bomb on the enemy space.
			GameObject bombObj = Instantiate(bomb, hitUnit.collider.gameObject.transform.position, Quaternion.identity) as GameObject;
			// Gets the animation controller associated with the bomb object.
			Animator bombAnimator = bombObj.GetComponent<Animator>();
			// Plays the explosion animation for the bomb.
			bombAnimator.Play ("BombExplosion");
			if (Input.GetKeyDown (KeyCode.RightArrow)) {
				animator.SetTrigger ("PlayerBombRight");
			} else if (Input.GetKeyDown (KeyCode.LeftArrow)) {
				animator.SetTrigger ("PlayerBombLeft");
			} else if (Input.GetKeyDown (KeyCode.UpArrow)) {
				animator.SetTrigger ("PlayerBombBackward");
			} else if (Input.GetKeyDown (KeyCode.DownArrow)) {
				animator.SetTrigger ("PlayerBombForward");
			} 
			string enemyType;
			if(hitUnit.collider.gameObject.name.Contains("(Clone)")){
				// Gets rid of the (Clone) in the object name.
				enemyType = hitUnit.collider.gameObject.name.Substring(0,hitUnit.collider.gameObject.name.Length - 7);
			}
			else{
				enemyType = hitUnit.collider.gameObject.name;
			}
			switch(enemyType){
			case "Cynthia":
				hitUnit.collider.gameObject.GetComponent<Cynthia>().Health -= bombDamage;
				if(hitUnit.collider.gameObject.GetComponent<Cynthia>().Health <= 0){
					DefeatEnemy(hitUnit.collider.gameObject.GetComponent<Cynthia>());
					Destroy (hitUnit.collider.gameObject);
				}
				break;
			case "Moblin":
				hitUnit.collider.gameObject.GetComponent<Moblin>().Health -= bombDamage;
				if(hitUnit.collider.gameObject.GetComponent<Moblin>().Health <= 0){
					DefeatEnemy(hitUnit.collider.gameObject.GetComponent<Moblin>());
					Destroy (hitUnit.collider.gameObject);
				}
				break;
			}
			// Destroy the bomb after 1.8 seconds (The approximate time of its animation).
			Destroy (bombObj, 1.8F);
		}
	}
	
    	//Restart reloads the scene when called.
    	private void Restart()
    	{
			// Resets the player's position before starting the new level.
			this.transform.position = new Vector3(0,0);
			// Resets the player's animation to forward idle.
			this.animator.Play ("PlayerForwardIdle");
    	    Application.LoadLevel(Application.loadedLevel);
    	}

	// Methods for GridAura to disable movement
//	public void stopWalk(){

//		canWalk = false;

//	}

//	public void stopJump(){
		
//		canJump = false;
		
//	}

	// Randomizes the stat bonuses when leveling.
	void RandomizeStatBonuses() {
		// A maximum of 4 stat bonuses can occur when leveling.
		int maxBonuses = 4;

		int index;
		for(int i = 0; i < maxBonuses; i++){
			// Randomizes the stat that will be increased.
			index = (int) (Random.value * stats.Length);
			// Increases the stat at the generated index by 1.
			stats[index]++;
			switch(index){
			case 0: 
				Health++;
				break;
			case 1:
				Attack++;
				break;
			case 2:
				Defense++;
				break;
			case 3:
				Speed++;
				break;
			}
			setHUDhealth(Health);
		}
	}

	// Updates the player's level and stats.
	void LevelUp() {
		// Increases the player's level by 1.
		this.Level++;
		setHUDplayerlevel (this.Level);
		int previousHealth = this.Health;
		// Increases the player's stats.
		RandomizeStatBonuses ();
		// The amount of health added upon leveling.
		int addedHealth = this.Health - previousHealth;
		// Adjusts maxHealth if the Health stat was increased.
		if (addedHealth > 0) {
			maxHealth += addedHealth;
			setHUDmaxhealth (maxHealth);
		}
	}

	public void DefeatEnemy(Unit enemy) {
		// Figures out how much experience is required for the player to level up.
		int nextLevel = (int) Mathf.Pow (this.Level, 2) * EXPERIENCE_FACTOR;
		// If the player still needs experience after defeating the enemy, then simply update
		// the player's experience.
		if ((this.Experience + enemy.Experience) < nextLevel) {
			this.Experience += enemy.Experience;
		} else {
			// Else, add the experience and increment the player's level.
			this.Experience += enemy.Experience;
			// Level up the player until they don't meet the nextLevel experience threshold.
			while(this.Experience >= nextLevel){
				LevelUp();
				nextLevel = (int) Mathf.Pow (this.Level, 2) * EXPERIENCE_FACTOR;
			}
		}
	}
	
	void OnTriggerEnter2D(Collider2D collider){

		//Check if the tag of the trigger collided with is Exit.
        	if (collider.gameObject.tag.Equals ("Exit") )
       	 	{
        	   //Invoke the Restart function to start the next level with a delay of 1 second.
         	   Invoke("Restart", 0);
        	}

		if (collider.gameObject.tag.Equals ("Item")) {
			// Adds the item to the player's inventory.
			Item item = new Item(collider.gameObject.name);
			if(item.Name.Contains("Rupee") || item.Name.Contains ("Heart")) {
				UseItem(item);
			}
			else {
				this.Inventory.Add (item);
			}
			// Removes the item from the game board.
			Destroy (collider.gameObject);
		}
	}

	void UseItem(Item item) {
		// Use the item.
		item.Use (this);
		// If the item healed any health, check if it healthed over the player's max allowed health.
		if (this.Health > maxHealth) {
			this.Health = maxHealth;
		}
		// Remove the item from the player's inventory.
		Inventory.Remove(item);
	}
	
	public void CalculateDamageDealt(Unit enemy){
		// If the player's attack stat is greater than the enemy's defense, then set the new damage amount.
		// The player's attack must be at least 2 more than the enemy's defense for the damage to be more
		// than 1.
		int damage = (this.Attack > enemy.Defense) ? this.Attack - enemy.Defense : 1;
		enemy.Health -= damage;
	}

	public void UnlockDoor(RaycastHit2D door){
		bool hasKey = false;
		Item key = null;
		foreach (Item item in Inventory) {
			if(item.Name.Equals("Key")){
				hasKey = true;
				key = item;
			}
		}

		if (door.collider.gameObject.name == "Door(Clone)" && hasKey) {
			this.Inventory.Remove(key);
			Destroy (door.collider.gameObject);
		}
	}
}
