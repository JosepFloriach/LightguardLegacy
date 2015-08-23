using UnityEngine;
using System.Collections;

public class InitialPlanetEventsManager : PlanetEventsManager {

	public GameObject bigPappadaInitialPosition;
	public GameObject littleGHopper;

	public GameObject boarHuntingGO;
	public GameObject shintoDoorGO;
	public GameObject toTheBridgeGO;
	public GameObject toTheBridge2GO;
	public GameObject bridgeFallGO;
	public GameObject lightGemGO;
	public GameObject rocksBlockingPathGO;
	public GameObject corruptionSeepingGO;
	public Vector3 weaponOnCranePosition;
	public Vector3 weaponOnCraneRotation;

	private Vector3 originalWeaponOnCranePosition;
	private Vector3 originalWeaponOnCraneRotation;

	bool firstCinematicPlayed = false;
	bool hasBeenActivated = false;
	bool hasDoneTutorialSpaceJumpDirections = false;

	private GameObject bigPappadaDialogue;
	private GameObject littleGDialogue;

	//Is called when the class is activated by the GameTimelineManager
	public override void isActivated(){
		if(!hasBeenActivated){
			if(isEnabled){
				GameObject middlePointBigPLittleG = new GameObject();
				GameManager.mainCamera.GetComponent<CameraFollowingPlayer>().setObjectiveZ(2.5f);
				GameManager.mainCamera.GetComponent<CameraFollowingPlayer>().setNewXAngle(0f);
				GameManager.mainCamera.GetComponent<CameraFollowingPlayer>().setUpMultiplyierWithAngle(0.7f);

				littleGHopper.SetActive(true);
				GameManager.player.transform.position = new Vector3(bigPappadaInitialPosition.transform.position.x,bigPappadaInitialPosition.transform.position.y,0f);
				middlePointBigPLittleG.transform.position = (littleGHopper.transform.position+GameManager.player.transform.position)/2f;
				middlePointBigPLittleG.transform.up = middlePointBigPLittleG.transform.position - transform.position;
				GameManager.mainCamera.GetComponent<CameraFollowingPlayer>().followObjective(middlePointBigPLittleG);
				hasBeenActivated = true;
				//First event is putting Big P in the initial position
				GameManager.player.transform.position = bigPappadaInitialPosition.transform.position;
				GameManager.player.transform.rotation = Quaternion.LookRotation (Vector3.forward*-1f, bigPappadaInitialPosition.transform.up*-1f);
				GameManager.mainCamera.GetComponent<CameraFollowingPlayer>().resetPosition();
				GameManager.playerSpaceBody.bindToClosestPlanet ();
				GameManager.playerSpaceBody.setStatic (true);
				GameManager.player.GetComponent<CharacterController> ().StopMoving ();
				GameManager.player.GetComponent<Rigidbody>().velocity = new Vector3(0f,0f,0f);
				GameManager.inputController.disableInputController ();
				GameManager.playerAnimator.SetBool("isDoingCranePosition",true);
				originalWeaponOnCranePosition = GameManager.playerController.weapon.transform.localPosition;
				originalWeaponOnCraneRotation = GameManager.playerController.weapon.transform.localEulerAngles;
				GameManager.playerController.weapon.transform.localPosition = weaponOnCranePosition;
				GameManager.playerController.weapon.transform.localEulerAngles = weaponOnCraneRotation;
				littleGHopper.GetComponent<SpaceGravityBody> ().bindToClosestPlanet ();
				littleGHopper.GetComponent<SpaceGravityBody> ().setStatic (true);
				littleGHopper.GetComponent<CharacterController> ().StopMoving ();
				littleGHopper.GetComponent<Rigidbody> ().velocity = new Vector3 (0f, 0f, 0f);

				toTheBridgeGO.GetComponent<Cutscene>().isActive = false;
				toTheBridge2GO.GetComponent<Cutscene>().isActive = false;
				bridgeFallGO.GetComponent<Cutscene>().isActive = false;
				GUIManager.deactivateMinimapGUI();
				boarHuntingGO.GetComponent<Cutscene>().isActive = false;
				boarHuntingGO.GetComponent<Cutscene> ().Initialize ();
				rocksBlockingPathGO.GetComponent<FirstPlanetBlockPathRocks>().rocks.SetActive(false);
				GameManager.persistentData.spaceJumpUnlocked = false;
			}else{
				GameManager.persistentData.spaceJumpUnlocked = true;
				boarHuntingGO.GetComponent<FirstPlanetBoarHunting>().boar.SetActive(false);
				boarHuntingGO.SetActive(false);
				littleGHopper.SetActive(false);
			}
		}
	}

