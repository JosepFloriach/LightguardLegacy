using UnityEngine;
using System.Collections;

public enum TutorialSplashScreen{HowToMove,HowToAttack,HowToDash,HowToThrow,HowToKame,HowToSpaceJump,HowToSpaceJumpOrbit,HowToDragonslide}

public class TutorialManager : MonoBehaviour {

	public GameObject howToMove, howToAttack, howToDash, howToThrow, howToKame, howToSpaceJump, howToSpaceJumpOrbit, howToDragonslide;


	private bool isActive = false;
	private bool fadingIn = false;
	private bool fadingOut = false;
	private float timer = 0f;

	private GameObject currentTutorial;

	void Awake(){
		GameManager.registerTutorialManager (this);
	}

	public void activateTutorial(TutorialSplashScreen t){
		GameManager.inputController.disableInputController ();
		//Fade In

		currentTutorial = getTutorialSplashScreen (t);
		currentTutorial.SetActive (true);
		isActive = true;
		currentTutorial.GetComponent<CanvasGroup>().alpha = 0f;
		timer = 0f;
		fadingIn = true;
		//Fade Out
	}

	void Update(){
		if (isActive) {
			timer+=Time.deltaTime;
			if(fadingIn){
				float ratio = timer/0.5f;
				if(ratio>=1f){
					currentTutorial.GetComponent<CanvasGroup>().alpha = 1f;
					fadingIn = false;
				}else{
					currentTutorial.GetComponent<CanvasGroup>().alpha = ratio;
				}
			}

			if(fadingOut){
				float ratio = 1f-(timer/0.5f);
				if(ratio<=0f){
					currentTutorial.GetComponent<CanvasGroup>().alpha = 0f;
					fadingOut = false;
					deactivate();
				}else{
					currentTutorial.GetComponent<CanvasGroup>().alpha = ratio;
				}
			}
		}
	}

	public void deactivateActualTutorial(){
		if (!fadingIn) {
			fadingOut = true;
			timer = 0f;
		}
	}

	private void deactivate(){
		currentTutorial.SetActive (false);
		currentTutorial = null;
		isActive = false;
		GameManager.inputController.enableInputController ();
	}

	public bool getIsActive(){
		return isActive;
	}

	private GameObject getTutorialSplashScreen(TutorialSplashScreen t){
		if (t.Equals (TutorialSplashScreen.HowToMove)) {
			return howToMove;
		}else if (t.Equals (TutorialSplashScreen.HowToAttack)) {
			return howToAttack;
		}else if (t.Equals (TutorialSplashScreen.HowToDash)) {
			return howToDash;
		}else if (t.Equals (TutorialSplashScreen.HowToThrow)) {
			return howToThrow;
		}else if (t.Equals (TutorialSplashScreen.HowToKame)) {
			return howToKame;
		}else if (t.Equals (TutorialSplashScreen.HowToSpaceJump)) {
			return howToSpaceJump;
		}else if (t.Equals (TutorialSplashScreen.HowToSpaceJumpOrbit)) {
			return howToSpaceJumpOrbit;
		}else if (t.Equals (TutorialSplashScreen.HowToDragonslide)) {
			return howToDragonslide;
		}
		return null;
	}

}
