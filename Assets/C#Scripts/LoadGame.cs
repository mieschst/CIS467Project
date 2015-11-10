using UnityEngine;
using System.Collections;

public class LoadGame : MonoBehaviour {

	public GameObject gameManager;

	// Use this for initialization
	void Start () {
		Instantiate (gameManager);
	}
	
	// Update is called once per frame
	void Update () {

	}
}
