using UnityEngine;
using System.Collections;

public class IAControllerMonkey : IAController {

	//IA NOT TESTED WITH FINAL MODEL
	private enum ActualBehaviour{Patroling,RangedAttack,Escape,ChasePlayer,JumpingAround};

	public AttackType rangedAttack;
	public float minimumDistanceInFront = 0.4f;
	public float patrolTimeToTurn = 1.5f;
	public float maxDistancePlayer = 20f;
	public float chanceToRangedAttack = 0.6f;
	public float rangedAttackCooldown = 1f;
	public float maxDistanceCharge = 8f;
	public float lookTime = 0.1f;
	public float minimumPlayerDistanceForRangedAttack = 4f;
	public float maximumPlayerDistanceForRangedAttack = 8f;
	
	private float rangedAttackTimer = 0f;
	private OutlineChanging outlineChanger;
	private ActualBehaviour actualBehaviour;
	private float patrolTime = 0f;
	private float changeBehaviourTimer = 0f;
	private bool isCharging = false;
	private bool isDoingRangedAttack = false;
	private float looktimer = 0f;
	
	protected override void initialize(){
		Attack rangedAttackA = attackController.getAttack(rangedAttack);
		rangedAttackA.informParent(gameObject);
	}
	
	protected override void UpdateAI(){
		changeBehaviour();
		rangedAttackTimer += Time.deltaTime;
		doActualBehaviour ();
	}

	//Changes the behaviour of the enemy depending on the state of it's surroundings
	private void changeBehaviour(){
		if (!attackController.isDoingAnyAttack()) {
			//We check if we have to reset the melee and charging attack timers
			if(isDoingRangedAttack){rangedAttackTimer = 0f; isDoingRangedAttack=false;}
			if(isCharging){rangedAttackTimer = 0f; isCharging = false;}
		}

		//Changes the behaviour depending on the conditions of the AI
		if(getPlayerDistance()<minimumPlayerDistanceForRangedAttack){
			actualBehaviour = ActualBehaviour.Escape;
		}else if(getPlayerDistance()>maximumPlayerDistanceForRangedAttack && canSeePlayer()){
			actualBehaviour = ActualBehaviour.ChasePlayer;
		}else if(canSeePlayer ()){
			if(rangedAttackTimer>rangedAttackCooldown){
				rangedAttackTimer = 0f;
				if(Random.value<=chanceToRangedAttack){
					actualBehaviour = ActualBehaviour.RangedAttack;
				}else{
					actualBehaviour = ActualBehaviour.JumpingAround;
				}
			}
		}else{
			actualBehaviour = ActualBehaviour.Patroling;
		}
	}

	//Does the action that corresponds to the actual behaviour unless it is dead
	private void doActualBehaviour(){
		if(!isDead && !attackController.isDoingAnyAttack()){
			if(actualBehaviour.Equals(ActualBehaviour.ChasePlayer)){
				moveAndAvoid (getPlayerDirection());
			}else if(actualBehaviour.Equals(ActualBehaviour.Escape)){
				moveAndAvoid(getPlayerDirection()*-1f);
			}else if(actualBehaviour.Equals(ActualBehaviour.Patroling)){
				Patrol ();
			}else if(actualBehaviour.Equals(ActualBehaviour.JumpingAround)){
				Patrol (); //Temporal
			}else if(actualBehaviour.Equals(ActualBehaviour.RangedAttack)){
				StopMoving();
				if(getIsTouchingPlanet()){
					if(attackController.doAttack(rangedAttack,false)){
						isDoingRangedAttack = true;
					}
				}
			}
		}
	}

	//Moves and if there is something close in front it jumps
	private void moveAndAvoid(float direction){
		if(closestThingInFrontDistance()<=minimumDistanceInFront ){
			if(getIsTouchingPlanet()){
				Jump();
			}else{
				Move(direction);
			}
		}else{
			Move (direction);
		}
	}

	//Walks to one side and to another alternatively
	private void Patrol(){
		//Patrols around
		patrolTime += Time.deltaTime;
		if(patrolTime>=patrolTimeToTurn){
			patrolTime = 0f;
			moveAndAvoid(getLookingDirection()*-1f);
		}else{
			moveAndAvoid(getLookingDirection());
		}
	}
}
