using UnityEngine;
using System;
using System.Collections;
using Random = UnityEngine.Random;
using System.Collections.Generic;

public class BoardManager : MonoBehaviour {
	
	public GameObject floorTile;
	public GameObject waterTile;
	public GameObject seaTile;
	public GameObject reefTile;
	public GameObject lavaTile;
	public GameObject pitTile;
	public GameObject[] wallTiles;

	public GameObject[] rockTiles;
	
	public GameObject[] basicItems;
	public GameObject[] keyItems;
	public GameObject[] enemies;

	public GameObject ladder;
	public GameObject lockedDoor;
	public GameObject merchant;
	
	int rows;
	int columns;

	public const int BOSS_LEVEL = 10;

	// Sets the depth of the wall tiles around the board to 2.
	const int WALL_DEPTH = 2;
	// Sets the depth of the sea tiles to 5.
	const int SEA_DEPTH = 5;

	List<Vector3> filledPositions;

    	public GameObject player;

    	public List<Vector3> OpenList = new List<Vector3>();
    	public List<Vector3> ClosedList = new List<Vector3>();
    	Dictionary<Vector3, Vector3> cameFrom = new Dictionary<Vector3, Vector3>();

   	//List<Vector3> filledPositions = new List<Vector3>();
   	List<Vector3> blockedPositions = new List<Vector3>();

	private Transform boardTiles;
	public static Transform boardItems;

	public void SetupBoard(){
		// The maximum additional height the board can have.
		int maxAdditionalBoardHeight = 6;
		// The maximum additional width the board can have.
		int maxAdditionalBoardWidth = 6;
		// The minimum width x height dimensions.
		int minDimension = 8;

		// Generates a random size between 8 and the additional height allowed.
		rows = (int)(Random.value * maxAdditionalBoardHeight) + minDimension;
		// Generates a random size between 8 and the additional width allowed.
		columns = (int)(Random.value * maxAdditionalBoardWidth) + minDimension;
	
        boardTiles = new GameObject("BoardTiles").transform;

		// Draws the floor tiles.
		DrawFloorTiles ();
		// Draws the wall tiles lining the floor.
		DrawWall ();
		// Draws the sea and reef tiles lining the outer wall layer.
		DrawSeaWithReefs ();
    }

	void DrawFloorTiles (){
		// Adds the floor tiles to the game board.
		for (int i = 0; i < rows; i++)
		{
			for (int j = 0; j < columns; j++)
			{
				// Creates a new tile game object at position (j,i).
				GameObject newTile = Instantiate(floorTile, new Vector2(j, i), Quaternion.identity) as GameObject;
				
				// Adds the new tile to GameObject called 'BoardTiles' to help reduce clutter in the
				// hierarchy.
				newTile.transform.SetParent(boardTiles);
			}
		}
	}

	void DrawWall(){
		// The minimum position where we can generate wall tiles.
		int min = -1 * WALL_DEPTH;
		// The maximum position where we can generate wall tiles.
		int max = WALL_DEPTH;
		// The position to spawn the merchant.
		Vector3 merchantPosition = new Vector3 ((int)(columns / 2), rows);
		// Adds the wall tiles to the game board.
		for (int i = min; i < rows + max; i++)
		{
			for (int j = min; j < columns + max; j++)
			{
				if (i < 0 || i >= rows || j < 0 || j >= columns)
				{
					if(i == rows && j == (int)(columns/2) && Player.floorLevel % BOSS_LEVEL != 0){
						// Instantiates the merchant.
						Instantiate(merchant, merchantPosition, Quaternion.identity);
						// Instantiates the floor tile for the merchant to stand on.
						GameObject newTile = Instantiate(floorTile, merchantPosition, Quaternion.identity) as GameObject;
						// Adds the floor tile to the Transform object containing all the board tiles.
						newTile.transform.SetParent(boardTiles);
					}
					else{
						// Generate a random index.
						int index = (int) (Random.value * wallTiles.Length);
						// Instantiate a new wall tile from the list of wall tiles.
						GameObject newTile = Instantiate(wallTiles[index], new Vector2(j, i), Quaternion.identity) as GameObject;
						// Adds the wall tile to the collection of board tiles.
						newTile.transform.SetParent(boardTiles);
					}
				}
			}
		}
	}

