using UnityEngine;
using System.Collections;

public class IAControllerPenguin : IAController {
	
	//IA NOT TESTED WITH FINAL MODEL
	private enum ActualBehaviour{Idle,Sliding,WhirlwindAttack};
	
	public AttackType meleeAttack;
	public AttackType slideMove;
	public float minimumDistanceInFront = 0.4f;
	public float patrolTimeToTurn = 1.5f;
	public float maxDistancePlayer = 20f;
	public float minDistanceMeleeAttack = 0.4f;
	public float chanceToMeleeAttack = 0.6f;
	public float chanceToSlide = 0.2f;
	public float meleeAttackCooldown = 1f;
	public float slideCooldown = 3f;
	public float lookTime = 0.1f;

	private float slideTimer = 0f;
	private float meleeAttackTimer = 0f;
	private OutlineChanging outlineChanger;
	private ActualBehaviour actualBehaviour;
	private float patrolTime = 0f;
	private float changeBehaviourTimer = 0f;

	private bool isSliding = false;
	private bool isDoingMeleeAttack = false;

	private float looktimer = 0f;
	
	protected override void initialize(){
		Attack meleeAttackA = attackController.getAttack(meleeAttack);
		meleeAttackA.informParent(gameObject);

		Attack slideMoveA = attackController.getAttack(slideMove);
		slideMoveA.informParent(gameObject);
	}
	
	protected override void UpdateAI(){
		changeBehaviour();
		meleeAttackTimer += Time.deltaTime;
		slideTimer += Time.deltaTime;
		doActualBehaviour ();
	}

	//Changes the behaviour of the enemy depending on the state of it's surroundings
	private void changeBehaviour(){
		if (!attackController.isDoingAnyAttack()) {
			//We check if we have to reset the melee and charging attack timers
			if(isDoingMeleeAttack){meleeAttackTimer = 0f; isDoingMeleeAttack=false;}
			if(isSliding){slideTimer = 0f; isSliding = false;}

			if(getPlayerDistance()>maxDistancePlayer){
				actualBehaviour = ActualBehaviour.Idle;
			}else{
				//If it's far away it slides, if it's close it does the whirlwind attack
				if(getPlayerDistance()>minDistanceMeleeAttack){
					if(slideTimer>slideCooldown){
						slideTimer = 0f;
						if(Random.value<=chanceToSlide){
							actualBehaviour = ActualBehaviour.Sliding;
						}else{
							actualBehaviour = ActualBehaviour.Idle;
						}
					}else{
						actualBehaviour = ActualBehaviour.Idle;
					}
				}else if(meleeAttackTimer>meleeAttackCooldown){
					meleeAttackTimer = 0f;
					if(Random.value<=chanceToMeleeAttack){
						actualBehaviour = ActualBehaviour.WhirlwindAttack;
					}else{
						actualBehaviour = ActualBehaviour.Idle;
					}
				}else{
					actualBehaviour = ActualBehaviour.Idle;
				}
			}
		}
	}

	//Does the action that corresponds to the actual behaviour unless it is dead
	private void doActualBehaviour(){
		if(!isDead && !attackController.isDoingAnyAttack()){
			if(actualBehaviour.Equals(ActualBehaviour.Idle)){
				lookAtDirection(getPlayerDirection());
				characterController.StopMoving();
			}else if(actualBehaviour.Equals(ActualBehaviour.Sliding)){
				characterController.StopMoving();
				lookAtDirection(getPlayerDirection());
				if(attackController.doAttack(slideMove,false)){
					isSliding = true;
				}

			}else if(actualBehaviour.Equals(ActualBehaviour.WhirlwindAttack)){
				characterController.StopMoving();
				lookAtDirection(getPlayerDirection());
				if(attackController.doAttack(meleeAttack,false)){
					isDoingMeleeAttack = true;
				}

			}
		}
	}

}
