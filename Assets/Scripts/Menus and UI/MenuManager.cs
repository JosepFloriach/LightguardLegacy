using UnityEngine;
using System.Collections;

public class MenuManager : MonoBehaviour {

	public bool onAnyButtonChangeToMainMenu = false;

	public GameObject firstButtonFocus;

	public bool menuWithLeaves = false;

	public void ChangeMenu(Menu newMenu){
		GUIManager.activateMenu (newMenu);
		//GameManager.actualSceneManager.ChangeScene (newMenu);
	}

	public void GoToControlsMenu(){
		GameManager.audioManager.PlayMiscSound(SoundIDs.PRESS_BUTTON,AudioManager.STABLE);
		GUIManager.fadeOutChangeMenuFadeIn (Menu.ControlsMenu);
	}

	public void GoToOptionsMenu(){
		GameManager.audioManager.PlayMiscSound(SoundIDs.PRESS_BUTTON,AudioManager.STABLE);
//		PlayStableSound(SoundIDs.PRESS_BUTTON);
		GUIManager.fadeOutChangeMenuFadeIn (Menu.OptionsMenu);
	}
	
	public void GoToCreditsMenu(){
		GameManager.audioManager.PlayMiscSound(SoundIDs.PRESS_BUTTON,AudioManager.STABLE);
		GUIManager.fadeOutChangeMenuFadeIn (Menu.CreditsMenu);
	}

	public void GoToPauseMenu(){
		GameManager.audioManager.PlayMiscSound(SoundIDs.PRESS_BUTTON,AudioManager.STABLE);
		GUIManager.fadeOutChangeMenuFadeIn (Menu.OnPauseMenu);
	}

	public void BackOptionsMenu(){
		GameManager.audioManager.PlayMiscSound(SoundIDs.PRESS_BUTTON,AudioManager.STABLE);
		if(GameManager.isGamePaused){
			GUIManager.fadeOutChangeMenuFadeIn (Menu.OnPauseMenu);
		}else{
			GUIManager.fadeOutChangeMenuFadeIn (Menu.MainMenu);
		}
	}

	private void unPause(){
		GameManager.unPauseGame ();
	}

	public void ResumeGame(){
		GUIManager.fadeOut (unPause);
	}

	private void restartGameAndFadeInMainMenu(){
		GameManager.rebuildGameFromGameState ();
		GUIManager.fadeIn (Menu.MainMenu);
		GUIManager.fadeAllIn ();
	}

	public void SaveAndQuit(){
		GameManager.persistentData.save ();
		GUIManager.fadeAllOut (restartGameAndFadeInMainMenu);
		GameManager.playerController.setLookingToCameraInCranePosition ();
		GameManager.unPauseGame ();
		GameManager.inputController.disableInputController ();
		GameManager.isGameEnded = true;
		//Application.LoadLevel(Application.loadedLevel);
	}
	
	public void StopGame(){
		GameManager.audioManager.PlayMiscSound(SoundIDs.PRESS_BUTTON,AudioManager.STABLE);
		GameManager.actualSceneManager.CloseApplication ();
	}
	
	public void StartGame(){
		GameManager.startGame ();
		GUIManager.fadeOutChangeMenuFadeIn (Menu.None);
	}

	public void RestartGame(){
		GameManager.restartGame ();
		GUIManager.fadeOutChangeMenuFadeIn (Menu.None);
	}

	public void GoToMainMenu(){
		GameManager.audioManager.PlayMiscSound(SoundIDs.PRESS_BUTTON,AudioManager.STABLE);
		GUIManager.fadeOutChangeMenuFadeIn (Menu.MainMenu);
	}

	public void OnQualityChanged(string newQuality){
		Debug.Log ("newQuality: " + newQuality);
		for(int i = 0;i<QualitySettings.names.Length;i++){
			string name = QualitySettings.names[i];
			if(name.Equals(newQuality)){
				QualitySettings.SetQualityLevel(i,true);
			}
		}
	}

	public void OnMusicVolumeSliderChange(float newVolume){
		Debug.Log ("Music Volume Changed To: " + newVolume);
	}

	public void OnFXVolumeSliderChange(float newVolume){
		Debug.Log ("FX Volume Changed To: " + newVolume);
	}

	public void GoToScreen(string scene){
		GameManager.actualSceneManager.ChangeScene (scene);
	}

	void Update(){
		if(onAnyButtonChangeToMainMenu){
			if(Input.anyKey){

			}
		}
	}
}
