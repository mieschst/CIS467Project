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

    //public List<Vector3> OpenList = new List<Vector3>();
    //public List<Vector3> ClosedList = new List<Vector3>();
    //public List<int> HValues = new List<int>();

	List<Vector3> filledPositions = new List<Vector3>();

	private Transform boardTiles;
	public static Transform boardItems;

    //List of all possible board positions
    private List<Vector3> boardPositions = new List<Vector3>();
	
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

   /* void SetupList()
    {
        //Clear previous list
        boardPositions.Clear();

        // Adds tiles to the board positions list.
        for (int i = 1; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {     
		if(j==8)
			break;
		                     
                //Adds the board location to the list
                boardPositions.Add(new Vector3(i, j, -1f));
            }
        }
    }	*/
    
	// Use this for initialization
	void Start () {

		// Assigns values to the column and row variables.
		SetupBoard ();

		filledPositions = new List<Vector3> ();

		// Adds a ladder right corner of the moveable section of the board.
		Instantiate (ladder, new Vector3 (columns-1, rows-1), Quaternion.identity);
		Instantiate (lockedDoor, new Vector3 (columns-1, rows-1), Quaternion.identity);

		GenerateKeyItems ();

		SpawnEnemies(0, 1);
		SpawnEnemies(1, 2);
		SpawnEnemies(2, 3);

		// May generate items up to the specified number and place them on the board.
		GenerateBasicItems ((rows+columns)/3);

		GenerateBlockingObjects (waterTile, 0.03F);
		for (int i = 0; i < rockTiles.Length; i++) {
			GenerateBlockingObjects (rockTiles [(int)(Random.value * rockTiles.Length)], 0.01F);
		}
		GenerateBlockingObjects (pitTile, 0.01F);
	}

	void GenerateKeyItems(){
		Vector3 position;
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
	
	/*void SpawnEnemies (int index, Vector3 position){			
		Instantiate(enemies[index], position, Quaternion.identity);
		filledPositions.Add (position);	
	}*/

    /*public void LevelSelector(int level)
    {

        //Un comment this when using the ghost instance board fix above
//        SetupBoard(rows, columns);

        //SetupList();

        Instantiate(player);        

        LayoutTilesAtRandom(wallTile, 5, 10);

        LayoutTilesAtRandom(pitTile, 1, 4);

        int enemyCounter = (int)Math.Log(level, 2);

        while (enemyCounter != 0)
        {
            SpawnEnemies(0, GetRandomPosition());
            SpawnEnemies(1, GetRandomPosition());

            enemyCounter--;
        }

    } */

    Vector3 GetRandomPosition()
    {
        int randomIndex = Random.Range(0, boardPositions.Count);

        Vector3 randomPosition = boardPositions[randomIndex];

        boardPositions.RemoveAt(randomIndex);

        return randomPosition;        
    }

    void LayoutTilesAtRandom(GameObject tile, int min, int max)
    {
        int objectCounter = Random.Range(min, max + 1);

        for (int i = 0; i < objectCounter; i++)
        {
            Vector3 randomPosition = GetRandomPosition();

            //if (BestSearch() == true)
                Instantiate(tile, randomPosition, Quaternion.identity);
            //else
            //    Instantiate(floorTile, randomPosition, Quaternion.identity);
        }
    }

    /* Work in Progress for Pathfinding
	bool BestSearch()
    {
        OpenList.Clear();
        ClosedList.Clear();
        OpenList.Add(new Vector3(0, 0, 0));
        ClosedList.Add(new Vector3(0, 0, 0));

        Vector3 goal = new Vector3(rows - 1, columns - 1);

        while(OpenList.Count != 0)
        {
            Vector3 current = OpenList[0];

            if (current == goal)
                break;

            foreach(Vector3 neighbor in GetNeighbors(current))
            {
                if(ClosedList.Contains(neighbor) == false)
                {
                    OpenList.Insert(0, neighbor);
                    ClosedList.Add(current);
                }
            }
        }

        return true;
    }

    int Heuristic(Vector3 a, Vector3 b)
    {
        return (int)Math.Abs(a.x - b.x) + (int)Math.Abs(a.y - b.y);
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

    */

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
			}
		}
	}
}
