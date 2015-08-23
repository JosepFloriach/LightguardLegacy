using UnityEngine;
using System.Collections;

public class PoisonAttack : Attack {

	public float timeToCharge = 0.5f;
	public int totalTicks = 4;
	public float timeBetweenTicks = 0.5f;
	public float attackDuration = 0.1f;

	private GameObject parent;
	private float direction = 0f;
	private Animator iaAnimator;
	private OutlineChanging outlineChanger;
	private float attackTimer = 0f;
	private bool interrupted = false;
	private bool isDoingAttack = false;
	private bool inCooldown = false; 
	private bool isPlayerInsideAttack = false;
	private bool isChargingAttack = false;
	private float chargeAttackTimer = 0f;
	private bool hitPlayer = false;

	public override void initialize(){
		attackType = AttackType.PoisonAttack;
	}

	public override void startAttack(){
		isChargingAttack = true;
		isFinished = false;
		interrupted = false;
	}

	void OnTriggerEnter(Collider col) {
		if(col.tag == "Player"){
			isPlayerInsideAttack = true;
		}
	}
	
	void OnTriggerExit(Collider col){
		if(col.tag == "Player"){
			isPlayerInsideAttack = false;
		}
	}

	protected override void update(){
		if(isChargingAttack){
			chargeAttack();
		}else if(isDoingAttack){
			attack();
		}
	}

	private void chargeAttack(){
		isInterruptable = true;
		chargeAttackTimer+=Time.deltaTime;
		float ratio = chargeAttackTimer / timeToCharge;
		Color newColor = Color.Lerp (Color.black, Color.red, ratio);
		outlineChanger.setOutlineColor (newColor);
		if(chargeAttackTimer>=timeToCharge || interrupted){
			outlineChanger.setOutlineColor (Color.black);
			isChargingAttack = false;
			isDoingAttack = true;
		}
	}

	private void attack(){
		attackTimer+=Time.deltaTime;
		if(attackTimer>=attackDuration || interrupted){
			isFinished = true;
			isDoingAttack = false;
			chargeAttackTimer = 0f;
			attackTimer = 0f;
			attackEffect();
			isInterruptable = false;
		}
	}

	private void attackEffect(){
		if(isPlayerInsideAttack && !interrupted){
			GameManager.player.GetComponent<PlayerController>().Poison(totalTicks,timeBetweenTicks);
		}
	}

	public override void interruptAttack(){
		interrupted = true;
	}

	public override void informParent(GameObject parentObject){
		transform.parent = parentObject.transform;
		transform.rotation = parentObject.transform.rotation;
		transform.position = parentObject.GetComponent<Rigidbody>().worldCenterOfMass + (parentObject.transform.forward*parentObject.GetComponent<WalkOnMultiplePaths>().centerToExtremesDistance*1.5f);
		parent = parentObject;
		iaAnimator = parent.GetComponent<IAController> ().getIAAnimator ();
		outlineChanger = parent.GetComponent<OutlineChanging> ();
	}


}
