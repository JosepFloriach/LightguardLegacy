using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Audio;

public class OptionsMenuManager : MenuManager {

	public GameObject resolutions;
	public Toggle defaultToggleResolutionWindowed;

	public AudioMixer mixer;
	private static string MASTER = "Master";
	private static string SFX = "Sfx";
	private static string MUSIC = "Music";

	void Awake(){

	}

	//We set fixed resolutions to keep the aspect ratio when in fullscreen
	public void setFullScreen(bool fullScreen){
		if(fullScreen){
			defaultToggleResolutionWindowed.isOn = true;
			resolutions.SetActive(false);
		}else{
			resolutions.SetActive(true);
		}
		GameManager.optionsManager.setFullScreen (fullScreen);
	}

	public void OnResolutionChanged(int width,int height){
		GameManager.optionsManager.OnResolutionChanged (width, height);
	}
	
	public void OnQualityChanged(string newQuality){
		GameManager.optionsManager.OnQualityChanged (newQuality);
	}
	
	public void OnMusicVolumeSliderChange(float newVolume){
		mixer.SetFloat(MUSIC,newVolume);
		
	}
	
	public void OnFXVolumeSliderChange(float newVolume){
		mixer.SetFloat(SFX,newVolume);
		
	}
	

	
	/*
	public void setMasterlvl(float lvl){
		mixer.SetFloat(MASTER,lvl);
	}
	*/

}