	public override void isDeactivated(){

	}

	//This cinematic corresponds to the first cinematic that will play when the play button is pressed
	IEnumerator initialCinematic(){
		if(!firstCinematicPlayed){
			if(isEnabled){
				GameManager.inputController.disableInputController();
				firstCinematicPlayed = true;
				float timer = 0f;
				float time = 0.2f;
				littleGHopper.GetComponentInChildren<Animator>().SetBool("isFallingDown",true);
				yield return new WaitForSeconds (1f);
				GameManager.audioManager.PlayStableSound(1);
				littleGHopper.GetComponent<SpaceGravityBody> ().setStatic (false);
				float originalZ = littleGHopper.transform.position.z;

				littleGDialogue = littleGHopper.GetComponent<DialogueController> ().createNewDialogue ("AAaaah..", 2.5f, true, false);
				while(timer<time){
					timer+=Time.deltaTime;
					float ratio = timer/time;
					littleGHopper.transform.position = new Vector3(littleGHopper.transform.position.x,littleGHopper.transform.position.y,originalZ*(1f-ratio) -0.15f);
					yield return null;
				}

				yield return new WaitForSeconds (0.7f);
				littleGHopper.GetComponent<CharacterController> ().setOriginalOrientation ();
				littleGHopper.GetComponent<CharacterController> ().LookLeftOrRight (1f);
				yield return new WaitForSeconds (0.7f);
				GameManager.mainCamera.GetComponent<CameraFollowingPlayer> ().resetXAngle();
				GameManager.playerAnimator.SetBool("isDoingCranePosition",false);
				GameManager.playerAnimator.SetBool ("isJumping", true);
				GameManager.mainCamera.GetComponent<CameraFollowingPlayer>().returnOriginalZ();
				GameManager.mainCamera.GetComponent<CameraFollowingPlayer>().resetXAngle();
				GameManager.mainCamera.GetComponent<CameraFollowingPlayer>().resetObjective();
				GameManager.mainCamera.GetComponent<CameraFollowingPlayer>().resetUpMultiplyierWithAngle();
				GameManager.player.GetComponent<CharacterController>().StopMoving();
				GameManager.playerSpaceBody.setStatic (false);
				GameManager.player.GetComponent<CharacterController>().Jump(12f);
				GameManager.playerController.initializePlayerRotation();
				GameManager.player.GetComponent<CharacterController>().LookLeftOrRight(-1f);
				GameManager.playerController.weapon.transform.localPosition = originalWeaponOnCranePosition;
				GameManager.playerController.weapon.transform.localEulerAngles = originalWeaponOnCraneRotation;
				
				timer = 0f;
				originalZ = GameManager.player.transform.position.z;
				while(timer<time){
					timer+=Time.deltaTime;
					float ratio = timer/time;
					GameManager.player.transform.position = new Vector3(GameManager.player.transform.position.x,GameManager.player.transform.position.y,(originalZ *(1f-ratio)) -0.1f);
					yield return null;
				}
				yield return new WaitForSeconds (1f);
				littleGHopper.GetComponentInChildren<Animator>().SetBool("isFallingDown",false);
				yield return new WaitForSeconds (1f);
				bigPappadaDialogue = GameManager.player.GetComponent<DialogueController> ().createNewDialogue ("You must focus \n Little G.", 4f, false, false);
				yield return StartCoroutine(WaitInterruptable (4f,bigPappadaDialogue));
				bigPappadaDialogue = GameManager.player.GetComponent<DialogueController> ().createNewDialogue ("The balance is not in your body \n It's in your mind ", 4f, false, false);
				yield return StartCoroutine(WaitInterruptable (4f,bigPappadaDialogue));
				littleGDialogue = littleGHopper.GetComponent<DialogueController> ().createNewDialogue ("Yes master!", 3f, false, false);
				yield return StartCoroutine(WaitInterruptable (3f,littleGDialogue));
				bigPappadaDialogue = GameManager.player.GetComponent<DialogueController> ().createNewDialogue ("It's enough training \n for today ", 4f, false, false);
				yield return StartCoroutine(WaitInterruptable (4f,bigPappadaDialogue));
				bigPappadaDialogue = GameManager.player.GetComponent<DialogueController> ().createNewDialogue ("Go to the temple \n I'll go to hunt something to eat ", 4f, false, false);
				yield return StartCoroutine(WaitInterruptable (4f,bigPappadaDialogue));
				littleGHopper.GetComponent<CharacterController> ().Move (-1f);
				littleGHopper.GetComponentInChildren<Animator> ().SetBool ("isWalking", true);
				GameManager.inputController.enableInputController ();
				
				boarHuntingGO.SetActive (true);
				boarHuntingGO.GetComponent<Cutscene>().isActive = true;
				shintoDoorGO.GetComponent<FirstPlanetShintoDoor>().isActive = true;

				GUIManager.setTutorialText("Move with left joystick, jump with 'A' ");
				GUIManager.activateTutorialText();
				yield return new WaitForSeconds (5f);
				GUIManager.deactivateTutorialText();
				yield return new WaitForSeconds (12f);
				littleGHopper.GetComponent<CharacterController> ().StopMoving ();
				littleGHopper.GetComponentInChildren<Animator> ().SetBool ("isWalking", false);
			}
		}
	}

