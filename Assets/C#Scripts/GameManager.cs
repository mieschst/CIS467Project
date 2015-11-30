using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	//public GameObject player;
	//public GameObject cynthia;
    //public BoardManager boardScript;
    private int level = 1;

	public static bool isHardMode { get; set; }

    public static GameManager instance = null;

    // Use this for initialization
    void Start () {
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

        //boardScript = GetComponent<BoardManager>();
        Init();
        //boardScript.LevelSelector(level);
    }

    //Called each time a level was loaded
    public void OnLevelWasLoaded(int index)
    {
        //Add one to our level number.
        level++;
        //Call Init to initialize the level.
        Init();
    }

    void Init()
    {
        //boardScript.LevelSelector(level);
    }

    // Update is called once per frame
    void Update () {

	}
}
