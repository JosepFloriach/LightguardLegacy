using UnityEngine;
using System;
using System.Collections;

public class FadeManager : MonoBehaviour {

	public Texture2D blackTexture;
	public Texture2D getHurtTexture;
	
	public float getHurtStartAlpha = 0.8f;
	public int drawDepth = -1000;

	float getHurtAlpha = 0f;
	int fadeDirGetHurt = 1;
	bool isFinishedGetHurt = true;

	float allAlpha = 0f;
	int fadeDirAll = 1;
	bool isFinishedAll = true;
	float lastTime;
	float deltaTime = 0f;

	float fadeSpeed = Constants.FADE_SPEED;
	private Action fadeAllInAction,fadeAllOutAction,fadeInAction,fadeOutAction;

	void Awake () {
		GUIManager.registerFadeManager (gameObject);
		lastTime = Time.realtimeSinceStartup;
	}

	void Update(){
		deltaTime = Time.realtimeSinceStartup - lastTime;
		lastTime = Time.realtimeSinceStartup;
	}

	public void getHurtEffect(){
		getHurtAlpha = getHurtStartAlpha;
		isFinishedGetHurt = false;
		fadeDirGetHurt = -1;
	}



	public void fadeIn(GameObject menu){
		fadeInWithAction (null, menu);
	}

	public void fadeInWithAction(Action fadeAction,GameObject menu){
		if(menu!=null){
			menu.GetComponent<CanvasGroup> ().alpha = 0f;
			menu.GetComponent<CanvasGroup> ().interactable = false;
			fadeSpeed = Constants.FADE_SPEED;
			StartCoroutine(fadeTexture(menu,1f,0f,fadeAction,fadeSpeed));
		}
	}

	public void fadeOut(Action fadeAction,GameObject menu){
		if(menu!=null){
			menu.GetComponent<CanvasGroup> ().alpha = 1f;
			menu.GetComponent<CanvasGroup> ().interactable = false;
		}
		StartCoroutine(fadeTexture(menu,-1f,1f,fadeAction,fadeSpeed));
	}

	public void fadeOutCoroutine(GameObject menu,Action fadeAction = null,float fadeSpeed = 1f){
		StartCoroutine(fadeTexture(menu,-1f,1f,fadeAction,fadeSpeed));
	}

	public void fadeInCoroutine(GameObject menu,Action fadeAction = null,float fadeSpeed = 1f){
		menu.GetComponent<CanvasGroup> ().alpha = 0f;
		menu.GetComponent<CanvasGroup> ().interactable = false;
		StartCoroutine(fadeTexture(menu,1f,0f,fadeAction,fadeSpeed));
	}


	public void fadeOutWithSpeed(Action fadeAction,GameObject menu,float speed){
		fadeSpeed = speed;
		fadeOut (fadeAction, menu);
	}

	public void fadeAllOut(Action fadeAction){
		fadeDirAll = -1;
		allAlpha = 1f;
		fadeAllOutAction = fadeAction;
		isFinishedAll = false;
	}
	public void fadeAllIn(){
		fadeAllInWithAction (null);
	}
	
	public void fadeAllInWithAction(Action fadeAction){
		fadeDirAll = 1;
		allAlpha = 0f;
		isFinishedAll = false;
		fadeAllInAction = fadeAction;

	}
	
	void OnGUI(){
		drawGetHurtTexture ();
		drawFadeOutCompleteTexture ();
	}

	private IEnumerator fadeTexture(GameObject fadingMenu,float fadeDir,float alpha,Action fadeAction,float fadeSpeed){
		bool isFinishedF = false;
		while(!isFinishedF){
			alpha += fadeDir * fadeSpeed * deltaTime;
			alpha = Mathf.Clamp01 (alpha); 
			if(fadingMenu!=null){
				fadingMenu.GetComponent<CanvasGroup>().alpha = alpha;
			}
			
			if (fadeDir == -1 && alpha == 0f) {
				isFinishedF = true;
				if(fadeAction!=null){
					fadeAction ();
				}
			} else if (fadeDir == 1 && alpha == 1f) {
				isFinishedF = true;
				if(fadingMenu!=null){
					fadingMenu.GetComponent<CanvasGroup>().interactable = true;
				}
				if(fadeAction!=null){
					fadeAction();
				}
			}
			yield return null;
		}
	}

	void drawFadeOutCompleteTexture(){
		if (!isFinishedAll) {
			allAlpha += fadeDirAll * Constants.FADE_SPEED * deltaTime;
			allAlpha = Mathf.Clamp01 (allAlpha); 

			GUI.color = new Color (GUI.color.r, GUI.color.g, GUI.color.b, 1f-allAlpha);
			
			GUI.depth = drawDepth;
			GUI.DrawTexture (new Rect (0, 0, Screen.width, Screen.height), blackTexture);
			
			if (fadeDirAll == -1 && allAlpha == 0f) {
				isFinishedAll = true;
				if(fadeAllOutAction!=null){
					fadeAllOutAction();
				}
			} else if (fadeDirAll == 1 && allAlpha == 1f) {
				isFinishedAll = true;
				if(fadeAllInAction!=null){
					fadeAllInAction();
				}
			}
		}
	}

	void drawGetHurtTexture(){
		if (!isFinishedGetHurt) {
			getHurtAlpha += fadeDirGetHurt * Constants.FADE_SPEED * Time.fixedDeltaTime;
			getHurtAlpha = Mathf.Clamp01 (getHurtAlpha); 
		  
			GUI.color = new Color (GUI.color.r, GUI.color.g, GUI.color.b, getHurtAlpha);
		
			GUI.depth = drawDepth;
			GUI.DrawTexture (new Rect (0, 0, Screen.width, Screen.height), getHurtTexture);
		
			if (fadeDirGetHurt == -1 && getHurtAlpha == 0f) {
				isFinishedGetHurt = true;
			} else if (fadeDirGetHurt == 1 && getHurtAlpha == 1f) {
				isFinishedGetHurt = true;
			}
		}
	}
}
