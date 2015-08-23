using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;


public class IAControllerCrane : IAController {

	//IA NOT IMPLEMENTED YET

	private enum ActualBehaviour{FlyingPatroling,FlyingChasing,FlyingAttack,FlyingFalling,GroundAttack,GroundStartFlying};
	
	public AttackType meleeAttack;
	public AttackType flyingAttack;
	public GameObject feet;

	public float minimumDistanceInFront = 0.4f;
	public float patrolTimeToTurn = 1.5f;
	public float maxDistancePlayer = 20f;
	public float minDistanceMeleeAttack = 0.4f;

	private OutlineChanging outlineChanger;
	private ActualBehaviour actualBehaviour;
	private float patrolTime = 0f;
	private float changeBehaviourTimer = 0f;

	public float meleeAttackCooldown = 0.5f;
	public float flyingAttackCooldown = 1.5f;
	public float meleeAttackChance = 0.8f;
	public float flyingAttackChance = 0.5f;
	private bool isDoingMeleeAttack = false;
	private bool isDoingFlyingAttack = false;
	private float meleeAttackTimer = 0f;
	private float flyingAttackTimer = 0f;
	
	private float looktimer = 0f;
	private bool isFlying = false;

	public float timeBetweenGroundRaycast = 0.1f;

	public float preferredHeight = 3f;
	private float preferredHeightOriginal;
	public float upSpeed = 0.5f;
	public LayerMask planetsMask;

	public float closestDistanceAttack = 5f;
	public float farthestDistanceAttack = 8f;
	public float maxDistanceChangeDirection = 13f;

	private float upDirection = 0f;
	private float timeInGround = 2f;
	private float inGroundTimer = 0f;
	private float upSpeedOriginal;
	private float missingDistancePreferredHeight = 0f;
	private Action actionToCallOnTouchGround;
	
	protected override void initialize(){
		upSpeedOriginal = upSpeed;
		preferredHeight += (UnityEngine.Random.value - 0.5f);
		preferredHeightOriginal = preferredHeight;
		Attack meleeAttackA = attackController.getAttack(meleeAttack);
		meleeAttackA.informParent(gameObject);
		Attack flyingAttackA = attackController.getAttack(flyingAttack);
		flyingAttackA.informParent(gameObject);

		GetComponent<GravityBody> ().setHasToApplyForce (false);
		StartCoroutine ("updateFlyingHeight");
		iaAnimator.SetBool ("isFlying",true);
		isFlying = true;
		characterController.Move (1f);
	}
	
	protected override void UpdateAI(){
		flyingAttackTimer += Time.deltaTime;
		meleeAttackTimer += Time.deltaTime;
		changeBehaviour();
		doActualBehaviour ();
	}

	protected override bool virtualGetHurt(){
		GameManager.audioManager.PlayStableSound(10);
		attackController.interruptActualAttacks ();
		actualBehaviour = ActualBehaviour.FlyingFalling;
		iaAnimator.SetBool ("isFlying",false);
		GetComponent<GravityBody> ().setHasToApplyForce (true);
		preferredHeight = 0f;
		GameManager.audioManager.PlaySoundDontRepeat(15,1.0f);
		return true;
	}

	IEnumerator standUpWithDelay(){
		yield return new WaitForSeconds(1f);
		isFlying = false;
		actualBehaviour = ActualBehaviour.GroundStartFlying;
		GetComponent<CharacterController>().StopMoving();
	}

	protected override void virtualOnCollisionEnter(Collision collision){
		if(collision.gameObject.layer.Equals(LayerMask.NameToLayer("Planets"))){
			if(actionToCallOnTouchGround!=null){
				actionToCallOnTouchGround();
			}else if(!attackController.isDoingAnyAttack() && GetComponent<GravityBody> ().getHasToApplyForce()){
				StartCoroutine(standUpWithDelay());
			}
		}
	}