	void DrawSeaWithReefs(){
		// The maximum position to generate a sea or reef tile.
		int max = WALL_DEPTH + SEA_DEPTH;
		// The minimum position to generate a sea or reef tile.
		int min = -1 * max;

		for (int i = min; i < rows + max; i++)
		{
			for (int j = min; j < columns + max; j++)
			{
				// Checks if we are at one of the valid positions.
				if (i < (-1 * WALL_DEPTH) || i >= (rows + WALL_DEPTH) || j < (-1 * WALL_DEPTH) || j >= (columns + WALL_DEPTH))
				{
					// Instantiates a new sea tile.
					GameObject newTile = Instantiate(seaTile, new Vector2(j, i), Quaternion.identity) as GameObject;
					// Adds the sea tile to the collection of board tiles.
					newTile.transform.SetParent(boardTiles);
					// Generates a random number.
					float reefProbability = Random.value;
					// If the generated probability is 0.95 or greater, then create a new reef tile.
					if(reefProbability >= 0.95){
						// Instantiates a new reef tile.
						GameObject reefObj = Instantiate(reefTile, new Vector2(j, i), Quaternion.identity) as GameObject;
						// Adds the reef tile to the collection fo board tiles.
						reefObj.transform.SetParent(boardTiles);
					}
				}
			}
		}
	}

	void SetupBasicLevel(){
		// Assigns values to the column and row variables.
		SetupBoard ();
		
		filledPositions = new List<Vector3> ();
		
		// Adds a ladder right corner of the moveable section of the board.
		Instantiate (ladder, new Vector3 (columns-1, rows-1), Quaternion.identity);
		Instantiate (lockedDoor, new Vector3 (columns-1, rows-1), Quaternion.identity);
		
		GenerateKeyItems ();

		int enemyCounter = (int)Math.Log(Player.floorLevel, 2);
		
		for (int i = 0; i < enemies.Length - 1; i++) {
			SpawnEnemies (i, enemyCounter);
		}
		
		// May generate items up to the specified number and place them on the board.
		GenerateBasicItems ((rows+columns)/3);
		
		GenerateBlockingObjects (waterTile, 0.01F);
		GenerateBlockingObjects (lavaTile, 0.01F);
		for (int i = 0; i < rockTiles.Length; i++) {
			GenerateBlockingObjects (rockTiles [(int)(Random.value * rockTiles.Length)], 0.01F);
		}
		GenerateBlockingObjects (pitTile, 0.01F);
	}

	void SetupBossLevel(){
		SetupBoard ();

		filledPositions = new List<Vector3> ();
        	blockedPositions = new List<Vector3>();
        	cameFrom = new Dictionary<Vector3, Vector3>();

		// Adds a ladder right corner of the moveable section of the board.
		Instantiate (ladder, new Vector3 (columns-1, rows-1), Quaternion.identity);
		Instantiate (lockedDoor, new Vector3 (columns-1, rows-1), Quaternion.identity);

		DrawFloorTiles ();
		DrawWall ();
		DrawSeaWithReefs ();
		SpawnEnemies (enemies.Length - 1, 1);

		GenerateBasicItems ((rows+columns)/3);

		GenerateBlockingObjects (lavaTile, 0.01F);
		for (int i = 0; i < rockTiles.Length; i++) {
			GenerateBlockingObjects (rockTiles [(int)(Random.value * rockTiles.Length)], 0.01F);
		}
	}

	// Use this for initialization
	void Start () {
		if (Player.floorLevel % BOSS_LEVEL == 0) {
			SetupBossLevel();
		} else {
			SetupBasicLevel ();
		}
		GenerateBlockingObjects (pitTile, 0.01F);

        	if (CheckPaths() == false)
            		Start();
	}

	void GenerateKeyItems(){
		foreach (GameObject keyItem in keyItems) {
			// Values between 1 and the number of rows-1.
			float x = (int)(Random.value * (columns-2)+1);
			// Values between 1 and the number of columns-1.
			float y = (int)(Random.value * (rows-2)+1);

			// The position on the board to place the item.
			Vector3 location = new Vector3(x,y);
			
			// Checks if the random position hasn't been added already.
			if(filledPositions.Contains(location) == false){
				// Add the position to the list.
				filledPositions.Add(location);
				// Instantiate the new item GameObject.
				Instantiate (keyItem, location, Quaternion.identity);
			}
		}
	}