	//Cinematic that corresponds to the boar hunting event
	IEnumerator boarHuntingCinematic(){
		if(isEnabled){
			boarHuntingGO.GetComponent<FirstPlanetBoarHunting>().makeBoarGoAway();
			bigPappadaDialogue = GameManager.player.GetComponent<DialogueController> ().createNewDialogue ("A boar!", 2f, false, false);
			yield return null;
		}
	}

	//Cinematic that corresponds to the boar hunting event
	IEnumerator rocksBlockingPathCinematic(){
		if(isEnabled){
			rocksBlockingPathGO.GetComponent<FirstPlanetBlockPathRocks>().isActive = false;
			GameManager.inputController.disableInputController ();
			bigPappadaDialogue = GameManager.player.GetComponent<DialogueController> ().createNewDialogue ("Mh...", 1f, false, false);
			yield return StartCoroutine(WaitInterruptable (1f,bigPappadaDialogue));
			bigPappadaDialogue = GameManager.player.GetComponent<DialogueController> ().createNewDialogue ("This rocks \n are in the way", 3f, false, false);
			yield return StartCoroutine(WaitInterruptable (3f,bigPappadaDialogue));
			bigPappadaDialogue = GameManager.player.GetComponent<DialogueController> ().createNewDialogue ("I must find a \n way to break them! ", 3f, false, false);
			yield return StartCoroutine(WaitInterruptable (3f,bigPappadaDialogue));
			GameManager.inputController.enableInputController ();
			GUIManager.setTutorialText("Press 'X' to attack \n and clear the path!");
			GUIManager.activateTutorialText();
		}
	}

	IEnumerator corruptionSeepingCinematic(){
		if(isEnabled){
			corruptionSeepingGO.GetComponent<FirstPlanetCorruptionSeeping>().isActive = false;
			GameManager.inputController.disableInputController ();
			bigPappadaDialogue = GameManager.player.GetComponent<DialogueController> ().createNewDialogue ("Hmph...", 1f, true, false);
			yield return StartCoroutine(WaitInterruptable (1f,bigPappadaDialogue));
			bigPappadaDialogue = GameManager.player.GetComponent<DialogueController> ().createNewDialogue ("The corruption has \n gotten this far!", 3f, true, false);
			yield return StartCoroutine(WaitInterruptable (3f,bigPappadaDialogue));
			bigPappadaDialogue = GameManager.player.GetComponent<DialogueController> ().createNewDialogue ("I'll dash through it! ", 3f, false, false);
			yield return StartCoroutine(WaitInterruptable (3f,bigPappadaDialogue));
			GameManager.inputController.enableInputController ();
			GUIManager.setTutorialText("Press 'B' to dash \n and pass through the corruption!");
			GUIManager.activateTutorialText();
			yield return new WaitForSeconds (4f);
			GUIManager.deactivateTutorialText();
			yield return null;
		}
	}

