using UnityEngine;
using System.Collections;

public class FrostFirePlanetEventsManager : PlanetEventsManager {

	private GameObject bigPappadaDialogue;

	private DialogueController bigPappadaDialogueController;

	public Material materialCoreOnSolidify;

	public RunnerSegment[] runnerSegments;

	public Checkpoint planetCheckpoint;
	public GameObject corruptionBlockade;
	public GameObject penguinAttackEvent;
	public GameObject hydraAttackEvent;

	public GameObject burningCore;
	public GameObject[] platforms;
	private Vector3[] platformsOriginalPosition;
	public GameObject rotatingFire;

	public float objectiveGrowingScale = 1.75f;
	private Vector3 startingScale;
	private Vector3 objectiveScale;
	private Quaternion startingFireRotation;

	private int lastCompletedSegment = 0;
	private bool diedOnSegment = false;

	public GameObject hydraPrefab;


	private bool hydraEventCinematicFinished = false;
	private bool hydraEventCinematicOngoing = false;

	private bool hasPlayedOnLandCinematic = false;
	private bool hasBeenAttackedByPenguins = false;

	private bool onGoingRunnerCinematic;
	
	public override void informEventActivated (CutsceneIdentifyier identifyier){
		if(identifyier.Equals(CutsceneIdentifyier.FrostFirePlanetPenguinAttack)){
			if(!hasBeenAttackedByPenguins){
				penguinAttackEvent.SetActive(false);
				GetComponent<PlanetSpawnerManager> ().enabled = true;
				hasBeenAttackedByPenguins = true;
			}
		}else if(identifyier.Equals(CutsceneIdentifyier.FrostFirePlanetHydraAppearence)){
			if(!hydraEventCinematicFinished && !hydraEventCinematicOngoing){
				hydraEventCinematicOngoing = true;
				startHydraCombat();
			}
		}
	}

	private void movePlatforms(float speed){
		foreach(GameObject platform in platforms){
			Vector3 platformPosition = platform.GetComponent<Renderer>().bounds.center;
			Vector3 direction = platformPosition - transform.position;
			platform.transform.position += direction.normalized * speed * Time.deltaTime;
		}
	}

	private IEnumerator doRunningEvent(){
		GameManager.mainCamera.GetComponent<CameraFollowingPlayer> ().setCameraShaking ();
		diedOnSegment = false;
		rotatingFire.SetActive(true);
		while (lastCompletedSegment<runnerSegments.Length && !diedOnSegment) {
			yield return StartCoroutine(doSegment());
		}
		if (!diedOnSegment) {
			hydraEventCinematicOngoing = false;
			hydraEventCinematicFinished = true;
			burningCore.GetComponent<DieOnTouch>().enabled = false;
			burningCore.layer = LayerMask.NameToLayer("Planets");
			burningCore.tag = "Planet";
			GetComponent<MeteoriteSpawner>().enabled = false;
			burningCore.GetComponent<Renderer>().material = materialCoreOnSolidify;
			burningCore.GetComponent<ParticleSystem>().Stop();
			GameManager.mainCamera.GetComponent<CameraFollowingPlayer> ().stopCameraShaking ();
			rotatingFire.SetActive(false);
		}
		//rotatingFire.SetActive(false);
	}

	public void hydraDead(){
		runningEvent ();
	}

	private void runningEvent(){
		StartCoroutine (doRunningEvent ());
	}

	private IEnumerator doSegment(){

		if(runnerSegments[lastCompletedSegment].resetPlatforms){
			resetPlatformPositions();
		}
		GameManager.persistentData.playerLastCheckpoint = runnerSegments [lastCompletedSegment].segmentCheckpoint.checkPointIndex;
		Vector3 startingScale = Vector3.one * runnerSegments [lastCompletedSegment].startingScale;
		Vector3 endScale = Vector3.one * runnerSegments [lastCompletedSegment].endingScale;

		float startingRotation = runnerSegments [lastCompletedSegment].startingFireRotation;
		float endingRotation = runnerSegments [lastCompletedSegment].endingFireRotation;

		float timer = 0f;
		while (timer<runnerSegments[lastCompletedSegment].timeItLasts && !diedOnSegment) {
			timer+=Time.deltaTime;
			float ratio = timer/runnerSegments[lastCompletedSegment].timeItLasts;
			burningCore.transform.localScale = Vector3.Lerp(startingScale,endScale,ratio);

			float actualRotation = ((endingRotation - startingRotation) * ratio) + startingRotation;
			Quaternion rotation = Quaternion.Euler (new Vector3 (burningCore.transform.localRotation.eulerAngles.x,burningCore.transform.localRotation.eulerAngles.y,actualRotation));
			rotatingFire.transform.rotation = rotation;

			if(runnerSegments[lastCompletedSegment].movePlatforms){
				movePlatforms(runnerSegments[lastCompletedSegment].speedPlatforms);
			}
			yield return null;
		}
		if (!diedOnSegment) {
			lastCompletedSegment++;
		}
	}

