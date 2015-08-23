using UnityEngine;
using System.Collections;

public class PenguinWhirlwindAttack : Attack {

	public float timeToCharge = 0.8f;

	public float timeItLasts = 1f;
	public float rotationSpeed = 360f;
	public float speedToAddV = 4f;
	public float timeTillItDoesDamage = 0.2f;
	public GameObject particlesWhirlwind;


	//Private variables
	private GameObject parent;
	private Animator iaAnimator;
	private OutlineChanging outlineChanger;
	private bool hasHitPlayer = false;
	private bool isAttackHurting = false;
	private bool interrupted = false;
	
	public override void initialize(){
		attackType = AttackType.PenguinWhirlwindAttack;
	}

	void OnTriggerStay(Collider enemy){
		if(enemy.gameObject.tag.Equals("Player") && !isFinished && !hasHitPlayer && isAttackHurting && !parent.GetComponent<IAController>().isDead){
			hasHitPlayer = true;
			GameManager.player.GetComponent<PlayerController> ().getHurt (damage,enemy.ClosestPointOnBounds(transform.position));
			Vector3 speedToAdd = GameManager.player.GetComponent<Rigidbody>().worldCenterOfMass - parent.transform.position;
			GameManager.player.GetComponent<Rigidbody>().velocity +=(speedToAdd.normalized * speedToAddV);
		}
	}
	
	public override void startAttack(){
		if(isFinished){
			if(!interrupted){
				StartCoroutine("doAttack");
			}
			isFinished = false;
			hasHitPlayer = true;
			interrupted = false;
			isAttackHurting = false;
		}
	}

	IEnumerator doAttack(){
		float timer = 0f;
		iaAnimator.SetTrigger("isChargingWhirlwind");
		//Charge of the attack
		while(timer<timeToCharge && !interrupted){
			timer+=Time.deltaTime;
			float ratio = timer/timeToCharge;
			outlineChanger.setOutlineColor(Color.Lerp(Color.black,Color.red,ratio));
			yield return null;
		}
		timer = 0f;
		iaAnimator.SetBool("isDoingWhirlwind",true);
		bool activatedHit = false;
		while (timer<timeItLasts && !interrupted) {
			timer+=Time.deltaTime;
			//We activate the particles and damage after a certain time defined by timeTillDoesDamage
			if(timer>=timeTillItDoesDamage && !isAttackHurting && !activatedHit){
				isAttackHurting = true;
				hasHitPlayer = false;
				activatedHit = true;
				particlesWhirlwind.GetComponent<ParticleSystem>().Play();
			}

			//We deactivate the particles and damage of the whirlwind after timeitlasts - timeTillDoesDamage.
			if(timer>= (timeItLasts - timeTillItDoesDamage) && isAttackHurting){
				isAttackHurting = false;
				particlesWhirlwind.GetComponent<ParticleSystem>().Stop();
				outlineChanger.setOutlineColor(Color.black);
			}
			transform.up = parent.transform.up;
			//transform.RotateAround(parent.GetComponent<Rigidbody>().worldCenterOfMass,parent.transform.up,rotationSpeed*Time.deltaTime);
			yield return null;
		}

		yield return null;
		iaAnimator.SetBool("isDoingWhirlwind",false);
		interrupted = false;
		isFinished = true;
	}

	public override void interruptAttack(){
		interrupted = true;
		isAttackHurting = false;
		outlineChanger.setOutlineColor(Color.black);
	}
	
	public override void informParent(GameObject parentObject){
		transform.parent = parentObject.transform;
		transform.rotation = parentObject.transform.rotation;
		transform.position = parentObject.GetComponent<Rigidbody>().worldCenterOfMass + (parentObject.transform.forward*parentObject.GetComponent<WalkOnMultiplePaths>().centerToExtremesDistance*1.3f);
		parent = parentObject;
		iaAnimator = parent.GetComponent<IAController> ().getIAAnimator ();
		outlineChanger = parent.GetComponent<OutlineChanging> ();
	}
}