	//Cinematic that corresponds to the third cinematic
	IEnumerator shintoDoorCinematic(){
		if(isEnabled){
			shintoDoorGO.GetComponent<Cutscene>().isActive = false;
			boarHuntingGO.GetComponent<Cutscene>().isActive = false;
			boarHuntingGO.GetComponent<FirstPlanetBoarHunting>().boar.SetActive(false);
			GameManager.inputController.disableInputController ();
			bigPappadaDialogue = GameManager.player.GetComponent<DialogueController> ().createNewDialogue ("That's weird... ", 1.5f, false, false);
			yield return StartCoroutine(WaitInterruptable (1.5f,bigPappadaDialogue));
			bigPappadaDialogue = GameManager.player.GetComponent<DialogueController> ().createNewDialogue ("I had never seen \n the seals shine this way...", 4f, false, false);
			yield return StartCoroutine(WaitInterruptable (4f,bigPappadaDialogue));
			GetComponent<PlanetCorruption>().corrupt();
			GameManager.mainCamera.GetComponent<CameraFollowingPlayer>().setCameraShaking();
			GameManager.audioManager.playSong(2);
			
			shintoDoorGO.GetComponent<FirstPlanetShintoDoor>().shintoDoor.GetComponent<ShintoDoor>().disableKanjis();
			yield return new WaitForSeconds(2f);
			bigPappadaDialogue = GameManager.player.GetComponent<DialogueController> ().createNewDialogue ("Woah!!", 1f, true, false);
			yield return StartCoroutine(WaitInterruptable (1f,bigPappadaDialogue));
			bigPappadaDialogue = GameManager.player.GetComponent<DialogueController> ().createNewDialogue ("¡¿What's this?!", 1f, true, false);
			yield return StartCoroutine(WaitInterruptable (5f,bigPappadaDialogue));
			bigPappadaDialogue = GameManager.player.GetComponent<DialogueController> ().createNewDialogue ("¡Little G.!", 2f, true, false);
			yield return StartCoroutine(WaitInterruptable (2f,bigPappadaDialogue));
			bridgeFallGO.GetComponent<FirstPlanetFallingFromTheBridge>().planetGettingCorrupted.SetActive(true);
			GameManager.player.GetComponent<PlayerController>().Move(1f);
			toTheBridgeGO.GetComponent<Cutscene>().isActive = true;



			//GameManager.inputController.enableInputController ();

		}
	}
	IEnumerator toTheBridgeCinematic(){
		if (isEnabled) {
			littleGHopper.GetComponentInChildren<ParticleSystem>().Play();
			GameManager.player.GetComponent<PlayerController>().Jump();
			GameManager.player.GetComponent<PlayerController>().Move(1f);
			toTheBridgeGO.GetComponent<Cutscene>().isActive = false;
			toTheBridge2GO.GetComponent<Cutscene>().isActive = true;
			yield return null;
		}
	}

