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
	
	}
}
