using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MundusPlanetEventsManager : PlanetEventsManager {

	public GameObject mundusPrefab;
	public GameObject mundusSpawnPosition;
	public GameObject positionOnPlanetExplode;
	public GameObject lastPlanetPrefab;
	public GameObject platformPrefab;
	public GameObject positionEndingBigP;
	public GameObject littleGHopper;
	public LayerMask layersToCollideRaycastPlatforms;

	private DialogueController mundusDialogueController; 
	private DialogueController bigPappadaDialogueController;
	private DialogueController littleGDialogueController; 

	private GameObject mundusDialogue;
	private GameObject bigPappadaDialogue;
	private GameObject littleGDialogue; 

	bool firstCinematicPlayed = false;
	bool hasBeenActivated = false;

	private GameObject mundus;
	private List<GameObject> fisures;
	private GameObject athmosphere;
	private GameObject lastPlanet;
	private bool isInSecondPhase = false;
	private bool isFinishedTransition = false;

	
	//Is called when the class is activated by the GameTimelineManager
	public override void isActivated(){
		if(!hasBeenActivated){
			if(isEnabled){

				//setFase2();

				//We instantiate the inside of the planet 

				//We create the new mundus

				GameManager.audioManager.playSong(4);

				mundus = GameObject.Instantiate(mundusPrefab) as GameObject;
				mundus.GetComponent<IAControllerMundus>().informPlanetEventManager(this);
				mundus.transform.position = mundusSpawnPosition.transform.position;
				mundusDialogueController = mundus.GetComponent<DialogueController>();
				bigPappadaDialogueController = GameManager.player.GetComponent<DialogueController>();
				littleGDialogueController = littleGHopper.GetComponent<DialogueController>();
				GameManager.inputController.disableInputController();
				GUIManager.deactivateMinimapGUI();
			}
		}
	}

	private void setFase2(){
		//We instantiate the inside of the planet 
		
		mundus = GameObject.Instantiate(mundusPrefab) as GameObject;
		mundus.GetComponent<IAControllerMundus>().informPlanetEventManager(this);
		mundus.transform.position = mundusSpawnPosition.transform.position;

		mundus.GetComponent<IAControllerMundus> ().setPhase (2);
		StartCoroutine(CinematicChangeToPhase2());
	}

	private IEnumerator spawnPlatforms(){
		float timer = 0f;
		while(isInSecondPhase){
			timer+=Time.deltaTime;
			if(timer>=0.05f){
				timer = 0f;

				float distance = 45f;
				Vector3 position = Vector3.up * 35f;
				position = Quaternion.Euler(new Vector3(0f,0f,Random.value*360f))*position;
				position += transform.position;
				position.z = GameManager.player.transform.position.z;
				bool isGoodPosition = true;
				Vector3 direction = transform.position - position;
				RaycastHit hit;
				if(Physics.SphereCast(position,1f,direction,out hit,50f,layersToCollideRaycastPlatforms)){
					if(hit.collider.gameObject.tag.Equals("MundusPlanetFragment") || hit.distance<1f){
						isGoodPosition = false;
					}
				}
				Collider[] colliders = Physics.OverlapSphere(position,3f);
				if(colliders.Length>1){
					isGoodPosition = false;
				}
				
				if(isGoodPosition){
					GameObject platform = GameObject.Instantiate(platformPrefab) as GameObject;
					platform.transform.parent = lastPlanet.transform;
					platform.transform.position = position;
				}
			}
			yield return null;
		}
	}

	private IEnumerator CinematicEndGame(){

		GameManager.mainCamera.GetComponent<CameraFollowingPlayer> ().returnOriginalZ ();
		//GameObject closestPlatform = getClosestPlatformTop (GameManager.player.transform.position);
		GUIManager.fadeIn (Menu.BlackMenu);
		littleGHopper.GetComponent<CharacterController> ().LookLeftOrRight (1f);
		GameManager.playerController.isInvulnerable = true;
		yield return new WaitForSeconds (1f);
		GameManager.inputController.disableInputController ();
		GetComponent<PlanetCorruption> ().setCorruptionToClean ();
		GameManager.player.GetComponent<CharacterController> ().LookLeftOrRight (-1f);
		GameManager.playerController.StopMove ();
		GameManager.player.GetComponent<Rigidbody> ().velocity = Vector3.zero;
		GameManager.player.transform.position = positionEndingBigP.transform.position;
		GameManager.playerAnimator.SetBool ("isDerribado", true);

		yield return new WaitForSeconds (0.5f);

		GUIManager.fadeOut (null);
		littleGHopper.GetComponentInChildren<Animator> ().SetBool ("isFallingDown",false);
		yield return new WaitForSeconds (2f);

		littleGDialogue = littleGDialogueController.createNewDialogue ("Master?", 2f, false, false);
		yield return StartCoroutine (WaitInterruptable (2f, littleGDialogue));

		littleGHopper.GetComponent<CharacterController> ().Move (1f);
		littleGHopper.GetComponentInChildren<Animator> ().SetBool ("isWalking", true);
		yield return new WaitForSeconds (1f);
		littleGHopper.GetComponent<CharacterController> ().StopMoving ();
		littleGHopper.GetComponentInChildren<Animator> ().SetBool ("isWalking", false);

		littleGDialogue = littleGDialogueController.createNewDialogue ("Big P.?", 2f, false, false);
		yield return StartCoroutine (WaitInterruptable (2f, littleGDialogue));

		littleGDialogue = littleGDialogueController.createNewDialogue ("Nooooo!!", 2f, false, false);
		yield return StartCoroutine (WaitInterruptable (2f, littleGDialogue));

		GUIManager.fadeIn (Menu.BlackMenu);
		yield return new WaitForSeconds (1f);
		GameManager.persistentData.playerLastCheckpoint = 0;
		GameManager.restartGame ();
		GUIManager.fadeOut(null);
		yield return new WaitForSeconds (1f);
		GUIManager.fadeIn (Menu.MainMenu);
	}

	private IEnumerator CinematicChangeToPhase2(){
		isFinishedTransition = false;
		isInSecondPhase = true;
		GameObject staticPlatforms = lastPlanet.GetComponent<MundusFightPlanet>().staticPlatforms;
		mundus.GetComponent<IAControllerMundus> ().setPhase (2);
		mundus.GetComponent<GravityBody> ().setHasToApplyForce (false);
		yield return new WaitForSeconds (3f);
		//Deactivate player input and move camera
		GameManager.inputController.disableInputController ();
		GameManager.mainCamera.GetComponent<CameraFollowingPlayer> ().followObjective (positionOnPlanetExplode);
		GameManager.mainCamera.GetComponent<CameraFollowingPlayer> ().setObjectiveZ (50f);
		GameManager.mainCamera.GetComponent<CameraFollowingPlayer> ().setCameraShaking ();
		yield return new WaitForSeconds (3f);
		//Explosion of the planet
		foreach(GameObject fisure in fisures){
			foreach(ParticleSystem ps in fisure.GetComponentsInChildren<ParticleSystem>()){
				ps.Stop();
			}
			fisure.GetComponent<SphereCollider>().enabled = false;
		}

		GameObject closestPlayerSafePlace = getClosestPlatformTop (GameManager.player.transform.position);

		yield return new WaitForSeconds (1f);

		//Jump so the player won't stay in a strange state when the plane disappears
		GameManager.playerController.Jump ();
		if (Vector3.Distance (closestPlayerSafePlace.transform.position, GameManager.player.transform.position) > 1f) {
			GameManager.playerController.Move (Util.getPlanetaryDirectionFromAToB(GameManager.player,closestPlayerSafePlace));
		}
		yield return new WaitForSeconds (0.4f);
		GetComponent<Collider> ().enabled = false;
		athmosphere.GetComponent<Renderer> ().enabled = false;
		float timer = 0f;
		lastPlanet.GetComponent<MundusFightPlanet> ().coreParticlesExplosion.GetComponent<ParticleSystem> ().Play ();
		StartCoroutine (spawnPlatforms());
		while(timer<2f){
			timer+=Time.deltaTime;
			mundus.transform.position += mundus.transform.up * 8f * Time.deltaTime;
			foreach(Collider child in staticPlatforms.GetComponentsInChildren<Collider>()){
				Vector3 direccion = child.GetComponent<Rigidbody>().worldCenterOfMass -transform.position;
				direccion.z = 0f;
				child.transform.position +=direccion.normalized*8f * Time.deltaTime;
			}

			Vector3 position = closestPlayerSafePlace.transform.position;
			position.z = GameManager.player.transform.position.z;
			GameManager.player.transform.position  = position;
			yield return null;
		}
		lastPlanet.GetComponent<MundusFightPlanet> ().coreParticlesImplosion.GetComponent<ParticleSystem> ().Play ();



		mundus.GetComponent<IAControllerMundus> ().setPhase (2);
		GameManager.inputController.enableInputController ();
		GameManager.mainCamera.GetComponent<CameraFollowingPlayer> ().stopCameraShaking ();
		GameManager.mainCamera.GetComponent<CameraFollowingPlayer> ().setObjectiveZ (20f);
		GameManager.mainCamera.GetComponent<CameraFollowingPlayer> ().resetObjective();
		isFinishedTransition = true;
	}

	private IEnumerator startingCinematic(){
		GameObject middlePosition = new GameObject ();
		middlePosition.transform.position = (mundus.GetComponent<Rigidbody>().worldCenterOfMass + GameManager.player.GetComponent<Rigidbody>().worldCenterOfMass) / 2f;
		middlePosition.transform.up = middlePosition.transform.position - getInsidePlanetPosition ();
		GameManager.mainCamera.GetComponent<CameraFollowingPlayer> ().followObjective (middlePosition);

		mundusDialogue = mundusDialogueController.createNewDialogue ("You finally came!!", 2f, false, false);
		yield return StartCoroutine (WaitInterruptable (2f, mundusDialogue));

		bigPappadaDialogue = bigPappadaDialogueController.createNewDialogue ("I came here to destroy \n you mundus!!", 2f, false, false);
		yield return StartCoroutine (WaitInterruptable (2f, bigPappadaDialogue));

		mundusDialogue = mundusDialogueController.createNewDialogue ("HA HA HA!", 1f, false, false);
		yield return StartCoroutine (WaitInterruptable (1f, mundusDialogue));

		mundusDialogue = mundusDialogueController.createNewDialogue ("Just try!", 2f, false, false);
		yield return StartCoroutine (WaitInterruptable (2f, mundusDialogue));

		GameManager.inputController.enableInputController();
		mundus.GetComponent<IAControllerMundus> ().setPhase (1);

		GameManager.mainCamera.GetComponent<CameraFollowingPlayer> ().resetObjective ();
	}

	public bool getIsFinishedTransition(){
		return isFinishedTransition;
	}

	public Vector3 getInsidePlanetPosition(){
		return lastPlanet.transform.position;
	}
	public GameObject getClosestPlatformTop(Vector3 position){
		GameObject closestGO = null;
		float minimumDistance = float.MaxValue;
		foreach (MundusStaticPlatform pa in lastPlanet.GetComponentsInChildren<MundusStaticPlatform>()) {
			if(pa.positionToHoldOver!=null){
				if(Vector3.Distance(pa.positionToHoldOver.transform.position,position)<minimumDistance){
					minimumDistance = Vector3.Distance(pa.positionToHoldOver.transform.position,position);
					closestGO = pa.positionToHoldOver;
				}
			}
		}
		return closestGO;
	}

	public override void isDeactivated(){
		GUIManager.activateMinimapGUI();
		GameManager.audioManager.playSong(1);
		isInSecondPhase = false;
		Destroy(mundus);
		DestroyImmediate(lastPlanet);
		lastPlanet = GameObject.Instantiate(lastPlanetPrefab) as GameObject;
		lastPlanet.transform.position = transform.position;
		GetComponent<Collider> ().enabled = true;
		athmosphere.GetComponent<Renderer> ().enabled = true;
		foreach(GameObject fisure in fisures){
			Destroy(fisure);
		}
		fisures.Clear ();
	}

	public void informFisure(GameObject fisure){
		fisures.Add (fisure);
	}
	
	public override void informEventActivated (CutsceneIdentifyier identifyier){
		if(isEnabled){
			if(identifyier.Equals(CutsceneIdentifyier.LastPlanetMundusSecondPhase)){
				StartCoroutine(CinematicChangeToPhase2());
			}if(identifyier.Equals(CutsceneIdentifyier.MundusDies)){
				StartCoroutine(CinematicEndGame());
			}
		}
	}

	public void mundusInRangeOfCinematic(){
		StartCoroutine (startingCinematic ());
	}

	
	public override void initialize(){
		if(isEnabled){
			littleGHopper.GetComponentInChildren<Animator> ().SetBool ("isFallingDown",true);
			littleGHopper.GetComponent<CharacterController> ().setOriginalOrientation ();
			littleGHopper.GetComponent<CharacterController> ().LookLeftOrRight (1f);

			athmosphere = GetComponent<GravityAttractor>().getAthmosphere();
			fisures = new List<GameObject>(0);

			lastPlanet = GameObject.Instantiate(lastPlanetPrefab) as GameObject;
			lastPlanet.transform.position = transform.position;
			//GetComponent<PlanetCorruption> ().setCorruptionToClean ();
		}
	}
}