	IEnumerator toTheBridge2Cinematic(){
		if (isEnabled) {
			littleGHopper.GetComponentInChildren<ParticleSystem>().Play();
			GameManager.player.GetComponent<PlayerController>().Jump();
			GameManager.player.GetComponent<PlayerController>().Move(1f);
			toTheBridge2GO.GetComponent<Cutscene>().isActive = false;
			bridgeFallGO.GetComponent<Cutscene>().isActive = true;
			yield return null;
		}
	}
	IEnumerator fallFromBridgeCinematic(){
		if(isEnabled){
			bridgeFallGO.GetComponent<FirstPlanetFallingFromTheBridge>().isActive = false;
			GameManager.player.GetComponent<PlayerController>().StopMove();
			littleGDialogue = littleGHopper.GetComponent<DialogueController> ().createNewDialogue ("Aaahhh!!\n MASTEEER!!", 1.5f, true, false);
			yield return StartCoroutine(WaitInterruptable (1f,littleGDialogue));
			littleGHopper.GetComponent<CharacterController>().Jump(25f);
			bigPappadaDialogue = GameManager.player.GetComponent<DialogueController> ().createNewDialogue ("¡Hold on!!", 1f, false, false);
			yield return StartCoroutine(WaitInterruptable (1f,bigPappadaDialogue));
			GameManager.inputController.disableInputController ();
			//bridgeFallGO.GetComponent<FirstPlanetFallingFromTheBridge>().bridge.GetComponent<Collider>().enabled = false;
			bridgeFallGO.GetComponent<FirstPlanetFallingFromTheBridge>().bridge.GetComponent<RotateAndMoveOverTime>().changeOverTime(2f);
			bigPappadaDialogue = GameManager.player.GetComponent<DialogueController> ().createNewDialogue ("¡Aaaaah!!", 1.5f, false, false);
			GameManager.playerAnimator.SetTrigger("isHurt");
			GUIManager.fadeIn(Menu.BlackMenu);
			yield return new WaitForSeconds(2f);
			bridgeFallGO.GetComponent<FirstPlanetFallingFromTheBridge>().bridge.SetActive(false);
			yield return new WaitForSeconds(3f);
			bridgeFallGO.GetComponent<FirstPlanetFallingFromTheBridge>().planetGettingCorrupted.SetActive(false);
			bridgeFallGO.GetComponent<FirstPlanetFallingFromTheBridge>().hideOutsidePlane.SetActive(true);
			GameManager.mainCamera.GetComponent<CameraFollowingPlayer>().stopCameraShaking();
			littleGHopper.SetActive(false);
			bridgeFallGO.GetComponent<FirstPlanetFallingFromTheBridge>().fallenRocks.SetActive(true);
			GUIManager.fadeOut(null);
			rocksBlockingPathGO.GetComponent<FirstPlanetBlockPathRocks>().rocks.SetActive(true);
			GameManager.audioManager.playSong(3);
			bigPappadaDialogue = GameManager.player.GetComponent<DialogueController> ().createNewDialogue ("¿Where am I? \n ¿What is this place?", 3f, false, false);
			yield return StartCoroutine(WaitInterruptable (3f,bigPappadaDialogue));
			bigPappadaDialogue = GameManager.player.GetComponent<DialogueController> ().createNewDialogue ("Looks like i have no choice \n but to must press onwards...", 3f, false, false);
			yield return StartCoroutine(WaitInterruptable (3f,bigPappadaDialogue));
			GameManager.inputController.enableInputController();
		}
	}

	IEnumerator IPTutorial() {
		GUIManager.setTutorialText ("Press 'Down' and Jump to perform a interplanetary jump.");
		GUIManager.activateTutorialText();
		yield return new WaitForSeconds(5f);
	}

	IEnumerator IPTutorialDirections(){
		if(!hasDoneTutorialSpaceJumpDirections){
			GUIManager.setTutorialText ("You can control the jump direction with 'Left' and 'Right'");
			GUIManager.activateTutorialText();
			yield return new WaitForSeconds(5f);
			GUIManager.deactivateTutorialText();
			hasDoneTutorialSpaceJumpDirections = true;
		}
	}

	public override void chargeSpaceJumping(){
		if(isEnabled){
			StartCoroutine (IPTutorialDirections ());
		}
	}

	IEnumerator gemPowerAttackTutorial() {
		//This event is activated when the player has kthe first wave.
		Util.changeTime (0.1f);
		GUIManager.setTutorialText ("Press 'Y' to perform a powerful light attack.");
		GUIManager.activateTutorialText();
		yield return new WaitForSeconds(5f * Util.getTimeProportion());
		//GUIManager.setTutorialText ("You can control the attack direction holding 'Y' and then pressing 'Up' and 'Down'");
		//GUIManager.activateTutorialText();
		//yield return new WaitForSeconds(5f * Util.getTimeProportion());
		GUIManager.deactivateTutorialText();
		Util.changeTime (1f);
	}

