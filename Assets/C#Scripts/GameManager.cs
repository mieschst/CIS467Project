using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	//public GameObject player;
	//public GameObject cynthia;

    //public BoardManager boardScript;

    //public static int level = 1;

	public static bool isHardMode { get; set; }

    public static GameManager instance = null;

    // Use this for initialization
    void Start()
    {

        //Instantiate (player);

        //Instantiate (cynthia);

        //Check if instance already exists
        if (instance == null)

            //if not, set instance to this
            instance = this;

        //If instance already exists and it's not this:
        else if (instance != this)

            //Then destroy this
            Destroy(gameObject);
        //        
        //
        //        //Sets this to not be destroyed when reloading scene
        //        DontDestroyOnLoad(gameObject);

        Player.floorLevel++;

    }

    // Update is called once per frame
    void Update () {

	}
}
