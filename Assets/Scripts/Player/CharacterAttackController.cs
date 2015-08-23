using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum AttackType{Missiles,
					   Kame,
					   KameDirectional,
					   Shockwave,
		               GuardBreaker,
	                   ThrowAttack,
	                   OnAir,
	                   Combo,
	                   BoarFrontAttack,
	                   BoarChargeAttack,
	                   MonkeyBananaAttack,
					   PoisonAttack,
					   JumpingAttack,
					   PenguinWhirlwindAttack,
					   PenguinSlideMove,
					   CraneFlyingAttack,
					   CraneMeleeAttack,
					   LowAttack,
					   MundusBaseAttack,
					   MundusBallOfDeath,
					   MundusSpawningAttack,
					   MundusFisureAttack};

public class CharacterAttackController : MonoBehaviour {

	//Attacks
	public GameObject[] attacksGO;

	private List<Attack> attacksInCharacter;

	public Attack currentAttack;

	public GameObject blockGO;
	public GameObject dashGO;

	private Block block;
	private Dash dash;
	
	// Initialization
	void Awake () {
		attacksInCharacter = new List<Attack> (0);
		foreach(GameObject attackGO in attacksGO){
			Attack attack = (Instantiate(attackGO) as GameObject).GetComponent<Attack>();
			attacksInCharacter.Add(attack);
		}

		if(blockGO!=null){block = (GameObject.Instantiate (blockGO) as GameObject).GetComponent<Block> ();}
		if(dashGO!=null){dash = (GameObject.Instantiate(dashGO) as GameObject).GetComponent<Dash> ();}
	}

	public Attack getAttack(AttackType aType){
		foreach(Attack attack in attacksInCharacter){
			if(aType.Equals (attack.getAttackType())){
				return attack;
			}
		}

		return null;
	}

	public void Update() {

	}

	public bool isDoingDash(){
		return dash.getIsDoingDash ();
	}

	public bool isDashOnCooldown(){
		return !dash.isCooldownFinished ();
	}

	public bool isDoingBlock(){
		return block.getIsDoingBlock();
	}

	public bool isBlockOnCooldown(){
		return !block.isCooldownFinished ();
	}

	public void doDash(){
		if(dash.isCooldownFinished()){
			dash.startAction();
		}
	}

	public bool doAttack(AttackType aType,bool isPlayer){
		Attack attackToDo = getAttack (aType);
		if(attackToDo!=null){
			if(isPlayer){
				if(attackToDo.canPayAttackCost() && attackToDo.canDoNextAttack()){
					GameManager.lightGemEnergyManager.substractPoints(attackToDo.cost);
					attackToDo.startAttack ();
					currentAttack = attackToDo;
					return true;
				}
			}else{
				if(GameManager.enemyAttackManager.canPayAttack(getAttack(aType).cost) && attackToDo.canDoNextAttack()){
					attackToDo.startAttack ();
					GameManager.enemyAttackManager.doNewAttack(getAttack(aType).cost);
					return true;
				}
			}
		}else{
			Debug.Log("No attack exists for the type given");
		}
		return false;
	}

	public bool isMovementLocked(){
		foreach(Attack attack in attacksInCharacter){
			if(attack.lockMovement()){
				return true;
			}
		}
		return false;
	}

	public void doBlock() {
		block.StartBlock();
	}

	public bool isDoingAnyAttack(){
		foreach(Attack attack in attacksInCharacter){
			if(!attack.isAttackFinished()){return true;}
		}

		return false;
	}

	public bool canDoAttack(){
		foreach(Attack attack in attacksInCharacter){
			if(!attack.canDoNextAttack()){return false;}
		}
		
		return true;
	}

	public void interruptActualAttacks(){
		foreach(Attack attack in attacksInCharacter){
			attack.interruptAttack();
		}
	}
}
