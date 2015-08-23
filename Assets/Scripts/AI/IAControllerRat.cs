using UnityEngine;
using System.Collections;

public class IAControllerRat : IAController {

	static int walkingState = Animator.StringToHash("Walking");
	static int undergroundState = Animator.StringToHash("Underground");

	public AttackType poisonAttack;

	public float chasingSpeed = 1.0f;
	public float patrolSpeed = 0.5f;
	public float patrolTimeToTurn = 1.5f;
	public float minimumDistanceToBurrow = 2f;

	private GameObject burrowParticles;
	private float timeWalkingDirectionIdle = 0f;
	private float attackTimer = 0f;
	private float patrolTime = 0f;
	private float initialY; 


	private bool isJumping;

	protected override void initialize(){
		Attack poisonAttackA = attackController.getAttack(poisonAttack);
		poisonAttackA.informParent (gameObject);
		burrowParticles = gameObject.transform.Find ("BurriedDust").gameObject;
		burrowParticles.SetActive (false);
		isJumping = false;
	}

	protected override void UpdateAI(){
		if (!isDead) {
			UpdateParticles ();
			UpdateModelHiding ();
			UpdateStopMotion ();
			if (isAtVisionRange () && !attackController.isDoingAnyAttack ()) {
				if (isAtMeleeRange ()) {
					if (IsBurried ()) {
						Emerge ();
					}
					;
					PoisonAttack ();
				} else {
					StopAttacking ();
					if (!IsBurried () && (getPlayerDistance () > minimumDistanceToBurrow)) {
						Burrow ();				
					}
				}
				if (!IsBurrowing () && !IsEmerging () && !IsAttacking ()) {
					Chase ();
				}
			} else if (!attackController.isDoingAnyAttack () && !isJumping) {
				Patrol ();
			}
		}
	}

	private void Chase() {
		characterController.setSpeed (chasingSpeed);
		characterController.Move (getPlayerDirection());
		if (!IsBurried ()) {
			jumpingWalk ();
		}
	}

	private void jumpingWalk() {
		//if (!GetComponent<CharacterController>().getIsJumping()) {
		if (GetComponent<GravityBody> ().getIsTouchingPlanet () && !GetComponent<CharacterController> ().getIsJumping ()) {
			characterController.Jump (2);
		}
	}

	private void Patrol(){
		//TODO: Check collisions, so it can turn around. 
		characterController.setSpeed (patrolSpeed);
		patrolTime += Time.deltaTime;
		if(patrolTime>=patrolTimeToTurn){
			patrolTime = 0f;
			characterController.Move(getLookingDirection()*-1f);
		}else{
			characterController.Move(getLookingDirection());
		}
		jumpingWalk ();
	}

	private void Emerge() {
		iaAnimator.SetBool ("Unearthing", true);
		iaAnimator.SetBool ("Burrowing", false);
		iaAnimator.SetBool ("Attack", false);
	}

	private void Burrow() {
		iaAnimator.SetBool ("Burrowing", true);
		iaAnimator.SetBool ("Unearthing", false);
		iaAnimator.SetBool ("Attack", false);
	}


	 private void StopAttacking() {
		iaAnimator.SetBool ("Attack", false);
	}
	private void PoisonAttack() {
		iaAnimator.SetBool ("Attack", true);
		characterController.StopMoving();
		lookAtDirection(getPlayerDirection());
		attackController.doAttack (poisonAttack, false);
	}

	private void StopMoving() {
		characterController.StopMoving ();
	}

	private bool IsWalkingState(){
		bool value = iaAnimator.GetCurrentAnimatorStateInfo (0).IsName("Base.Walking");
		return value;
	}

	private bool IsBurried() {
		bool value = iaAnimator.GetCurrentAnimatorStateInfo (0).IsName("Base.Underground");
		return value;
	}

	private bool IsBurrowing() {
		bool value = iaAnimator.GetCurrentAnimatorStateInfo (0).IsName ("Base.Burrow");
		return value;
	}

	private bool IsAttacking() {
		bool value = iaAnimator.GetCurrentAnimatorStateInfo (0).IsName ("Base.Attack") ;
		return value;
	}

	private bool IsEmerging() {
		bool value = iaAnimator.GetCurrentAnimatorStateInfo (0).IsName ("Base.Unearthing") || iaAnimator.GetCurrentAnimatorStateInfo (0).IsName ("Base.Dummy");
		return value;
	}

	private void UpdateParticles()  {
		if (IsWalkingState () || IsAttacking ()) {
			burrowParticles.SetActive(false);
		} else {
			burrowParticles.SetActive(true);
		}
	} 

	private void UpdateModelHiding() {
		if (IsBurried ()) {
			gameObject.transform.Find ("Model").gameObject.SetActive (false);
			gameObject.GetComponent<BoxCollider>().enabled = false;
			//gameObject.GetComponent<CapsuleCollider>().enabled = false;
			
		} else {
			gameObject.transform.Find ("Model").gameObject.SetActive (true);
			gameObject.GetComponent<BoxCollider>().enabled = true;
			//gameObject.GetComponent<CapsuleCollider>().enabled = true;
		}
	}

	private void UpdateStopMotion() {
		if (IsBurrowing() || IsEmerging () || IsAttacking ()) {
			StopMoving ();
		}
	}
}