	// Generates an item and places it at some random position on the board. Note: The floor lining the wall
	// will not have items in it. This is so that the player doesn't get blocked when we add obstacles.
	void GenerateBasicItems(int numberOfItems){
		boardItems = new GameObject ("BoardItems").transform;

		// Adds items at random positions on the board.
		for (int i = 0; i < numberOfItems; i++) {
			// Values between 1 and the number of rows-1.
			float x = (int)(Random.value * (columns-2)+1);
			// Values between 1 and the number of columns-1.
			float y = (int)(Random.value * (rows-2)+1);

			// The position on the board to place the item.
			Vector3 location = new Vector3(x,y);

			// Checks if the random position hasn't been added already.
			if(!filledPositions.Contains(location)){
				// Add the position to the list.
				filledPositions.Add(location);
				// Instantiate the new item GameObject.
				GameObject newItem = Instantiate (RandomItem(), location, Quaternion.identity) as GameObject;
				// Add the item to a parent GameObject to reduce cluster in the hierarchy.
				newItem.transform.SetParent(boardItems);
			}
		}
	}

	GameObject RandomItem(){
		// Generates a random number between 0 and the size of the list of items.
		int randomNum = (int) (Random.value * basicItems.Length);
		GameObject obj = basicItems[randomNum];
		return obj;
	}

	void SpawnEnemies (int type, int numToSpawn){
		// Spawns x number of enemies at random positions on the game board.
		for(int i = 0; i < numToSpawn; i++) {
			float x = (int)(Random.value * columns-2) + 1;
			float y = (int)(Random.value * rows-2) + 1;
			Vector3 position = new Vector3(x,y);
			if(!filledPositions.Contains(position)) {
				Instantiate(enemies[type], position, Quaternion.identity);
				filledPositions.Add (position);
			}
		}
	}


    bool CheckPaths()
    {
        OpenList.Clear();
        ClosedList.Clear();
        List<Vector3> neighbors = new List<Vector3>();
        var frontier = new Queue<Vector3>();

        Vector3 goal = new Vector3(rows - 1, columns - 1);

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                if(!blockedPositions.Contains(new Vector3(i, j)))
                OpenList.Add(new Vector3(i, j));
            }
        }

        frontier.Enqueue(OpenList[0]);
        cameFrom[OpenList[0]] = OpenList[0];

        while(frontier.Count > 0)
        {
            Vector3 current = frontier.Dequeue();
            ClosedList.Add(current);

            if (current == goal)
                break;

            foreach(var next in GetNeighbors(current))
            {
                if(OpenList.Contains(next))
                {
                    if(!frontier.Contains(next) && !ClosedList.Contains(next))
                    {
                        frontier.Enqueue(next);
                        cameFrom[next] = current;
                    }
                }
            }
        }

        return CheckCameFrom(cameFrom, OpenList[0], goal);

    }

    bool CheckCameFrom(Dictionary<Vector3,Vector3> cameFrom, Vector3 start, Vector3 goal)
    {
        Vector3 current = goal;
        Vector3 temp = current;
        int infiniteCounter = 0;

        while (current != start)
        {
            temp = cameFrom[current];
            current = temp;
            infiniteCounter++;

            if (infiniteCounter > cameFrom.Count)
                return false;
        }        

        return true;
    }

    List<Vector3> GetNeighbors(Vector3 current)
    {
        List<Vector3> ret = new List<Vector3>();

        ret.Add(new Vector3(current.x + 1, current.y, current.z));
        ret.Add(new Vector3(current.x - 1, current.y, current.z));
        ret.Add(new Vector3(current.x, current.y + 1, current.z));
        ret.Add(new Vector3(current.x, current.y - 1, current.z));
        ret.Add(new Vector3(current.x + 1, current.y + 1, current.z));
        ret.Add(new Vector3(current.x + 1, current.y - 1, current.z));
        ret.Add(new Vector3(current.x - 1, current.y + 1, current.z));
        ret.Add(new Vector3(current.x - 1, current.y - 1, current.z));

        return ret;
    }    

    // Update is called once per frame
    void Update () {

	}
    
	void GenerateBlockingObjects(GameObject blockingObject, float frequency){
		// Generates blocking objects throughout the game board as a percentage of the game board.
		for (int i = 0; i < (rows * columns) * frequency; i++) {
			float x = (int)(Random.value * (columns-2) + 1);
			float y = (int)(Random.value * (rows-2) + 1);
			if(!filledPositions.Contains(new Vector3(x,y))) {
				Instantiate(blockingObject, new Vector3(x,y),Quaternion.identity);
				filledPositions.Add (new Vector3(x,y));
                blockedPositions.Add(new Vector3(x, y));
			}
		}
	}
}
