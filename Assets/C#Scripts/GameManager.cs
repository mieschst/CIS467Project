using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

    public static bool isHardMode { get; set; }

    public static GameManager instance = null;

    // Use this for initialization
    void Start () {

        //Check if instance already exists
        if (instance == null)

            //if not, set instance to this
            instance = this;

        //If instance already exists and it's not this:
        else if (instance != this)
            //Then destroy this
            Destroy(gameObject);
    }

    // Update is called once per frame
    void Update () {

    }
}