	private void cleanHydraCombat(){

	}

	private void startHydraCombat(){
		//Temporarily there is no hydra
		StartCoroutine (hydraCombat ());
	}

	private IEnumerator hydraCombat(){
		GameObject hydra = GameObject.Instantiate (hydraPrefab) as GameObject;
		hydra.transform.position = GameManager.player.transform.position + (GameManager.player.transform.up * 3f);
		IAController controller = hydra.GetComponent<IAController> ();
		controller.isDead = false;
		while (!controller.isDead) {
			yield return null;
		}
		hydraDead ();
	}

	private void resetPlatformPositions(){
		for(int i = 0;i<platformsOriginalPosition.Length;i++){
			platforms[i].transform.position = platformsOriginalPosition[i];
		}
	}

	public override void initialize (){
		if(isEnabled){
			GetComponent<PlanetSpawnerManager> ().enabled = false;
			bigPappadaDialogueController = GameManager.player.GetComponent<DialogueController>();
			corruptionBlockade.SetActive(true);
			objectiveScale = new Vector3 (objectiveGrowingScale, objectiveGrowingScale, objectiveGrowingScale);
			startingScale = new Vector3 (1f, 1f, 1f);
			rotatingFire.SetActive(false);
			startingFireRotation = rotatingFire.transform.rotation;
			platformsOriginalPosition = new Vector3[platforms.Length];
			for(int i = 0;i<platforms.Length;i++){
				platformsOriginalPosition[i] = platforms[i].transform.position;
			}
		}else{
			corruptionBlockade.SetActive(false);
		}
	}

	private IEnumerator onLandCinematic(){
		if(!hasPlayedOnLandCinematic){
			while(GameManager.playerController.getIsSpaceJumping()){
				yield return null;
			}
			GameManager.persistentData.playerLastCheckpoint = planetCheckpoint.checkPointIndex;
			GameManager.inputController.disableInputController ();
			yield return new WaitForSeconds(1.5f);

			bigPappadaDialogue = bigPappadaDialogueController.createNewDialogue ("Tsk...", 1f, false, false);
			yield return StartCoroutine (WaitInterruptable (1f, bigPappadaDialogue));

			bigPappadaDialogue = bigPappadaDialogueController.createNewDialogue ("Another corrupted planet...", 2f, false, false);
			yield return StartCoroutine (WaitInterruptable (2f, bigPappadaDialogue));

			bigPappadaDialogue = bigPappadaDialogueController.createNewDialogue ("I must find a way to cleanse it!", 2f, false, false);
			yield return StartCoroutine (WaitInterruptable (2f, bigPappadaDialogue));
			
			GameManager.inputController.enableInputController ();
			hasPlayedOnLandCinematic = true;
		}
	}

	public override void planetCleansed(){
		if(isEnabled){
			corruptionBlockade.SetActive(false);
		}
	}
	
	public override void isActivated (){
		StartCoroutine (onLandCinematic ());
	}

	public override void isDeactivated (){

	}

	public override void playerDies (){
		if(hydraEventCinematicOngoing){
			diedOnSegment = true;
		}
	}

	public override void playerRespawned (){
		if (diedOnSegment) {
			runningEvent();
		}
	}

	public override void onFadeOutAfterDeath (){
		if(diedOnSegment){
			burningCore.transform.localScale = runnerSegments[lastCompletedSegment].startingScale * Vector3.one;
		}
	}

}
