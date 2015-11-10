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

	List<Vector3> filledPositions;

	private Transform boardTiles;
	private Transform boardItems;

    //List of all possible board positions
    //private List<Vector3> boardPositions = new List<Vector3>();
	
	public void SetupBoard(){

		int maxBoardHeight = 14;
		int maxBoardWidth = 14;
		int minDimension = 6;

		rows = (int)(Random.value * maxBoardHeight) + minDimension;
		columns = (int)(Random.value * maxBoardWidth) + minDimension;
	
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
    }

//    void SetupList()
//    {
//        //Clear previous list
//        //boardPositions.Clear();
//
//        // Adds tiles to the board positions list.
//        for (int i = 1; i < rows; i++)
//        {
//            for (int j = 0; j < columns; j++)
//            {
//                if (j == 8)
//                    break;
//                
//                //Adds the board location to the list
//                //boardPositions.Add(new Vector3(i, j, -1f));
//            }
//        }
//    }
	
    
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
		SpawnEnemies(1, 4);

		// May generate items up to the specified number and place them on the board.
		GenerateBasicItems ((rows+columns)/3);

		GenerateWater ();
		GenerateRocks ();
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

	void SpawnEnemies (int type, int numToSpawn){
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

    public void LevelSelector(int level)
    {
//        SetupList();

//        LayoutTilesAtRandom(wallTile, 5, 10);
//
//        LayoutTilesAtRandom(pitTile, 1, 4);
    }



//    Vector3 GetRandomPosition()
//    {
//        int randomIndex = Random.Range(0, boardPositions.Count);
//
//        Vector3 randomPosition = boardPositions[randomIndex];
//
//        boardPositions.RemoveAt(randomIndex);
//
//        return randomPosition;        
//    }

//    void LayoutTilesAtRandom(GameObject tile, int min, int max)
//    {
//        int objectCounter = Random.Range(min, max + 1);
//
//        for (int i = 0; i < objectCounter; i++)
//        {
//            Vector3 randomPosition = GetRandomPosition();
//
//            Instantiate(tile, randomPosition, Quaternion.identity);
//        }
//    }

    // Update is called once per frame
    void Update () {

	}

	void GenerateWater(){
		for (int i = 0; i < (rows+columns)/6; i++) {
			float x = (int)(Random.value * (columns-2) + 1);
			float y = (int)(Random.value * (rows-2) + 1);
			DrawPond (1, 1, new Vector3 (x, y));
		}
	}

	void GenerateRocks(){
		for (int i = 0; i < (rows+columns)/6; i++) {
			float x = (int)(Random.value * (columns-2) + 1);
			float y = (int)(Random.value * (rows-2) + 1);
			if(!filledPositions.Contains(new Vector3(x,y))) {
				Instantiate(rockTiles[(int)(Random.value * rockTiles.Length)], new Vector3(x,y),Quaternion.identity);
				filledPositions.Add (new Vector3(x,y));
			}
		}
	}
}
