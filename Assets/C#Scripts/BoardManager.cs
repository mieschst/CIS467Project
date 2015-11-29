using UnityEngine;
using System;
using System.Collections;
using Random = UnityEngine.Random;
using System.Collections.Generic;

public class BoardManager : MonoBehaviour {
	
	public GameObject floorTile;
	public GameObject wallTile;
	public GameObject waterTile;

	public GameObject[] basicItems;
	public GameObject[] keyItems;
	public GameObject[] enemies;

    public GameObject pitTile;

    public GameObject[] items;

	public GameObject ladder;

    public GameObject player;

    public int rows;
	public int columns;

    //public List<Vector3> OpenList = new List<Vector3>();
    //public List<Vector3> ClosedList = new List<Vector3>();
    //public List<int> HValues = new List<int>();

	List<Vector3> filledPositions = new List<Vector3>();

	private Transform boardTiles;
	private Transform boardItems;

    //List of all possible board positions
    private List<Vector3> boardPositions = new List<Vector3>();
	
	public void SetupBoard(int rows = 1, int columns = 1){

		// Makes sure the rows value is greater than 0.
		if(rows > 0)
			this.rows = rows;
		else
			this.rows = 1;
		// Makes sure the columns value is greater than 0.
		if (columns > 0)
			this.columns = columns;
		else
			this.columns = 1;

        //This section includes the board fix for the ghost instance
        boardTiles = new GameObject("BoardTiles").transform;

        // Assigns values to the column and row variables.
        //SetupBoard(9, 9);

        // Adds the floor tiles to the game board.
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                // Creates a new tile game object at position (i,j).
                GameObject newTile = Instantiate(floorTile, new Vector2(i, j), Quaternion.identity) as GameObject;

                // Adds the new tile to GameObject called 'BoardTiles' to help reduce clutter in the
                // hierarchy.
                newTile.transform.SetParent(boardTiles);

                //Adds the board location to the list
                //boardPositions.Add(new Vector3(i, j, -1f));
            }
        }

        // Adds the wall tiles to the game board.
        for (int i = -1; i <= rows; i++)
        {
            for (int j = -1; j <= columns; j++)
            {
                if (i == -1 || i == rows || j == -1 || j == columns)
                {
                    GameObject newTile = Instantiate(wallTile, new Vector2(i, j), Quaternion.identity) as GameObject;
                    newTile.transform.SetParent(boardTiles);
                }
            }
        }

        // Adds a ladder right corner of the moveable section of the board.
        Instantiate(ladder, new Vector3(rows - 1, columns - 1, 0), Quaternion.identity);

        // May generate items up to the specified number and place them on the board.
        GenerateBasicItems(10);

        //SpawnEnemies(0, new Vector3[] { new Vector3(3, 6), new Vector3(8, 2), new Vector3(10, 7) });
        //SpawnEnemies(1, new Vector3[] { new Vector3(5, 5), new Vector3(7, 4), new Vector3(8, 8) });

    }

    void SetupList()
    {
        //Clear previous list
        boardPositions.Clear();

        // Adds tiles to the board positions list.
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {                          
                //Adds the board location to the list
                boardPositions.Add(new Vector3(i, j, -1f));
            }
        }
    }
	
    
	// Use this for initialization
	void Start () {
		
		boardTiles = new GameObject ("BoardTiles").transform;

		// Assigns values to the column and row variables.
		SetupBoard (15, 9);

		filledPositions = new List<Vector3> ();
		
		// Adds the floor tiles to the game board.
		for (int i = 0; i < rows; i++) {
			for(int j = 0; j < columns; j++){
				// Creates a new tile game object at position (i,j).
				GameObject newTile = Instantiate(floorTile, new Vector2(i, j), Quaternion.identity) as GameObject;

				// Adds the new tile to GameObject called 'BoardTiles' to help reduce clutter in the
				// hierarchy.
				newTile.transform.SetParent(boardTiles);

                //Adds the board location to the list
                //boardPositions.Add(new Vector3(i, j, -1f));
			}
		}

		// Adds the wall tiles to the game board.
		for (int i = -1; i <= rows; i++) {
			for (int j = -1; j <= columns; j++){
				if(i == -1 || i == rows || j == -1 || j == columns){
					GameObject newTile = Instantiate(wallTile, new Vector2(i,j), Quaternion.identity) as GameObject;
					newTile.transform.SetParent(boardTiles);
				}
			}
		}

		GenerateKeyItems ();

		DrawPond (2, 1, new Vector3 (1, 3));

		DrawPond (1, 2, new Vector3 (9, 2));

		// Adds a ladder right corner of the moveable section of the board.
		Instantiate (ladder, new Vector3 (rows-1, columns-1), Quaternion.identity);

		// May generate items up to the specified number and place them on the board.
		GenerateBasicItems (6);

	}

	public void DrawPond(int rows, int columns, Vector3 position){
		GameObject newTile;
		Vector3 tilePosition;
		for (int i = 0; i < rows; i++) {
			tilePosition = new Vector3(position.x, position.y + i);
			if(!filledPositions.Contains(tilePosition)){
				newTile = Instantiate (waterTile, new Vector3(position.x, position.y + i), Quaternion.identity) as GameObject;
				filledPositions.Add(newTile.transform.position);
				newTile.transform.SetParent(boardTiles);
			}
			for(int j = 1; j < columns; j++){
				tilePosition = new Vector3(position.x + (j % columns), position.y + (i % rows));
				if(!filledPositions.Contains(tilePosition)){
					newTile = Instantiate (waterTile, new Vector3(position.x + (j % columns), position.y + (i % rows)), Quaternion.identity) as GameObject;
					newTile.transform.SetParent(boardTiles);
					filledPositions.Add(newTile.transform.position);
				}
			}
		}
	}

	void GenerateKeyItems(){
		Vector3 position;
		foreach (GameObject keyItem in keyItems) {
			// Values between 1 and the number of rows-1.
			float x = (int)(Random.value * (rows-2)+1);
			// Values between 1 and the number of columns-1.
			float y = (int)(Random.value * (columns-2)+1);

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

	// Generates an item and places it at some random position on the board. Note: The floor lining the wall
	// will not have items in it. This is so that the player doesn't get blocked when we add obstacles.
	void GenerateBasicItems(int numberOfItems){

		boardItems = new GameObject ("BoardItems").transform;

		// Adds items at random positions on the board.
		for (int i = 0; i < numberOfItems; i++) {
			// Values between 1 and the number of rows-1.
			float x = (int)(Random.value * (rows-2)+1);
			// Values between 1 and the number of columns-1.
			float y = (int)(Random.value * (columns-2)+1);

			// The position on the board to place the item.
			Vector3 location = new Vector3(x,y);

			// Checks if the random position hasn't been added already.
			if(filledPositions.Contains(location) == false){
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

	void SpawnEnemies (int index, Vector3 position){			
		Instantiate(enemies[index], position, Quaternion.identity);
		filledPositions.Add (position);			
	}

    public void LevelSelector(int level)
    {
        //Un comment this when using the ghost instance board fix above
        SetupBoard(rows, columns);

        SetupList();

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

    }

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

    /* bool BestSearch()
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
}
