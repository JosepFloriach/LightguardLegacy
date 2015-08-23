using UnityEngine;
using System.Collections;

public class BoarFrontAttack : Attack {
	
	public float timeToChargeAttack = 0.5f;
	public float attackDuration = 0.1f;
	public GameObject particlesAttack;
	public GameObject animator;
	public bool hasPushBack = false;
	public float sphereColliderRadius = 0.15f;


	private float attackTimer = 0f;
	private bool isChargingAttack = false;
	private bool isDoingAttack = false;
	private float chargeAttackTimer = 0f;
	private GameObject parent;
	private Animator iaAnimator;
	private bool isPlayerInsideAttack = false;
	private bool interrupted = false;
	private OutlineChanging outlineChanger;


	public override void initialize(){
		attackType = AttackType.BoarFrontAttack;
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

	public override void startAttack(){
		isChargingAttack = true;
		isFinished = false;
		interrupted = false;
		iaAnimator.SetTrigger ("isChargingHeadbutt");
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
		float ratio = chargeAttackTimer / timeToChargeAttack;
		Color newColor = Color.Lerp (Color.black, Color.red, ratio);
		outlineChanger.setOutlineColor (newColor);
		if(chargeAttackTimer>=timeToChargeAttack || interrupted){
			outlineChanger.setOutlineColor (Color.black);
			isChargingAttack = false;
			isDoingAttack = true;
			iaAnimator.SetTrigger ("isDoingHeadbutt");
			
			if(!interrupted){
				particlesAttack.GetComponent<ParticleSystem>().Play();
			}
		}
	}
	
	private void attack(){
		attackTimer+=Time.deltaTime;
		if(attackTimer>=attackDuration || interrupted){
			particlesAttack.GetComponent<ParticleSystem>().Stop();
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
			GameManager.player.GetComponent<PlayerController>().getHurt(damage,GameManager.player.GetComponentInChildren<Collider>().ClosestPointOnBounds(transform.position));
			if(hasPushBack){
				Vector3 direction = GameManager.player.GetComponent<Rigidbody>().worldCenterOfMass - parent.transform.position;
				GameManager.player.GetComponent<Rigidbody>().AddForce(direction.normalized*15f,ForceMode.VelocityChange);
			}
		}
	}

	public override void interruptAttack(){
		interrupted = true;
	}

	public override void informParent(GameObject parentObject){
		transform.parent = parentObject.transform;
		transform.rotation = parentObject.transform.rotation;
		transform.position = parentObject.GetComponent<Rigidbody>().worldCenterOfMass + (parentObject.transform.forward*parentObject.GetComponent<WalkOnMultiplePaths>().centerToExtremesDistance*1.5f);
		GetComponent<SphereCollider> ().radius = sphereColliderRadius;
		parent = parentObject;
		iaAnimator = parent.GetComponent<IAController> ().getIAAnimator ();
		outlineChanger = parent.GetComponent<OutlineChanging> ();
	}

}
