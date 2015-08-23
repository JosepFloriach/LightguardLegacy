using UnityEngine;
using System.Collections;
using System.Collections.Generic;


[RequireComponent (typeof (PlayerController))]

public class InputController : MonoBehaviour {

	public float startChargeSpaceJump;
	public float timeIsSpaceJumpCharged;
	public float maxDistanceToInteract;
	
	private bool isSpaceJumpCharging = false;
	private bool isSpaceJumpCharged;
	private float timeJumpPressed;
	private PlayerController character;
	private CharacterAttackController attackController;
	private SpaceGravityBody characterGravityBody;

	private bool isCraftingMenuOpen = false;

	public AttackType upSpecialAttack;
	public AttackType sidesSpecialAttack;
	public AttackType downSpecialAttack;

	public AttackType upNormalAttack;
	public AttackType sidesNormalAttack;
	public AttackType downNormalAttack;

	public AttackType onAirAttack;

	private bool isEnabled = true;
	private float timeSinceGameReenabled = 0f;
	
	void Start () {
		timeJumpPressed = 0;
		character = GetComponent<PlayerController>();
		characterGravityBody = character.GetComponent<SpaceGravityBody> ();
		attackController = GetComponent<CharacterAttackController> ();
		WeaponManager wpm = WeaponManager.Instance;
	}

	void Update() {
		//We wait some time to avoid jumping when we select to start the game
		timeSinceGameReenabled += Time.deltaTime;
		if(GameManager.isGameEnded || GameManager.isGamePaused){
			timeSinceGameReenabled = 0f;
		}

		if(!GameManager.isGameEnded && isEnabled && timeSinceGameReenabled>0.2f && !character.getIsFallingDown()){
			//MOVEMENT BUTTON
			if(!attackController.isDoingDash()){
				if (Input.GetAxis ("Horizontal")!=0f) {
					if(isSpaceJumpCharged){
						character.MoveArrow(Input.GetAxisRaw ("Horizontal"),Input.GetAxis ("Vertical"));
					}else if(isCharacterAllowedToMove()){
						ResetJumping ();
						if(attackController.isDoingAnyAttack()){
							character.Move (Input.GetAxisRaw("Horizontal")/2f);
						}else{
							character.Move (Input.GetAxisRaw("Horizontal"));
						}

					}else{
						character.StopMove ();
					}
				} else {
					character.StopMove ();
				}
			}

			//NORMAL ATTACK BUTTON

			/*if(character.getIsJumping() && !character.getIsSpaceJumping()){
				if (Input.GetButtonDown("Normal Attack")) {
					attackController.doAttack(onAirAttack,true);
				}
			}else{*/
			if (Input.GetButtonDown("Normal Attack") && Input.GetAxisRaw("Vertical")>0.5f && isCharacterAllowedToDoNormalAttack()) {
				attackController.doAttack(upNormalAttack,true);
			}
				if (Input.GetButtonDown("Normal Attack") && isCharacterAllowedToDoNormalAttack()) {
					attackController.doAttack(sidesNormalAttack,true);
				}
			//}

			//SPECIAL ATTACK BUTTON
			KameAttackDirectionable kameDir = attackController.getAttack(sidesSpecialAttack) as KameAttackDirectionable;
			if (isCharacterAllowedToDoSpecialAttack()) {
				if(Input.GetButtonDown("Special Attack")){
					
					if (kameDir.isAttackFinished()) {
						attackController.doAttack(sidesSpecialAttack,true);
					} 
	
					kameDir.Detonate();
					
				
				}else if(Input.GetButtonUp("Special Attack")){
					kameDir.buttonReleased();
				}else if(kameDir.canReceiveInputDirections()){
					kameDir.receiveInputDirections(Input.GetAxisRaw("Vertical"),Input.GetAxisRaw("Horizontal"));
				}
				/*if(Mathf.Abs(Input.GetAxisRaw("Vertical"))>Mathf.Abs(Input.GetAxisRaw("Horizontal"))){
					if(Input.GetAxis("Vertical")>0.5f){
						attackController.doAttack(upSpecialAttack,true);
					}else if(Input.GetAxis("Vertical")<-0.5f){
						attackController.doAttack(downSpecialAttack,true);
					}
				}else{
					attackController.doAttack(sidesSpecialAttack,true);
				}*/

			} 

			//JUMP BUTTON
			if (Input.GetButtonDown("Jump") && isSpaceJumpCharged) {
				ResetJumping(); 
				character.SpaceJump(); 
			}

			if(Input.GetButtonDown("Jump") && (Input.GetAxisRaw("Vertical")<-0.5f || isSpaceJumpCharging)){
				if(isCharacterAllowedToSpaceJump()){
					isSpaceJumpCharged = true; character.ChargeJump();
				}
			}else if(Input.GetButtonDown("Jump") && isCharacterAllowedToJump()) {
				character.Jump(); 
			}

			if (Input.GetButtonDown("Jump")){
				if(character.getIsSpaceJumping()){
					SpaceGravityBody body = GetComponent<SpaceGravityBody>();
					if(body.getIsOrbitingAroundPlanet()){
						body.setIsGettingOutOfOrbit(true);
					}
				}
			}

			if (Input.GetButtonDown("Block")) {
				Interactuable entity = EntityManager.getClosestInteractuable();
				SpaceGravityBody body = GetComponent<SpaceGravityBody>();
				if(isSpaceJumpCharged){
					CancelChargingSpaceJump();
				}else if(character.getIsSpaceJumping() && body.getIsOrbitingAroundPlanet()){
					body.setIsFallingIntoPlanet(true);
				}else if (entity != null){ 
					entity.doInteractAction();
				}else if(Input.GetAxis("Vertical")<-0.5f && isCharacterAllowedToBlock()){
					attackController.doBlock();
				}else if(isCharacterAllowedToDash()){
					attackController.doDash();
				}
			}

			if(Input.GetButtonUp("PauseMenu")){
				if(!GameManager.isGameEnded && !GameManager.playerController.getIsSpaceJumping() && !GameManager.playerController.getIsChargingSpaceJump()){
					if(!GameManager.isGamePaused){
						GameManager.pauseGame();
						GUIManager.activatePauseMenu();
					}else{
						GameManager.unPauseGame();
						GUIManager.deactivatePauseMenu();
					}
				}
			}
		}else if(!isEnabled){
			attackController.interruptActualAttacks();
			if (Input.GetButtonUp("Jump")){
				//If it's not enabled, interrupt any ongoing cinematic 
				Planet actualPlanet = GameManager.playerSpaceBody.getClosestPlanet();
				if(actualPlanet!=null && actualPlanet.isPlanetCorrupted()){
					PlanetEventsManager pem = (actualPlanet as PlanetCorrupted).getPlanetEventsManager();
					if(pem!=null){
						pem.interrupt();
					}
				}
			}
		}
	}

