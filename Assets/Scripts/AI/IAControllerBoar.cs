using UnityEngine;
using System.Collections;

public class IAControllerBoar : IAController {

	private enum ActualBehaviour{Patroling,MeleeAttack,ChargeAttack,ChasePlayer,OffensiveIdle};

	public AttackType frontAttack;
	public AttackType chargeAttack;
	public float patrolTimeToTurn = 1.5f;
	public float maxDistancePlayer = 20f;
	public float maxDistanceMeleeAttack = 0.1f;
	public float chanceToCharge = 0.02f;
	public float chanceToAttackMelee = 0.05f;
	public float meleeAttackCooldown = 0.5f;
	public float chargeAttackCooldown = 1.5f;
	public float maxDistanceCharge = 8f;
	public float lookTime = 0.1f;

	private float meleeAttackTimer = 0f;
	private float chargeAttackTimer = 0f;
	private OutlineChanging outlineChanger;
	private ActualBehaviour actualBehaviour;
	private float patrolTime = 0f;
	private float changeBehaviourTimer = 0f;
	private bool isCharging = false;
	private bool isDoingMeleeAttack = false;
	private float looktimer = 0f;


	protected override void initialize(){
		Attack frontAttackA = attackController.getAttack(frontAttack);
		frontAttackA.informParent(gameObject);
		Attack chargeAttackA = attackController.getAttack(chargeAttack);
		chargeAttackA.informParent(gameObject);
	}

	protected override void UpdateAI(){
		doActualBehaviour ();
		meleeAttackTimer += Time.deltaTime;
		chargeAttackTimer += Time.deltaTime;
		changeBehaviour();
	}


	private void changeBehaviour(){
		//Changes the behaviour depending on the conditions of the AI

		if (!attackController.isDoingAnyAttack()) {
			//We check if we have to reset the melee and charging attack timers
			if(isDoingMeleeAttack){meleeAttackTimer = 0f; isDoingMeleeAttack=false;}
			if(isCharging){chargeAttackTimer = 0f; isCharging = false;}
		}

		if(!canSeePlayer()){
			actualBehaviour = ActualBehaviour.ChasePlayer;
		}else{
			
			if(getPlayerDistance()>=maxDistancePlayer){
				actualBehaviour = ActualBehaviour.ChasePlayer;
			}

			if(getPlayerDistance()<=maxDistanceMeleeAttack){
					//Is at melee range
					if(meleeAttackTimer>meleeAttackCooldown){
						meleeAttackTimer = 0f;
						if(Random.value<=chanceToAttackMelee){
							actualBehaviour = ActualBehaviour.MeleeAttack;
						}else{
							actualBehaviour = ActualBehaviour.OffensiveIdle;
						}
					}else{
						actualBehaviour = ActualBehaviour.OffensiveIdle;
					}

			}else if(getPlayerDistance()<=maxDistanceCharge){
					//Is farther away
					if(chargeAttackTimer>chargeAttackCooldown){
						chargeAttackTimer = 0f;
						if(Random.value<=chanceToCharge){
							actualBehaviour = ActualBehaviour.ChargeAttack;
						}else{
							actualBehaviour = ActualBehaviour.ChasePlayer;
						}
					}else{
						actualBehaviour = ActualBehaviour.ChasePlayer;
					}
					
			}else{
				actualBehaviour = ActualBehaviour.ChasePlayer;
			}
		}
	}

	private void doActualBehaviour(){
		//Does the action that corresponds to the actual behaviour unless it is dead
		if(!isDead){
			if(!attackController.isDoingAnyAttack() && getIsTouchingPlanet()){
				if(actualBehaviour.Equals(ActualBehaviour.Patroling)){
					Patrol();
				}else if(actualBehaviour.Equals(ActualBehaviour.MeleeAttack)){
					if(!attackController.isDoingAnyAttack()){
							StopMoving();
							lookAtDirection(getPlayerDirection());
						if(!attackController.doAttack(frontAttack,false)){
							IdleInFrontOfPlayer();
						}else{
							isDoingMeleeAttack = true;
						}
					}
				}else if(actualBehaviour.Equals(ActualBehaviour.ChargeAttack)){
					if(!attackController.isDoingAnyAttack()){
							StopMoving();
							lookAtDirection(getPlayerDirection());

						if(!attackController.doAttack(chargeAttack,false)){
							actualBehaviour = ActualBehaviour.ChasePlayer;
						}else{
							isCharging = true;
						}
					}
				}else if(actualBehaviour.Equals(ActualBehaviour.ChasePlayer)){
					if(closestThingInFrontDistance()>=minimumDistanceAttackPlayer){
						Move(getPlayerDirection());
					}else{
						StopMoving();
					}
				}else if(actualBehaviour.Equals(ActualBehaviour.OffensiveIdle)){
					IdleInFrontOfPlayer();
				}
			}
		}
	}

	private void IdleInFrontOfPlayer(){
		//Looks in the player direction
		looktimer += Time.deltaTime;
		if(looktimer>lookTime){
			looktimer = 0f;
			lookAtDirection(getPlayerDirection());
		}
	}

	private void Patrol(){
		//Patrols around
		patrolTime += Time.deltaTime;
		if(patrolTime>=patrolTimeToTurn){
			patrolTime = 0f;
			Move(getLookingDirection()*-1f);
		}else{
			Move(getLookingDirection());
		}
	}

	protected override bool virtualGetHurt ()
	{
		GameManager.audioManager.PlayStableSound(10);
		GameManager.audioManager.PlaySoundDontRepeat(5,1.0f);
		return base.virtualGetHurt ();
	}

	protected override void virtualDie ()
	{
		base.virtualDie ();
		GameManager.audioManager.PlaySoundDontRepeat(6,1.0f);
	}

	/***FUNCTIONS***/
	//initialize()
	//UpdateAI()
	//Move(float)
	//StopMoving()
	//Jump()

	/***INFORMATION***/
	//canSeePlayer()
	//getPlayerDistance()
	//getIsBlockedBySomethingInFront()
	//closestThingInFrontDistance()
	//getIsLookingRight()
	//getListCloseAllies()
	//isDoingAttack
	//isDead;
	//isOnGuard;
	//isFrozen;
	//isStunned;

	/***STATES***/
	//

}
