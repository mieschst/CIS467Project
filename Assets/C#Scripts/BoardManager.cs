using UnityEngine;
using System;
using System.Collections;
using Random = UnityEngine.Random;
using System.Collections.Generic;

public class BoardManager : MonoBehaviour {
	
	public GameObject floorTile;
	public GameObject wallTile;
	public GameObject waterTile;
	public GameObject pitTile;

	public GameObject[] rockTiles;
	
	public GameObject[] basicItems;
	public GameObject[] keyItems;
	public GameObject[] enemies;

	public GameObject ladder;
	public GameObject lockedDoor;
	
	int rows;
	int columns;


    public GameObject player;

    public List<Vector3> OpenList = new List<Vector3>();
    public List<Vector3> ClosedList = new List<Vector3>();
    Dictionary<Vector3, Vector3> cameFrom = new Dictionary<Vector3, Vector3>();
    //public List<int> HValues = new List<int>();

    List<Vector3> filledPositions = new List<Vector3>();
    List<Vector3> blockedPositions = new List<Vector3>();

	private Transform boardTiles;
	private Transform boardItems;

    //List of all possible board positions
    //private List<Vector3> boardPositions = new List<Vector3>();
	
	public void SetupBoard(){

		int maxAdditionalBoardHeight = 6;
		int maxAdditionalBoardWidth = 6;
		int minDimension = 6;

		rows = (int)(Random.value * maxAdditionalBoardHeight) + minDimension;
		columns = (int)(Random.value * maxAdditionalBoardWidth) + minDimension;
	
        boardTiles = new GameObject("BoardTiles").transform;

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

        // Adds the wall tiles to the game board.
        for (int i = -2; i <= rows+1; i++)
        {
            for (int j = -2; j <= columns+1; j++)
            {
                if (i < 0 || i >= rows || j < 0 || j >= columns)
                {
                    GameObject newTile = Instantiate(wallTile, new Vector2(j, i), Quaternion.identity) as GameObject;
                    newTile.transform.SetParent(boardTiles);
                }
            }

        }

        // Adds a ladder right corner of the moveable section of the board.
        //Instantiate(ladder, new Vector3(rows - 1, columns - 1, 0), Quaternion.identity);

 }
    
	// Use this for initialization
	void Start () {

		// Assigns values to the column and row variables.
		SetupBoard ();

		filledPositions = new List<Vector3> ();
        blockedPositions = new List<Vector3>();
        cameFrom = new Dictionary<Vector3, Vector3>();

		// Adds a ladder right corner of the moveable section of the board.
		Instantiate (ladder, new Vector3 (columns-1, rows-1), Quaternion.identity);
		Instantiate (lockedDoor, new Vector3 (columns-1, rows-1), Quaternion.identity);

		GenerateKeyItems ();

        int enemyCounter = (int)Math.Log(Player.floorLevel, 2);

        SpawnEnemies(0, enemyCounter);
        SpawnEnemies(1, enemyCounter);
        SpawnEnemies(2, enemyCounter);

		// May generate items up to the specified number and place them on the board.
		GenerateBasicItems ((rows+columns)/3);

		GenerateBlockingObjects (waterTile, 0.03F);
		for (int i = 0; i < rockTiles.Length; i++) {
			GenerateBlockingObjects (rockTiles [(int)(Random.value * rockTiles.Length)], 0.01F);
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
				GameObject newItem = Instantiate (keyItem, location, Quaternion.identity) as GameObject;
			}
		}
	}

	public void DrawPond(int rows, int columns, Vector3 position){
		GameObject newTile;
		Vector3 tilePosition;
		for (int i = 0; i < rows; i++) {
			tilePosition = new Vector3(position.x, position.y + i);
			if(!filledPositions.Contains(tilePosition)){
				newTile = Instantiate (waterTile, new Vector3(tilePosition.x, tilePosition.y), Quaternion.identity) as GameObject;
				filledPositions.Add(newTile.transform.position);
				newTile.transform.SetParent(boardTiles);
			}
			for(int j = 1; j < columns; j++){
				tilePosition = new Vector3(position.x + j, position.y + i);
				if(!filledPositions.Contains(tilePosition)){
					newTile = Instantiate (waterTile, new Vector3(tilePosition.x, tilePosition.y), Quaternion.identity) as GameObject;
					newTile.transform.SetParent(boardTiles);
					filledPositions.Add(newTile.transform.position);
				}
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


    void SpawnEnemies(int type, int numToSpawn)
    {
        for (int i = 0; i < numToSpawn; i++)
        {
            float x = (int)(Random.value * columns - 2) + 1;
            float y = (int)(Random.value * rows - 2) + 1;
            Vector3 position = new Vector3(x, y);
            if (!filledPositions.Contains(position))
            {
                Instantiate(enemies[type], position, Quaternion.identity);
                filledPositions.Add(position);
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
