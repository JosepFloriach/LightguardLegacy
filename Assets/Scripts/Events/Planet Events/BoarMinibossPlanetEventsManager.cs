using UnityEngine;
using System.Collections;

public class BoarMinibossPlanetEventsManager : PlanetEventsManager {

	private bool dragonSlideTutorialDone = false;

	private GameObject bigPappadaDialogue;

	public override void informEventActivated (CutsceneIdentifyier identifyier){

	}

	public override void initialize (){

	}
	
	public override void isActivated (){
		
	}
	
	public override void isDeactivated (){
		
	}

	public override void chargeSpaceJumping (){
		if(!dragonSlideTutorialDone){
			StartCoroutine (dragonslidesTutorial ());
		}
	}
	

	private IEnumerator dragonslidesTutorial(){
		dragonSlideTutorialDone = true;
		GameManager.tutorialManager.activateTutorial(TutorialSplashScreen.HowToDragonslide);
		//GUIManager.setTutorialText("You can jump to the DragonHeads to travel to another galaxy!");
		//GUIManager.activateTutorialText();
		yield return new WaitForSeconds (10f);
		//GUIManager.deactivateTutorialText();
	}

	public override void planetCleansed(){
		if(isEnabled){
			StartCoroutine("planetCleansedCinematic");
		}
	}
	
	IEnumerator planetCleansedCinematic() {
		GameManager.inputController.disableInputController ();
		yield return new WaitForSeconds(1.5f);
		
		bigPappadaDialogue = GameManager.player.GetComponent<DialogueController> ().createNewDialogue ("Another planet cleansed... ", 3f,false,TextureDialogue.BigPappada,!GameManager.playerController.getIsLookingRight());
		yield return StartCoroutine(WaitInterruptable (3f,bigPappadaDialogue));
		
		bigPappadaDialogue = GameManager.player.GetComponent<DialogueController> ().createNewDialogue ("I must keep on looking for Little G.!", 3f,false,TextureDialogue.BigPappada,!GameManager.playerController.getIsLookingRight());
		yield return StartCoroutine(WaitInterruptable (3f,bigPappadaDialogue));

		GameManager.inputController.enableInputController ();

	}
}