	IEnumerator tongueAttackTutorial() {
		//This event is activated when the player has kthe first wave.
		Util.changeTime (0.1f);
		GUIManager.setTutorialText ("Press 'Up' and 'X' to perform a tongue attack");
		GUIManager.activateTutorialText();
		yield return new WaitForSeconds(5f * Util.getTimeProportion());
		GUIManager.deactivateTutorialText();
		Util.changeTime (1f);
	}

	IEnumerator lightgemCinematic(){
		if(isEnabled){
			GameManager.inputController.disableInputController ();
			lightGemGO.GetComponent<SanctuaryLightGem>().isActive = false;
			bigPappadaDialogue = GameManager.player.GetComponent<DialogueController> ().createNewDialogue ("Is that the lightgem of \n Whiteheart sensei!?", 3f, false, false);
			yield return StartCoroutine(WaitInterruptable (3f,bigPappadaDialogue));
			bigPappadaDialogue = GameManager.player.GetComponent<DialogueController> ().createNewDialogue ("I wonder what's it doing here...", 3f, false, false);
			yield return StartCoroutine(WaitInterruptable (3f,bigPappadaDialogue));
			GameManager.player.GetComponent<PlayerController>().Move(-1f);
			
			yield return new WaitForSeconds(0.25f);
			GameManager.player.GetComponent<PlayerController>().StopMove();

			float timer = 0f;
			float time = 3f;
			Vector3 lightGemOriginalPosition = GameManager.player.GetComponent<Rigidbody>().worldCenterOfMass;

			while(timer<time){
				timer+=Time.deltaTime;
				float proportion = timer/time;
				lightGemGO.GetComponent<SanctuaryLightGem>().lightGemGO.transform.position = Vector3.Lerp(lightGemOriginalPosition,GameManager.playerController.lightGemObject.transform.position,proportion);
				yield return null;
			}
			foreach(ParticleSystem particles in lightGemGO.GetComponentsInChildren<ParticleSystem>()){
				particles.Stop();
			}
			bigPappadaDialogue = GameManager.player.GetComponent<DialogueController> ().createNewDialogue ("Maybe I can use this \n to get out of here!", 3f, true, false);
			yield return StartCoroutine(WaitInterruptable (3f,bigPappadaDialogue));
			GameManager.audioManager.playSong(4);
			foreach(Collider collider in bridgeFallGO.GetComponent<FirstPlanetFallingFromTheBridge>().fallenRocks.GetComponentsInChildren<Collider>()){
				collider.enabled = false;
			}
			foreach(Collider collider in lightGemGO.GetComponent<SanctuaryLightGem>().rocksGO.GetComponentsInChildren<Collider>()){
				collider.enabled = false;
			}

			bridgeFallGO.GetComponent<FirstPlanetFallingFromTheBridge>().hideOutsidePlane.SetActive(false);
			GameManager.playerAnimator.SetBool("isChargingSpaceJumping",true);
			yield return new WaitForSeconds(2f);
			float originalForce = GameManager.player.GetComponent<PlayerController>().spaceJumpForce;
			GameManager.player.GetComponent<PlayerController>().spaceJumpForce = 30f;
			Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"),LayerMask.NameToLayer("Planets"),true);
			GameManager.player.GetComponent<PlayerController>().SpaceJump(GameManager.player.transform.up,false);
			GUIManager.deactivateSpaceJumpGUI();
			GameManager.player.GetComponent<PlayerController>().spaceJumpForce = originalForce;
			bridgeFallGO.GetComponent<FirstPlanetFallingFromTheBridge>().fallenRocks.GetComponentInChildren<ParticleSystem>().Play();
			lightGemGO.GetComponent<SanctuaryLightGem>().rocksGO.GetComponentInChildren<ParticleSystem>().Play();
			yield return new WaitForSeconds(1f);
			Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"),LayerMask.NameToLayer("Planets"),false);
			GameManager.player.GetComponent<CharacterController>().Move(1f);
			bridgeFallGO.GetComponent<FirstPlanetFallingFromTheBridge>().fallenRocks.SetActive(false);
			bridgeFallGO.GetComponent<FirstPlanetFallingFromTheBridge>().fallenRocksAfter.SetActive(true);
			lightGemGO.GetComponent<SanctuaryLightGem>().rocksGO.SetActive(false);
			lightGemGO.GetComponent<SanctuaryLightGem>().rocksGOAfter.SetActive(true);
			yield return new WaitForSeconds(1f);
			GameManager.player.GetComponent<PlayerController>().StopMove();
			yield return new WaitForSeconds(2f);
			bigPappadaDialogue = GameManager.player.GetComponent<DialogueController> ().createNewDialogue ("¡Finally! I'm outside!", 2f, true, false);
			yield return StartCoroutine(WaitInterruptable (2f,bigPappadaDialogue));
			bigPappadaDialogue = GameManager.player.GetComponent<DialogueController> ().createNewDialogue ("¡¿Where is Little G??!", 2f, true, false);
			yield return StartCoroutine(WaitInterruptable (2f,bigPappadaDialogue));
			bigPappadaDialogue = GameManager.player.GetComponent<DialogueController> ().createNewDialogue ("¡I must find him!", 2f, true, false);
			yield return StartCoroutine(WaitInterruptable (2f,bigPappadaDialogue));
			bigPappadaDialogue = GameManager.player.GetComponent<DialogueController> ().createNewDialogue ("¡But first i must \n find a way to \n cleanse this mess!", 4f, true, false);
			yield return StartCoroutine(WaitInterruptable (4f,bigPappadaDialogue));
			GetComponent<PlanetCorruption>().activateSpawning();
			GameManager.inputController.enableInputController();
			GameManager.inputController.enableInputController ();
			GUIManager.setTutorialText("Press 'A' to perform a consecutive attack series");
			GUIManager.activateTutorialText();			
			yield return new WaitForSeconds(5f);
			GUIManager.deactivateTutorialText();
		}
	}


