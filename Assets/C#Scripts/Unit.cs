using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class Unit : MonoBehaviour {
	
	// Action mode of the unit [0 = idle, 1 = movement, 2 = attacking, 3 = midmanuever, 4 = turnless]
	int stat;
	// Maximum amount of moves that an entity can make in one turn
	double maxmvs;
	// Remaining turns. (Some actions take partial moves, negative moves results in skipped turns
	double mvs;

	// variables to be adjusted by collision check
	bool walk;
	bool jump;

    public bool isTurn { get; set; }

	public int Level { get; set; }

	public int Health { get; set; }

	public int Attack { get; set; }

	public int Defense { get; set; }

	public int Speed { get; set; }

	public int Experience { get; set; }

	public int Currency { get; set; }

	// Every unit has an inventory.
	public List<Item> Inventory { get; set; }

	// State Property
	public int state
	{
		get { return stat; }
		set { stat = value; }
	}

	// Maxmoves Property
	public double maxmoves
	{
		get { return maxmvs; }
		set { maxmvs = value; }
	}

	// xmoves Property
	public double moves
	{
		get { return mvs; }
		set { mvs = value; }
	}

	// canWalk Property
	public bool canWalk
	{
		get { return walk; }
		set { walk = value; }
	}

	// canJump Property
	public bool canJump
	{
		get { return jump; }
		set { jump = value; }
	}

	// Every unit must have an algorithm or method of moving on the board.
	public abstract void Move();

	// Use this for initialization
	void Start () {
        isTurn = true;
	}

    IEnumerator CameraWait()
    {
        for(int i=0; i<5; i++)
            yield return new WaitForSeconds(Random.value/5);
    }

    // Update is called once per frame
    public void Update () {

        Unit[] creatures = FindObjectsOfType(typeof(Unit)) as Unit[];

        // returns to active state if it is your turn
        if ((state == 4) & (isTurn)) {
            TargettedCamera.setTarget(this);
            state = 0;
                foreach (Unit guy in creatures)
                {
                    if(guy != this)
                    {
                        guy.isTurn = false;
                        guy.state = 4;
                    }
                }
            CameraWait();
		}

        // debug turn incrementor, NOT NEEDED ANYMORE?!?
        //moves += 0.01;

        // If all actions have been taken this turn, end the turn
        if (moves <= 0)
        {
            isTurn = false;
            state = 4;
            bool hasTurn = true;

            while (hasTurn)
            {
                foreach (Unit guy in creatures)
                {

                    //pass your turn to the first eligible target
                    if ((Random.value < (guy.Speed * 0.1f)) & (this != guy) & (guy.moves > 0) & hasTurn)
                    {
                        guy.isTurn = true;
                        hasTurn = false;
                    }
                }
            }

            isTurn = false;
            state = 4;
            // If you are the only entity, it is always your turn
            if (creatures.Length == 1)
            {
                isTurn = true;
            }

        }

        //If nobody is doing anything, restore moves
        bool restore = true;
        foreach (Unit guy in creatures)
        {
            if (guy.isTurn)
                restore = false;
        }
        if (restore)
             foreach (Unit guy in creatures)
            {
                guy.moves += 1;
                // Make sure moves are limited by maxmoves
                if (guy.moves > guy.maxmoves)
                {
                    guy.moves = guy.maxmoves;
                }
            }
	}
}
