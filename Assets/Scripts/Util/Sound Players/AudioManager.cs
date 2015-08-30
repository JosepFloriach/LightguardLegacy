using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour {

	public AudioSource musicplayer;
	public AudioSource bigpplayer;
	public AudioSource variablebigpPlayer;
	public AudioSource miscPlayer;
	

//Music
	public AudioClip[] music;
	// 0 is opening
	// 1 is main game peaceful
	
//SFX  
	public AudioClip[] sfx;
	// 0 for click on buttons
	// 1 little g faling
	
//SFX Misc
	public AudioClip[] sfxMisc;
	
//SFX Enemies
	public AudioClip[] sfxEnemies;
	
//SFX Big P
	public AudioClip[] sfxBigp;
	
									
//SFX parameters

	private float lowPitchRange = .75F;
	private float highPitchRange = 1.5F;
	private float velToVol = .2F;
	private float velocityClipSplit = 10F;
	
	private float volLowRange = .5f;
	private float volHighRange = 1.0f;
	
	//Tipos de reproduccion
	public static int STABLE = 0;
	public static int VARIABLE = 1;

	//Categoria de sonido
	private const  int MUSIC = 0;
	private const int MISC = 1;
	private const int BIGP = 2;
	private const int ENEMY = 3;
			
			
	void Awake () {
		GameManager.RegisterAudioManager(this);	
	}			
	
	private AudioClip getClip(int id, int type) {
		AudioClip[] selectedSoundType;
		switch (type) {
		case MUSIC: 
			selectedSoundType = music;
			break;
		case MISC:
			selectedSoundType = sfxMisc;
			break;
		case BIGP:
			selectedSoundType = sfxBigp;
			break;
		case ENEMY: 
			selectedSoundType = sfxEnemies;
			break;
		default:
			return null;
		}
		
		if (0 <= id && id < selectedSoundType.Length) {
		 	return selectedSoundType[id];
		} else {
			return null;
		}
	}
	
	public AudioSource getVariableSource(int type) {
		AudioSource selectedSource;
		switch (type) {
		case BIGP:
			selectedSource = variablebigpPlayer;
			break;
		case ENEMY: 
			selectedSource = miscPlayer;
			break;
		default:
			return null;
			
		}
		return selectedSource;
	
	}
	
	public AudioSource getSource(int type) {
		AudioSource selectedSource;
		switch (type) {
		case MUSIC: 
			selectedSource = musicplayer;
			break;
		case MISC:
			selectedSource = miscPlayer;
			break;
		case BIGP:
			selectedSource = bigpplayer;
			break;
		case ENEMY: 
			selectedSource = miscPlayer;
			break;
		default:
			return null;
			
		}
		return selectedSource;
	}
	
	public void PlayMiscSound(int id, int type) {
		AudioClip clipToPlay = getClip(id, MISC);
		AudioSource mSource = getSource(MISC);
		JustPlay(clipToPlay,mSource);
	}
	
	public void PlayBigpSound(int id, int type) {
		AudioClip clipToPlay = getClip(id, BIGP);
		AudioSource mSource;
		if (type == STABLE) {
			mSource = getSource(BIGP);
			JustPlay(clipToPlay,mSource);
		} else {
			mSource = getVariableSource(BIGP);
			PlayVariableSound(clipToPlay,mSource);
		}
		 
		
	}
	
	private void PlayVariableSound(AudioClip sound, AudioSource source) {
		float hitVol = volLowRange;
		source.pitch = Random.Range (lowPitchRange,highPitchRange);
		hitVol = Random.Range (volLowRange, volHighRange);
		source.PlayOneShot(sound,hitVol);
	}
	
	private void JustPlay(AudioClip sound, AudioSource audioSource) {
		if (sound == null) {return;} 
		if (audioSource == null) {return;}
		audioSource.pitch = 1f;
		audioSource.PlayOneShot(sound,volHighRange);
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
	
	float lastVol = 1f;
	float lastPitch = 1f;
	
	public void setPause(bool pause) {
		if (pause) {
			lastVol = musicplayer.volume;
			musicplayer.volume = 0.5f;
			lastPitch = musicplayer.pitch;
			musicplayer.pitch = 0.5f;
		} else {
			musicplayer.volume = lastVol;
			musicplayer.pitch = lastPitch;
		}
	
	}
	
}
