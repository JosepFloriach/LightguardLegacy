using UnityEngine;
using System.Collections;

public class IAControllerBoarBoss : IAController {

	private enum ActualBehaviour{Patroling,MeleeAttack,ChargeAttack,ChasePlayer,Stunned};

	public AttackType frontAttack;
	public AttackType chargeAttack;
	public float patrolTimeToTurn = 1.5f;

	public float lookTime = 0.1f;
	public float maxDistanceMeleeAttack = 0.1f;
	public float phase1Duration = 10f;
	public float stunDuration = 5f;

	private float patrolTime = 0f;
	private float changeBehaviourTimer = 0f;

	private float looktimer = 0f;
	private bool isCharging = false;
	private bool isDoingMeleeAttack = false;
	private ActualBehaviour actualBehaviour;
	private bool phase1 = true;
	private bool phase2 = false;
	private bool stunned = false;
	private float timeInPhase1 = 0f;
	private float timeInPhase2 = 0f;
	private float timeStunned = 0f;

	protected override void initialize(){
		Attack frontAttackA = attackController.getAttack(frontAttack);
		frontAttackA.informParent(gameObject);
		Attack chargeAttackA = attackController.getAttack(chargeAttack);
		chargeAttackA.informParent(gameObject);
	}

	protected override void UpdateAI(){
		if (GameManager.player.transform.parent != null) {
			if (GameManager.player.transform.parent.name.Equals("Galaxy Boss Planet")){
				doActualBehaviour ();
				changeBehaviour ();
			}
		}
	}

	private void changeBehaviour(){
		//Changes the behaviour depending on the conditions of the AI
		//actualBehaviour = ActualBehaviour.ChargeAttack;
		if (phase1) {
			if (getPlayerDistance () >= maxDistanceMeleeAttack) {
				actualBehaviour = ActualBehaviour.ChasePlayer;
			} else {
				actualBehaviour = ActualBehaviour.MeleeAttack;
			}
			timeInPhase1 += Time.deltaTime;
			if (timeInPhase1 >= phase1Duration) {
				timeInPhase1 = 0;
				phase1 = false;
				stunned = false;
				phase2 = true;
			}
		}else if (phase2) {
			actualBehaviour = ActualBehaviour.ChargeAttack;
			timeInPhase2 += Time.deltaTime;
			//Debug.Log (timeInPhase2);
			if (timeInPhase2 >= 6.5f){
				timeInPhase2 = 0;
				phase1 = false;
				stunned = true;
				phase2 = false;
			}
		} else if(stunned) {
			actualBehaviour = ActualBehaviour.Stunned;
			timeStunned += Time.deltaTime;
			//Debug.Log (timeStunned);
			if (timeStunned >= stunDuration ) {
				timeStunned = 0;
				phase1 = true;
				phase2 = false;
				stunned = false;
			}
		} 
	}

	private void doActualBehaviour(){
		//Does the action that corresponds to the actual behaviour unless it is dead
		if(!isDead){
			if(!attackController.isDoingAnyAttack() && getIsTouchingPlanet()){
				if(actualBehaviour.Equals(ActualBehaviour.MeleeAttack)){
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
				} else if (actualBehaviour.Equals (ActualBehaviour.Stunned)) {
					//Do Nothing.
					StopMoving();
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
