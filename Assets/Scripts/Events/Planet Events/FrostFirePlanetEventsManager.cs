using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class FrostFirePlanetEventsManager : PlanetEventsManager {

	private GameObject bigPappadaDialogue;

	private DialogueController bigPappadaDialogueController;

	public Material materialCoreOnSolidify;

	public RunnerSegment[] runnerSegments;
	private List<Material> materialsPlanetUnfreeze;
	private float maxDistanceUnFreeze = 55f;

	public Checkpoint planetCheckpoint;
	public GameObject corruptionBlockade;
	public GameObject penguinAttackEvent;
	public GameObject hydraAttackEvent;
	public GameObject goToRunnerEvent;

	public bool runnerActivated = false; 

	public GameObject burningCore;
	public GameObject[] platforms;
	private Vector3[] platformsOriginalPosition;
	public GameObject rotatingFire;
	public GameObject lavaRunner;
	private Vector3 originalLavaRunnerPosition;
	private float originalRadiusCore;


	public float objectiveGrowingScale = 1.75f;
	private Vector3 startingScale;
	private Vector3 objectiveScale;
	private Quaternion startingFireRotation;

	private int lastCompletedSegment = 0;
	private bool diedOnSegment = false;

	public GameObject hydraPrefab;


	private bool hydraEventCinematicFinished = false;
	private bool hydraEventCinematicOngoing = false;
	private bool runnerStarted = false;

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
		}else if(identifyier.Equals(CutsceneIdentifyier.FrostFirePlanetGoToRunner)){
			if(!runnerStarted){
				StartCoroutine(startRunner());
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
		runnerActivated = true;
		diedOnSegment = false;
		rotatingFire.SetActive(true);
		while (lastCompletedSegment<runnerSegments.Length && !diedOnSegment) {
			yield return StartCoroutine(doSegment());
		}
		if (!diedOnSegment) {

			StartCoroutine(OnRunnerFinished());
		}
		//rotatingFire.SetActive(false);
	}

	private IEnumerator OnRunnerFinished(){
		float timer = 0f;
		float burningCoreRadius = burningCore.GetComponent<SphereCollider> ().radius * burningCore.transform.lossyScale.x;
		float missingDistance = (maxDistanceUnFreeze - burningCoreRadius);
		while (timer<3f) {
			timer+=Time.deltaTime;
			burningCoreRadius+= (Time.deltaTime * missingDistance)/3f;
			setMaterialsFrozenDistance(burningCoreRadius);
			yield return null;
		}

		hydraEventCinematicOngoing = false;
		hydraEventCinematicFinished = true;

		burningCore.GetComponent<DieOnTouch>().enabled = false;
		burningCore.layer = LayerMask.NameToLayer("Planets");
		burningCore.tag = "Planet";
		GetComponent<MeteoriteSpawner>().enabled = false;
		burningCore.GetComponent<Renderer>().material = materialCoreOnSolidify;
		burningCore.GetComponent<ScrollingUVs_Layers>().uvAnimationRate = Vector2.zero;
		burningCore.GetComponent<ParticleSystem>().Stop();
		GameManager.mainCamera.GetComponent<CameraFollowingPlayer> ().stopCameraShaking ();
		rotatingFire.SetActive(false);
	}

	private void resetPlatformsFromCombat(){
		foreach (GameObject platform in platforms) {
			if(!platform.GetComponent<HydraPlatform>().isActivated()){
				platform.GetComponent<HydraPlatform>().repositionPlatform();
			}
		}
	}

	public void hydraDead(){
		resetPlatformsFromCombat ();
		goToRunnerEvent.SetActive (true);
		StartCoroutine (goToRunner ());
	}

	private IEnumerator goToRunner(){
		GameManager.inputController.disableInputController ();
		bigPappadaDialogue = bigPappadaDialogueController.createNewDialogue ("That was a tough fight...", 2f, false,TextureDialogue.BigPappada,!GameManager.playerController.getIsLookingRight());
		yield return StartCoroutine (WaitInterruptable (2f, bigPappadaDialogue));

		GameManager.playerController.Move (-1f);

		bigPappadaDialogue = bigPappadaDialogueController.createNewDialogue ("I must find a way to get out of here!", 2f, false,TextureDialogue.BigPappada,!GameManager.playerController.getIsLookingRight());
		yield return StartCoroutine (WaitInterruptable (2f, bigPappadaDialogue));

	}

	private IEnumerator startRunner(){
		GameManager.playerController.StopMove ();
		GameManager.mainCamera.GetComponent<CameraFollowingPlayer> ().setCameraShaking ();
		bigPappadaDialogue = bigPappadaDialogueController.createNewDialogue ("Woooah!!", 1f, false,TextureDialogue.BigPappada,!GameManager.playerController.getIsLookingRight());
		yield return StartCoroutine (WaitInterruptable (1f, bigPappadaDialogue));

		GameManager.inputController.enableInputController ();
		runnerStarted = true;
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

			setMaterialsFrozenDistance ((burningCore.GetComponent<SphereCollider>().radius * burningCore.transform.lossyScale.x)-1f);
			setActualLavaRunnerPosition();
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
		StartCoroutine (hydraCombat ());
	}

	public GameObject getCloserHydraPlatformToPlayer(GameObject notCloseToObject = null){
		Vector3 playerPosition = GameManager.player.transform.position;
		GameObject platform = null;
		float minDistance = float.MaxValue;
		for (int i = 0; i<platforms.Length; ++i) {
			if(platforms[i].GetComponent<HydraPlatform>().isActivated()){
		
				Vector3 position = platforms[i].GetComponent<Renderer>().bounds.center;
				if(notCloseToObject == null || Vector3.Distance(notCloseToObject.transform.position,position)>5f){
					float distance = Vector3.Distance(position,playerPosition);
					if(distance<minDistance){
						minDistance = distance;
						platform = platforms[i];
					}
				}
			}
		}
		return platform;
	}


	private GameObject spawnHydra(GameObject other = null){
		GameObject hydra = GameObject.Instantiate (hydraPrefab) as GameObject;
		hydra.GetComponent<IAControllerHydra> ().eventManager = this;
		GameObject closestPlatform = getCloserHydraPlatformToPlayer (other);
		Vector3 platformPosition = closestPlatform.GetComponent<Renderer>().bounds.center;
		Vector3 direction = platformPosition - transform.position;
		hydra.transform.position = platformPosition+direction.normalized;
		return hydra;
	}

	private IEnumerator hydraCombat(){
		GameObject hydra = spawnHydra ();
		Killable killable = hydra.GetComponent<Killable> ();
		while (!killable.isDead()) {
			yield return null;
		}

		GameObject hydra2 = spawnHydra ();
		Killable killable2 = hydra2.GetComponent<Killable> ();
		yield return new WaitForSeconds (3f);

		GameObject hydra3 = spawnHydra (hydra2);
		Killable killable3 = hydra3.GetComponent<Killable> ();

		hydra3.GetComponent<IAControllerHydra> ().otherHydra = hydra2;
		hydra2.GetComponent<IAControllerHydra> ().otherHydra = hydra3;

		while (!killable2.isDead() || !killable3.isDead()) {
			yield return null;
		}

		hydraDead();
	}

	private void resetPlatformPositions(){
		for(int i = 0;i<platformsOriginalPosition.Length;i++){
			platforms[i].transform.position = platformsOriginalPosition[i];
		}
	}

	private void setActualLavaRunnerPosition(){
	/*	float actualCoreRadius = burningCore.GetComponent<SphereCollider> ().radius * burningCore.transform.lossyScale.z;
		float extraOffset = actualCoreRadius - originalRadiusCore ;
		Debug.Log (extraOffset);
		lavaRunner.transform.localPosition = originalLavaRunnerPosition + new Vector3 (0f, extraOffset, 0f);*/
	}

	public override void initialize (){
		if(isEnabled){
			MeshRenderer[] meshes = GetComponentsInChildren<MeshRenderer>();
			materialsPlanetUnfreeze = new List<Material>();
			foreach(MeshRenderer mesh in meshes){
				foreach(Material mat in mesh.materials){
					if(mat.HasProperty("_OriginFrozenCore")){
						materialsPlanetUnfreeze.Add(mat);
					}
				}
			}
			setMaterialsOriginPosition (transform.position);
			setMaterialsFrozenDistance(0f);

			GetComponent<PlanetSpawnerManager> ().enabled = false;
			bigPappadaDialogueController = GameManager.player.GetComponent<DialogueController>();
			corruptionBlockade.SetActive(true);
			objectiveScale = new Vector3 (objectiveGrowingScale, objectiveGrowingScale, objectiveGrowingScale);
			startingScale = new Vector3 (1f, 1f, 1f);
			rotatingFire.SetActive(false);
			originalLavaRunnerPosition = lavaRunner.transform.localPosition;
			originalRadiusCore = (burningCore.GetComponent<SphereCollider>().radius * burningCore.transform.lossyScale.z);
			startingFireRotation = rotatingFire.transform.rotation;
			platformsOriginalPosition = new Vector3[platforms.Length];
			for(int i = 0;i<platforms.Length;i++){
				platformsOriginalPosition[i] = platforms[i].transform.position;
			}
			goToRunnerEvent.SetActive(false);
		}else{
			goToRunnerEvent.SetActive(false);
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

			bigPappadaDialogue = bigPappadaDialogueController.createNewDialogue ("Tsk...", 1f, false,TextureDialogue.BigPappada,!GameManager.playerController.getIsLookingRight());
			yield return StartCoroutine (WaitInterruptable (1f, bigPappadaDialogue));

			bigPappadaDialogue = bigPappadaDialogueController.createNewDialogue ("Another corrupted planet...", 2f, false,TextureDialogue.BigPappada,!GameManager.playerController.getIsLookingRight());
			yield return StartCoroutine (WaitInterruptable (2f, bigPappadaDialogue));

			bigPappadaDialogue = bigPappadaDialogueController.createNewDialogue ("I must find a way to cleanse it!", 2f, false,TextureDialogue.BigPappada,!GameManager.playerController.getIsLookingRight());
			yield return StartCoroutine (WaitInterruptable (2f, bigPappadaDialogue));
			
			GameManager.inputController.enableInputController ();
			hasPlayedOnLandCinematic = true;
		}
	}

	private void setMaterialsOriginPosition(Vector3 position){
		foreach (Material material in materialsPlanetUnfreeze) {
			material.SetVector("_OriginFrozenCore",position);
		}
	}

	private void setMaterialsFrozenDistance(float distance){

		foreach (Material material in materialsPlanetUnfreeze) {
			material.SetFloat("_DistanceFreezeDisappear",distance);
			if((distance/maxDistanceUnFreeze)>0.2f){
				material.SetFloat("_Outline",0f);
			}
		}
	}
	
	IEnumerator planetCleansedCinematic() {
		GameManager.inputController.disableInputController ();
		yield return new WaitForSeconds(2.5f);
		
		bigPappadaDialogue = GameManager.player.GetComponent<DialogueController> ().createNewDialogue ("It's gonna be hard to leave this planet with the storm outside... ", 3f,false,TextureDialogue.BigPappada,!GameManager.playerController.getIsLookingRight());
		yield return StartCoroutine(WaitInterruptable (3f,bigPappadaDialogue));
		
		bigPappadaDialogue = GameManager.player.GetComponent<DialogueController> ().createNewDialogue ("But I can't give up now! Little G needs me!", 3f,false,TextureDialogue.BigPappada,!GameManager.playerController.getIsLookingRight());
		yield return StartCoroutine(WaitInterruptable (3f,bigPappadaDialogue));

		GameManager.inputController.enableInputController ();

	}

	public override void planetCleansed(){
		if(isEnabled){
			foreach(ParticleSystem c in corruptionBlockade.GetComponentsInChildren<ParticleSystem>()){
				c.Stop();
			}
			foreach(Collider c in corruptionBlockade.GetComponentsInChildren<Collider>()){
				c.enabled = false;
			}
			StartCoroutine("planetCleansedCinematic");
		}
	}
	
	public override void isActivated (){
		StartCoroutine (onLandCinematic ());
	}

	public override void isDeactivated (){

	}

	public override void playerDies (){
		if(runnerStarted){
			diedOnSegment = true;
		}
	}

	public override void playerRespawned (){
		if (diedOnSegment) {
			runningEvent ();
		}
	}

	public override void onFadeOutAfterDeath (){
		if(diedOnSegment){
			burningCore.transform.localScale = runnerSegments[lastCompletedSegment].startingScale * Vector3.one;
			float burningCoreRadius = burningCore.GetComponent<SphereCollider> ().radius * burningCore.transform.lossyScale.x;
			setActualLavaRunnerPosition();
			setMaterialsFrozenDistance(burningCoreRadius);

			float startingRotation = runnerSegments [lastCompletedSegment].startingFireRotation;
			Quaternion rotation = Quaternion.Euler (new Vector3 (burningCore.transform.localRotation.eulerAngles.x,burningCore.transform.localRotation.eulerAngles.y,startingRotation));
			rotatingFire.transform.rotation = rotation;
		}
	}

}
