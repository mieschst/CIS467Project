using UnityEngine;
using System.Collections;

public class LoadGame : MonoBehaviour {

	public GameObject gameManager;

	// Use this for initialization
	void Start () {
        //When this is in, the ghost instance of the game goes away, but so does the old board.
        //Fixed BoardSetup, however, player and enemies still no longer show up
        if (GameManager.instance == null) 
		    Instantiate (gameManager);
	}
	
	// Update is called once per frame
	void Update () {

	}
}
