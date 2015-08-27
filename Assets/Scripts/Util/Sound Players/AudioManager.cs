using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour {





	public AudioSource musicplayer;
	public AudioSource bigpplayer;

//Music
	public AudioClip[] music;
	// 0 is opening
	// 1 is main game peaceful
	
	
//SFX
	public AudioClip[] sfx;
	// 0 for click on buttons
	// 1 little g faling
	
//SFX parameters

	private float lowPitchRange = .75F;
	private float highPitchRange = 1.5F;
	private float velToVol = .2F;
	private float velocityClipSplit = 10F;
	
	private float volLowRange = .5f;
	private float volHighRange = 1.0f;
	

	// Use this for initialization
	void Awake () {
		GameManager.RegisterAudioManager(this);
		
	}
	
	public void PlaySong(AudioClip mySong) {
		if (musicplayer.isPlaying) {
			//please stop the music
			musicplayer.Stop();
		}
		musicplayer.clip = mySong;
		musicplayer.Play();
	
	}
	
	//Plays the song at position n
	public void playSong(int n) {
		if (0 <= n && n < music.Length) 
		{
			if (musicplayer.isPlaying) {
			//please stop the music
				musicplayer.Stop();
			}
			musicplayer.clip = music[n];
			musicplayer.Play();
		}
	}
	
	public void RestartSong() {
		musicplayer.Play();
	}
	
	public void StopSong() {
	
		musicplayer.Stop();
	}
	
	public void PlaySound(int i) {
		if (0 <= i && i < sfx.Length) 
		{
			float hitVol = volLowRange;
			if (!bigpplayer.isPlaying) {
				bigpplayer.pitch = Random.Range (lowPitchRange,highPitchRange);
				hitVol = Random.Range (volLowRange, volHighRange);
			}
			
			bigpplayer.PlayOneShot(sfx[i],hitVol);
		}
	
	}
	/** Plays the sound with a constant pitch and volume, used for things that won-t repeat 
	*/
	public void PlayStableSound(int i) {
		if (0 <= i && i < sfx.Length) 
		{
		
		bigpplayer.pitch = 1f;
		bigpplayer.PlayOneShot(sfx[i],volLowRange);
		}
	}
	
	float lastplay = 0.0f;
	float delta = 0.0f;
	
	public void PlaySoundDontRepeat(int sound, float time) {
		float now = Time.time;
		if (lastplay +delta < now) {
			lastplay = now;
			delta = time;
			PlaySound(sound);
		}
	}
	
}
