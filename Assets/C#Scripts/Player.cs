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
	public static bool PLAYERS_TURN;
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

	// The floor the player's currently on.
	public static int floorLevel = 1;
	
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

	public static int INVENTORY_CAPACITY = 18;
	public static int basicItemCount = 0;

	Animator animator;
	
	// A string variable that we can change while playing the game or outside Play mode.
	public static string myName;

	public GameObject bomb;
	public GameObject arrow;
	public GameObject smallRupee;
	public GameObject mediumRupee;
	public GameObject largeRupee;
	public GameObject heart;

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
		if (player == null) {
			player = this;
		}

		if (myName == null) {
			myName = "Link";
		}

		this.Level = 1;
		this.Health = 6;
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

		Player.PLAYERS_TURN = true;
	}
	
	// Use this for initialization
	void Start () {
		animator = GetComponent<Animator> ();
        TargettedCamera.setTarget(this);
		// Ititializes the player stats.
		InitPlayer ();
	}

	public void CanMove(bool isJump = false){
		if (PLAYERS_TURN) {
			bool actionPerformed = false;

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
				if(endPosition != startPosition){
					actionPerformed = true;
				}
			} else if (hitUnit) {
				if(hitUnit.collider.tag.Contains ("NPC")){
					FindObjectOfType<ShopScript>().OpenShopScreen();
				}
				else{
					if (Input.GetKey (keyATTACK)) {
						UseBowAttack (hitUnit);
						actionPerformed = true;
					} else if (Input.GetKey (KeyCode.B)) {
						UseBomb (hitUnit);
						actionPerformed = true;
					} else if(!isJump){
						UseSwordAttack (hitUnit);
						actionPerformed = true;
					}
				}
			} else if (hit) {
				if(Input.GetKey(keyATTACK)){
					UseDig(hit);
					actionPerformed = true;
				}
				else {
					UnlockDoor (hit);
				}

			}

			if(actionPerformed){
				Player.PLAYERS_TURN = false;
			}
		}
	}

	public override void Move(){

	}

	IEnumerator WaitForEnemies(){
		yield return new WaitForEndOfFrame();
		Player.PLAYERS_TURN = true;
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
			DamageEnemy(hitUnit);
		}
	}

	// Allows the player to do damage with their bow attack.
	void UseBowAttack(RaycastHit2D hitUnit){
		bool hasArrow = false;
		
		foreach (Item item in Inventory) {
			if(item.Name.Equals ("Arrow")){
				FindObjectOfType<InventoryScript>().ClearSlot(this.Inventory.IndexOf(item));
				Inventory.Remove (item);
				hasArrow = true;
				basicItemCount--;
				break;
			}
		}

		if (hitUnit.collider.gameObject.tag.Equals ("Enemy") && hasArrow) {
			int arrowDamage = 1;
			// Spawns an arrow when attacking with the bow.
			if (Input.GetKeyDown (KeyCode.RightArrow)) {
				animator.SetTrigger ("PlayerBowRight");
			} else if (Input.GetKeyDown (KeyCode.LeftArrow)) {
				animator.SetTrigger ("PlayerBowLeft");
			} else if (Input.GetKeyDown (KeyCode.UpArrow)) {
				animator.SetTrigger ("PlayerBowBackward");
			} else if (Input.GetKeyDown (KeyCode.DownArrow)) {
				animator.SetTrigger ("PlayerBowForward");
			} 
			DamageEnemy(hitUnit, arrowDamage);
		}
	}

	// Allows the player to use bombs.
	public void UseBomb(RaycastHit2D hitUnit){
		int bombDamage = 2;

		bool hasBomb = false;

		foreach (Item item in Inventory) {
			if(item.Name.Equals ("Bomb")){
				FindObjectOfType<InventoryScript>().ClearSlot(this.Inventory.IndexOf(item));
				Inventory.Remove (item);
				hasBomb = true;
				basicItemCount--;
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
			DamageEnemy(hitUnit, bombDamage);
			// Destroy the bomb after 1.8 seconds (The approximate time of its animation).
			Destroy (bombObj, 1.8F);
		}
	}

	// Allows the player to dig up and remove rocks.
	void UseDig(RaycastHit2D rock){
		if (rock.collider.gameObject.name.Contains("Rock")) {
			if (Input.GetKeyDown (KeyCode.RightArrow)) {
				animator.SetTrigger ("PlayerDigRight");
			} else if (Input.GetKeyDown (KeyCode.LeftArrow)) {
				animator.SetTrigger ("PlayerDigLeft");
			} else if (Input.GetKeyDown (KeyCode.UpArrow)) {
				animator.SetTrigger ("PlayerDigBackward");
			} else if (Input.GetKeyDown (KeyCode.DownArrow)) {
				animator.SetTrigger ("PlayerDigForward");
			} 

			float lootProbability = Random.value;
			if(lootProbability > 0.15 && lootProbability < 0.3){
				Instantiate (heart, rock.transform.position, Quaternion.identity);
			}
			if(lootProbability >= 0.3 && lootProbability < 0.70){
				Instantiate(smallRupee, rock.transform.position, Quaternion.identity);
			}
			else if(lootProbability >= 0.70 && lootProbability < 0.95){
				Instantiate(mediumRupee, rock.transform.position, Quaternion.identity);
			}
			else if(lootProbability >= 0.95){
				Instantiate(largeRupee, rock.transform.position, Quaternion.identity);
			}

			Destroy (rock.collider.gameObject);
		}
	}
	
    	//Restart reloads the scene when called.
    	private void Restart()
    	{
			// Resets the player's position before starting the new level.
			this.transform.position = new Vector3(0,0);
			// Resets the player's animation to forward idle.
			this.animator.Play ("PlayerForwardIdle");
			Player.floorLevel++;
    	    Application.LoadLevel(Application.loadedLevel);
    	}

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

		if(enemy.Currency > 0){
			this.Currency += enemy.Currency;
			this.setHUDcurrency(this.Currency);
		}
	}
	
	void OnTriggerEnter2D(Collider2D collider){

		//Check if the tag of the trigger collided with is Exit.
        	if (collider.gameObject.tag.Equals ("Exit"))
       	 	{
        	   //Invoke the Restart function to start the next level with a delay of 1 second.
         	   Invoke("Restart", 0);
        	}

		if (collider.gameObject.tag.Equals ("Item")) {
			// Adds the item to the player's inventory.
			Item item = new Item (collider.gameObject.name);
			if (item.Name.Contains ("Rupee") || item.Name.Contains ("Heart")) {
				UseItem (item);
				Destroy (collider.gameObject);
			} else if (basicItemCount < INVENTORY_CAPACITY - 1){
				this.Inventory.Add (item);
				basicItemCount++;
				// Removes the item from the game board.
				Destroy (collider.gameObject);
			}
		} 
		if (collider.gameObject.tag.Equals ("KeyItem")) {
			Item item = new Item (collider.gameObject.name);
			this.Inventory.Add (item);
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
	}
	
	public void CalculateDamageDealt(Unit enemy, int additionalDamage = 0){
		// If the player's attack stat is greater than the enemy's defense, then set the new damage amount.
		// The player's attack must be at least 2 more than the enemy's defense for the damage to be more
		// than 1.
		int damage = ((this.Attack + additionalDamage) > enemy.Defense) ? (this.Attack + additionalDamage) - enemy.Defense : 1;
		enemy.Health -= damage;
	}

	void DamageEnemy(RaycastHit2D hitUnit, int additionalDamage = 0){
		string enemyType;
		if(hitUnit.collider.gameObject.name.Contains("(Clone)")){
			// Gets rid of the (Clone) in the object name.
			enemyType = hitUnit.collider.gameObject.name.Substring(0,hitUnit.collider.gameObject.name.Length - 7);
		}
		else{
			enemyType = hitUnit.collider.gameObject.name;
		}
		Unit enemy = null;
		switch(enemyType){
		case "Cynthia":
			enemy = hitUnit.collider.gameObject.GetComponent<Cynthia>();
			CalculateDamageDealt(enemy, additionalDamage);
			if(hitUnit.collider.gameObject.GetComponent<Cynthia>().Health <= 0){
				DefeatEnemy(hitUnit.collider.gameObject.GetComponent<Cynthia>());
				Destroy (hitUnit.collider.gameObject);
			}
			break;
		case "Moblin":
			enemy = hitUnit.collider.gameObject.GetComponent<Moblin>();
			CalculateDamageDealt(enemy, additionalDamage);
			if(hitUnit.collider.gameObject.GetComponent<Moblin>().Health <= 0){
				DefeatEnemy(hitUnit.collider.gameObject.GetComponent<Moblin>());
				Destroy (hitUnit.collider.gameObject);
			}
			break;
		case "Sableye":
			CalculateDamageDealt(hitUnit.collider.gameObject.GetComponent<Sableye>());
			if(hitUnit.collider.gameObject.GetComponent<Sableye>().Health <= 0){
				DefeatEnemy(hitUnit.collider.gameObject.GetComponent<Sableye>());
				Destroy (hitUnit.collider.gameObject);
			}
			break;
		}
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
			FindObjectOfType<InventoryScript>().ClearSlot(this.Inventory.IndexOf(key));
			this.Inventory.Remove(key);
			Destroy (door.collider.gameObject);
		}
	}

	// Update is called once per frame
	new void Update () {
		CanMove (Input.GetKey(keyMOVE));
		StartCoroutine(WaitForEnemies ());
		// Check each frame if the player's health has changed.
		setHUDcurrency (this.Currency);
		setHUDhealth (this.Health);
		FindObjectOfType<InventoryScript>().updateInventory(this.Inventory);
	}
}
