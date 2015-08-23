using UnityEngine;
using System.Collections;

public class OptionsManager : MonoBehaviour {


	public float aspectRatioWidth = 16f;
	public float aspectRatioHeight = 9f;
	void Awake(){
		GameManager.registerOptionsManager (this);
		initialize ();
	}

	void initialize(){
		bool fullScreen = Screen.fullScreen;
		if(fullScreen){
			StartCoroutine(delayedCorrectScreenSize());
		}else{
			Screen.SetResolution(Screen.currentResolution.width,Screen.currentResolution.height,fullScreen);
		}
	}
	//We set fixed resolutions to keep the aspect ratio when in fullscreen
	public void setFullScreen(bool fullScreen){
		Debug.Log ("Fullscreen set to: " + fullScreen);
		if(fullScreen){
			StartCoroutine(delayedCorrectScreenSize());
		}else{
			Screen.SetResolution(1280,720,fullScreen);
		}
	}

	IEnumerator delayedCorrectScreenSize(){
		Screen.fullScreen = true;
		int widthOriginal = Screen.width;
		int heightOriginal = Screen.height;
		float timer = 0f;
		while(widthOriginal == Screen.width && heightOriginal == Screen.height && timer<0.2f){
			timer+=Time.deltaTime;
			yield return null;
		}
		float width = Screen.currentResolution.width;
		float height = Screen.currentResolution.height;
		float objectivelAspectRatio = aspectRatioWidth /aspectRatioHeight;
		float actualRatio = width/height;
		
		int newWidth = (int) width;
		int newHeight = (int) ((height * actualRatio)/objectivelAspectRatio);
		
		Screen.SetResolution(newWidth,newHeight,true);
	}
	
	public void OnResolutionChanged(int width,int height){
		Debug.Log ("New resolution: " + width + " " + height);
		Screen.SetResolution(width,height,false);
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
}
