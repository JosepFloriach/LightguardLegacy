using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour {

	public AudioSource musicplayer;
	public AudioSource bigpplayer;
	public AudioSource variablebigpPlayer;
	public AudioSource miscPlayer;
	

//Music
	public AudioClip[] music;
	
//SFX Misc
	public AudioClip[] sfxMisc;
	
//SFX Enemies
	public AudioClip[] sfxEnemies;
	
//SFX Big P
	public AudioClip[] sfxBigp;
	
									
//SFX parameters

	private float lowPitchRange = .95F;
	private float highPitchRange = 1.2F;
	private float velToVol = .2F;
	private float velocityClipSplit = 10F;
	
	private float volLowRange = .5f;
	private float volHighRange = 1.0f;
	
	//Tipos de reproduccion
	public const int STABLE = 0;
	public const int VARIABLE = 1;
	public const int COOLDOWN = 2; 

	//Categoria de sonido
	public const  int MUSIC = 0;
	public const int MISC = 1;
	public const int BIGP = 2;
	public const int ENEMY = 3;
			
			
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
	
	private AudioSource getVariableSource(int type) {
		AudioSource selectedSource;
		switch (type) {
		case BIGP:
			selectedSource = variablebigpPlayer;
			break;
		case ENEMY: 
			selectedSource = miscPlayer;
			break;
		default:
			selectedSource = miscPlayer;
			break;			
		}
		return selectedSource;
	
	}
	
	private AudioSource getSource(int type) {
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
	
	
	public void PlaySound(int id, int mode, int type) {
		AudioClip mClip = getClip(id,type);
		AudioSource mSource = getSource(type);
	
		switch (mode) {
		case STABLE:
			JustPlay(mClip,mSource);
			break;
		case VARIABLE:
			mSource = getVariableSource(type);
			PlayVariableSound(mClip,mSource);
			break;
		case COOLDOWN:
			mSource = getVariableSource(type);
			PlayNoRepeat(mClip,mSource);
			break;
		default:
		break;
		}
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
	
	float cd = 1f;
	private void PlayNoRepeat(AudioClip sound, AudioSource source) {
		float now = Time.time;
		if (lastplay +delta < now) {
			lastplay = now;
			delta = cd;
			JustPlay(sound,source);
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
	
	float lastplay = 0.0f;
	float delta = 0.0f;
	

	
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
