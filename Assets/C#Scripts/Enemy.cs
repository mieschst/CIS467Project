using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

public abstract class Enemy : Unit {

	public const int FRAMES_PER_TURN = 60;

	public LayerMask blockingLayer;
	public LayerMask unitsLayer;

	public abstract void InitEnemy(int level, bool isHardMode);

	public abstract void CalculateStats(int level, bool isHardMode);

	// Use this for initialization
	void Start () {
	
	}

	public abstract void CalculateDamageDealt(Unit player);

	// Update is called once per frame
	void Update () {
	
	}
}
