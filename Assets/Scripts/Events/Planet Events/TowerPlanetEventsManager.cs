using UnityEngine;
using System.Collections;

public class TowerPlanetEventsManager : PlanetEventsManager {

	public GameObject corruptionPlanetPrefab;
	public GameObject whiteHeartEncounter;
	public GameObject corruptionChasing;
	public GameObject jumpingToMundus;

	private DialogueController whiteHeartDialogueController; 
	private DialogueController bigPappadaDialogueController;
	private GameObject whiteHeartDialogue;
	private GameObject bigPappadaDialogue;

	private GameObject corruptionOngoing;

	private bool corruptionChased = false;
	private bool whiteHeartTalkedTo = false;

	public override void informEventActivated (CutsceneIdentifyier identifyier){
		if(identifyier.Equals(CutsceneIdentifyier.TowerPlanetCorruptionChasing)){
			StartCoroutine(towerPlanetCorruptionCutscene());
		}else if(identifyier.Equals(CutsceneIdentifyier.TowerPlanetWhiteHeartEncounter)){
			StartCoroutine(towerPlanetWhiteHeartEncounter());
		}else if(identifyier.Equals(CutsceneIdentifyier.TowerPlanetJumpingToMundus)){
			StartCoroutine(jumpingToMundusCutscene());
		}
	}

	private void stopCorruptionChasingEvent(){
		if(corruptionOngoing!=null){
			GameManager.mainCamera.GetComponent<CameraFollowingPlayer>().stopCameraShaking();
			Destroy(corruptionOngoing);
		}
	}

	private IEnumerator towerPlanetCorruptionCutscene(){
		if(!corruptionChased){
			corruptionChasing.GetComponent<TowerPlanetCorruptionChasing>().isActive = false;
			GameManager.inputController.disableInputController();
			GameManager.mainCamera.GetComponent<CameraFollowingPlayer>().setCameraShaking();
			yield return new WaitForSeconds (2f);
			bigPappadaDialogue = bigPappadaDialogueController.createNewDialogue ("What is this?!", 1f, true,TextureDialogue.BigPappada,!GameManager.playerController.getIsLookingRight());
			yield return StartCoroutine(WaitInterruptable (1f,bigPappadaDialogue));

			corruptionOngoing = GameObject.Instantiate (corruptionPlanetPrefab) as GameObject;
			GameObject planePosition = corruptionChasing.GetComponent<TowerPlanetCorruptionChasing> ().corruptionPoint;
			corruptionOngoing.transform.position = planePosition.transform.position;
			corruptionOngoing.transform.rotation = planePosition.transform.rotation;

			GameManager.inputController.enableInputController();
		}
	}

	private IEnumerator towerPlanetWhiteHeartEncounter(){
		if (!whiteHeartTalkedTo) {
			whiteHeartEncounter.GetComponent<TowerPlanetWhiteHeartEncounter>().isActive = false;
			GameManager.inputController.disableInputController ();
			whiteHeartDialogue = whiteHeartDialogueController.createNewDialogue ("Big P.?! ...", 1f, true,TextureDialogue.Whiteheart,true);
			yield return StartCoroutine (WaitInterruptable (1f, whiteHeartDialogue));
			Animator whiteHeartAnimator = whiteHeartEncounter.GetComponent<TowerPlanetWhiteHeartEncounter>().whiteHeartSensei.GetComponentInChildren<Animator>();
			whiteHeartAnimator.SetTrigger("standUp");
			yield return new WaitForSeconds(3f);
			bigPappadaDialogue = bigPappadaDialogueController.createNewDialogue ("...", 1f, true,TextureDialogue.BigPappada,!GameManager.playerController.getIsLookingRight());
			yield return StartCoroutine (WaitInterruptable (1f, bigPappadaDialogue));

			bigPappadaDialogue = bigPappadaDialogueController.createNewDialogue ("Master?!", 1f, true,TextureDialogue.BigPappada,!GameManager.playerController.getIsLookingRight());
			yield return StartCoroutine (WaitInterruptable (1f, bigPappadaDialogue));

			bigPappadaDialogue = bigPappadaDialogueController.createNewDialogue ("What are you doing here?!", 1f, true,TextureDialogue.BigPappada,!GameManager.playerController.getIsLookingRight());
			yield return StartCoroutine (WaitInterruptable (1f, bigPappadaDialogue));

			whiteHeartDialogue = whiteHeartDialogueController.createNewDialogue ("It's a long story... ", 2f, true,TextureDialogue.Whiteheart,true);
			yield return StartCoroutine (WaitInterruptable (2f, whiteHeartDialogue));
			bigPappadaDialogue = bigPappadaDialogueController.createNewDialogue ("Tell me everything! Master! ", 2f, true,TextureDialogue.BigPappada,!GameManager.playerController.getIsLookingRight());
			yield return StartCoroutine (WaitInterruptable (2f, bigPappadaDialogue));
			GUIManager.fadeIn (Menu.BlackMenu);
			yield return new WaitForSeconds (2f);
			GUIManager.fadeOut (null);
			bigPappadaDialogue = bigPappadaDialogueController.createNewDialogue ("I see... So that's why  they kidnapped Little G...! ", 2f, true,TextureDialogue.BigPappada,!GameManager.playerController.getIsLookingRight());
			yield return StartCoroutine (WaitInterruptable (2f, bigPappadaDialogue));
			whiteHeartDialogue = whiteHeartDialogueController.createNewDialogue ("Yes, you are our only option now! ", 2f, true,TextureDialogue.Whiteheart,true);
			yield return StartCoroutine (WaitInterruptable (2f, whiteHeartDialogue));
			bigPappadaDialogue = bigPappadaDialogueController.createNewDialogue ("I'll make them pay! ", 2f, true,TextureDialogue.BigPappada,!GameManager.playerController.getIsLookingRight());
			yield return StartCoroutine (WaitInterruptable (2f, bigPappadaDialogue));
			GameManager.inputController.enableInputController ();
			whiteHeartTalkedTo = true;
		}
	}

	private IEnumerator jumpingToMundusCutscene(){
		jumpingToMundus.GetComponent<TowerPlanetJumpingToMundus>().isActive = false;
		GameManager.inputController.disableInputController();

		stopCorruptionChasingEvent ();
		corruptionChased = true;
		bigPappadaDialogue = bigPappadaDialogueController.createNewDialogue ("Well, this is it!", 1f, true,TextureDialogue.BigPappada,!GameManager.playerController.getIsLookingRight());
		yield return StartCoroutine(WaitInterruptable (1f,bigPappadaDialogue));

		bigPappadaDialogue = bigPappadaDialogueController.createNewDialogue ("Here I come Little G.!!", 2f, true,TextureDialogue.BigPappada,!GameManager.playerController.getIsLookingRight());
		yield return StartCoroutine(WaitInterruptable (2f,bigPappadaDialogue));

		GameManager.inputController.enableInputController();

	}

	public override void onFadeOutAfterDeath (){
		stopCorruptionChasingEvent ();
	}

	public override void initialize (){
		whiteHeartDialogueController = whiteHeartEncounter.GetComponent<TowerPlanetWhiteHeartEncounter> ().whiteHeartSensei.GetComponent<DialogueController> ();
		bigPappadaDialogueController = GameManager.player.GetComponent<DialogueController> ();
	}

	public override void isActivated (){

	}

	public override void isDeactivated (){

	}

	public override void playerDies(){
		corruptionChasing.GetComponent<TowerPlanetCorruptionChasing>().isActive = true;
	}

	

}
