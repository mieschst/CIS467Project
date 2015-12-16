using UnityEngine;
using System.Collections;

public class attackEffect : MonoBehaviour {

    public AudioClip startSound = new AudioClip();
    public AudioClip endSound = new AudioClip();
    private int counter = 1;
    public int animationLength = 1;
    private AudioSource speaker;

    // Use this for initialization
    void Start () {
        speaker = GetComponent<AudioSource>();
        if (!startSound.Equals(null)){
            speaker.clip = startSound;
            speaker.Play();
        }
    }
	
	// Update is called once per frame
	void Update () {
	    if (counter >= animationLength)
        {
            End();
        }
        counter++;
    }

    // Use this as the last thing before destruction
    void End ()
    {
        if (!endSound.Equals(null))
        {
            speaker.clip = endSound;
            speaker.Play();
        }
        Destroy(gameObject, 0.02f);
    }


}