	private IEnumerator updateFlyingHeight(){
		float timer = 0f;
		while(!isDead){
			timer+=Time.deltaTime;
			if(timer>=timeBetweenGroundRaycast){
				timer = 0f;
				RaycastHit hit;
				if (Physics.Raycast(GetComponent<Rigidbody>().worldCenterOfMass,transform.up*-1f, out hit, 20f,planetsMask)){
					float distance = hit.distance;
					missingDistancePreferredHeight = preferredHeight - distance;
					upDirection = missingDistancePreferredHeight;
					upDirection = Mathf.Clamp(upDirection,-1f,1f);
					if(!GetComponent<GravityBody>().getHasToApplyForce()){
						GetComponent<Rigidbody>().velocity = GetComponent<Rigidbody>().velocity/4f;
						if(GetComponent<Rigidbody>().velocity.magnitude<0.4f){
							GetComponent<Rigidbody>().velocity = new Vector3(0f,0f,0f);
						}
					}else{
						upDirection = 0f;
					}
				}
			}
			transform.position = transform.position + (transform.up * upDirection * upSpeed * Time.deltaTime);
			yield return null;
		}
	}


	private void changeBehaviour(){
		if (!attackController.isDoingAnyAttack()) {
			//We check if we have to reset the melee and flying attack timers
			if(isDoingMeleeAttack){meleeAttackTimer = 0f; isDoingMeleeAttack =false;}
			if(isDoingFlyingAttack){flyingAttackTimer = 0f; isDoingFlyingAttack = false;}
		}

		if(!actualBehaviour.Equals(ActualBehaviour.FlyingFalling)){
			if(isFlying){
				float playerDistance = getPlayerDistance();
				if(playerDistance>closestDistanceAttack && playerDistance<farthestDistanceAttack && (getLookingDirection() == getPlayerDirection()) && missingDistancePreferredHeight<0.3f){
					actualBehaviour = ActualBehaviour.FlyingAttack;
					if(flyingAttackTimer>flyingAttackCooldown){
						flyingAttackTimer = 0f;
						if(UnityEngine.Random.value<=flyingAttackChance){
							actualBehaviour = ActualBehaviour.FlyingAttack;
						}
					}
				}else if(playerDistance>maxDistanceChangeDirection){
					actualBehaviour = ActualBehaviour.FlyingChasing;
				}else{
					actualBehaviour = ActualBehaviour.FlyingPatroling;
				}
			}else{
				if(getPlayerDistance()<=minDistanceMeleeAttack){
					if(meleeAttackTimer>meleeAttackCooldown){
						meleeAttackTimer = 0f;
						if(UnityEngine.Random.value<=meleeAttackChance){
							actualBehaviour = ActualBehaviour.GroundAttack;
						}
					}else{
						actualBehaviour = ActualBehaviour.GroundStartFlying;
					}
				}else{
					actualBehaviour = ActualBehaviour.GroundStartFlying;
				}
			}
		}
	}
	
	private void doActualBehaviour(){
		if(!isDead && !attackController.isDoingAnyAttack()){
			if(actualBehaviour.Equals(ActualBehaviour.FlyingChasing)){
				characterController.Move (getPlayerDirection());
			}else if(actualBehaviour.Equals(ActualBehaviour.FlyingPatroling)){
				Patrol ();
			}else if(actualBehaviour.Equals(ActualBehaviour.FlyingFalling)){

			}else if(actualBehaviour.Equals(ActualBehaviour.FlyingAttack)){
				if(attackController.doAttack(flyingAttack,false)){
					isDoingFlyingAttack = true;
				}
			}else if(actualBehaviour.Equals(ActualBehaviour.GroundAttack)){
				GetComponent<CharacterController>().LookLeftOrRight(getPlayerDirection());
				if(attackController.doAttack(meleeAttack,false)){
					isDoingMeleeAttack = true;
				}
			}else if(actualBehaviour.Equals(ActualBehaviour.GroundStartFlying)){
				inGroundTimer+=Time.deltaTime;
				if(inGroundTimer>timeInGround){
					inGroundTimer = 0f;
					isFlying = true;
					iaAnimator.SetBool ("isFlying",true);
					GetComponent<CharacterController>().Move(getPlayerDirection()*-1f);
					GetComponent<GravityBody> ().setHasToApplyForce (false);
					preferredHeight = preferredHeightOriginal;
				}
			}
		}
	}

	public void resetOriginalPreferredHeight(){
		preferredHeight = preferredHeightOriginal;
	}

	public void resetOriginalUpSpeed(){
		upSpeed = upSpeedOriginal;
	}

	public void setUpSpeed(float newSpeed){
		upSpeed = newSpeed;
	}

	public void setActionToCallOnTouchGround(Action action){
		actionToCallOnTouchGround = action;
	}

	public void resetActionToCallOnTouchGround(){
		actionToCallOnTouchGround = null;
	}


	private void Patrol(){
		//Patrols around
	}
}
