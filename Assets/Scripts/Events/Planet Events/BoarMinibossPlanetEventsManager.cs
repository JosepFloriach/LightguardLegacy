using UnityEngine;
using System.Collections;

public class BoarMinibossPlanetEventsManager : PlanetEventsManager {

	private bool dragonSlideTutorialDone = false;

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
		GUIManager.setTutorialText("You can jump to the DragonHeads to travel to another galaxy!");
		GUIManager.activateTutorialText();
		yield return new WaitForSeconds (10f);
		GUIManager.deactivateTutorialText();
		dragonSlideTutorialDone = true;
	}
}
