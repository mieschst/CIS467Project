using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour {
	public AudioSource audioSource;
	public AudioClip [] clips;

	// Use this for initialization
	void Start () {
		PlayRandomSong ();
	}

	public void PlayRandomSong(){
		audioSource = this.GetComponent<AudioSource> ();
		if (clips.Length > 0) {
			int index = (int)(Random.value * clips.Length);
			audioSource.clip = clips[index];
			audioSource.Play ();
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
