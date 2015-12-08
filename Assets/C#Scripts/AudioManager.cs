using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour {
	// The audioSource component of the game object.
	public AudioSource audioSource;
	// The list of audio clips for the audioSource.
	public AudioClip [] clips;
	// The index of the current song being played.
	int currentSong;

	// Use this for initialization
	void Start () {
		// Play a random song from the audio clip list.
		PlayRandomSong ();
	}

	public void PlayRandomSong(){
		// Gets the AudioSource component from the game object attached with this script.
		audioSource = this.GetComponent<AudioSource> ();
		// Checks to see if we even have any audio clips.
		if (clips.Length > 0) {
			// Generates a random index value.
			int index = (int)(Random.value * clips.Length);
			// Sets the AudioSource component default clip to play.
			audioSource.clip = clips[index];
			// Sets the generated index to currentSong so we know the last song that played.
			currentSong = index;
			// Play the audio clip.
			audioSource.Play ();
		}
	}

	public void PlayNextSong(){
		// Checks if there are any audio clips.
		if (clips.Length > 0) {
			// Assigned the next song index to currentSong.
			currentSong = (currentSong + 1) % clips.Length;
			// Sets the default audio clip for the AudioSource to the new clip.
			audioSource.clip = clips [currentSong];
			// Plays the new audio clip.
			audioSource.Play ();
		}
	}
}