	bool isCharacterAllowedToJump(){
		if(character.getIsSpaceJumping()){
			return false;
		}else if(character.getIsJumping()){
			return false;
		}

		return true;
	}

	bool isCharacterAllowedToSpaceJump(){
		if(character.getIsSpaceJumping()){
			return false;
		}else if(character.getIsJumping()){
			return false;
		}else if(GameManager.playerSpaceBody.getClosestPlanet()!=null && GameManager.playerSpaceBody.getClosestPlanet().isPlanetCorrupted() && !(GameManager.playerSpaceBody.getClosestPlanet() as PlanetCorrupted).canPlayerSpaceJumpInPlanet()){
			return false;
		}else if(GameManager.getIsInsidePlanet()){
			return false;
		}else if(!GameManager.persistentData.spaceJumpUnlocked){
			return false;
		}
		return true;
	}

	bool isCharacterAllowedToMove(){
		/*if(GetComponent<CharacterAttackController>().isDoingAnyAttack() && !character.getIsJumping()){
			return false;
		}else */
		if(attackController.isMovementLocked()){
			return false;
		}
		return true;
	}

	bool isCharacterAllowedToDoSpecialAttack(){
		if(character.getIsSpaceJumping()){
			return false;
		}else if(!GetComponent<CharacterAttackController>().canDoAttack()){
			return false;
		}
		return true;
	}

	bool isCharacterAllowedToDoNormalAttack(){
		if(character.getIsSpaceJumping()){
			return false;
		}else if(!GetComponent<CharacterAttackController>().canDoAttack()){
			return false;
		}
		return true;
	}

	bool isCharacterAllowedToDash(){
		if(character.getIsSpaceJumping()){
			return false;
		}else if(attackController.isDoingDash() || attackController.isDashOnCooldown()){
			return false;
		}else if(attackController.isDoingBlock()){
			return false;
		}
		return true;
	}

	bool isCharacterAllowedToBlock(){
		if(character.getIsSpaceJumping()){
			return false;
		}else if (attackController.isDoingAnyAttack ()) {
			return false;
		}else if (attackController.isDoingBlock () || attackController.isBlockOnCooldown ()) {
			return false;
		}
		return true;
	}

	void CancelChargingSpaceJump(){
		timeJumpPressed = 0f;
		isSpaceJumpCharged = false;
		isSpaceJumpCharging = false;
		character.CancelChargingSpaceJump ();
	}

	void ResetJumping () {
		isSpaceJumpCharging = false;
		isSpaceJumpCharged = false;
		timeJumpPressed = 0f;
	}

	public void disableInputController(){
		character.StopMove ();
		GUIManager.deactivatePlayingGUI ();
		isEnabled = false;
	}

	public void enableInputController(){
		isEnabled = true;
		GUIManager.activatePlayingGUIWithFadeIn ();
	}
}