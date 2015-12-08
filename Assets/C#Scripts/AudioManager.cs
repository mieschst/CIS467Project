using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour {
	public AudioSource audioSource;
	public AudioClip [] clips;
	int currentSong;

	// Use this for initialization
	void Start () {
		PlayRandomSong ();
	}

	public void PlayRandomSong(){
		audioSource = this.GetComponent<AudioSource> ();
		if (clips.Length > 0) {
			int index = (int)(Random.value * clips.Length);
			audioSource.clip = clips[index];
			currentSong = index;
			audioSource.Play ();
		}
	}

	public void PlayNextSong(){
		currentSong = (currentSong + 1) % clips.Length;
		audioSource.clip = clips [currentSong];
		audioSource.Play ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