	public override void startButtonPressed(){
		if(isEnabled && !firstCinematicPlayed){
			StartCoroutine("initialCinematic");
		}else{
			GameManager.persistentData.spaceJumpUnlocked = true;
		}
	}

	public override void planetCleansed(){
		if(isEnabled){
			corruptionSeepingGO.GetComponent<FirstPlanetCorruptionSeeping>().corruptionSeeping.SetActive(false);
			StartCoroutine("IPTutorial");
		}
	}

	public override void informEventActivated (CutsceneIdentifyier identifyier){
		if(isEnabled){
			if(identifyier.Equals(CutsceneIdentifyier.FirstPlanetBoarHunting)){
				StartCoroutine("boarHuntingCinematic");
			}else if(identifyier.Equals(CutsceneIdentifyier.FirstPlanetShintoDoor)){
				StartCoroutine("shintoDoorCinematic");
			}else if(identifyier.Equals(CutsceneIdentifyier.FirstPlanetToTheBridge)){
				StartCoroutine("toTheBridgeCinematic");
			}else if(identifyier.Equals(CutsceneIdentifyier.FirstPlanetToTheBridge2)){
				StartCoroutine("toTheBridge2Cinematic");
			}else if(identifyier.Equals(CutsceneIdentifyier.FirstPlanetFallingFromTheBridge)){
				StartCoroutine("fallFromBridgeCinematic");
			}else if(identifyier.Equals(CutsceneIdentifyier.FirstPlanetPathBlockedStones)){
				StartCoroutine("rocksBlockingPathCinematic");
			}else if(identifyier.Equals(CutsceneIdentifyier.FirstPlanetCorruptionSeeping)){
				StartCoroutine("corruptionSeepingCinematic");
			}else if(identifyier.Equals(CutsceneIdentifyier.SanctuaryLightGem)){
				StartCoroutine("lightgemCinematic");
			}
		}
	}

	public override void initialize(){
		if(isEnabled){
			GetComponent<PlanetCorruption> ().setCorruptionToClean ();
		}

	}

	public override void firstWaveFinished(){
		StartCoroutine("gemPowerAttackTutorial");
	}
	public override void secondWaveFinished(){
		StartCoroutine("tongueAttackTutorial");
	}

}
