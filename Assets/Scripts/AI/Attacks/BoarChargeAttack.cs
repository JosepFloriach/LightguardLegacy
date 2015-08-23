using UnityEngine;
using System.Collections;

public class BoarChargeAttack : Attack {


	public float timeItLastsCharge = 6f;
	public float chargeSpeed = 6f;
	public float velocityAppliedToPlayer = 2f;
	public float velocityAppliedToEnemy = 2f;
	public float timeBeforeCharge = 2f;
	public Vector3 localPosition;
	public GameObject chargeParticles;
	public GameObject whileChargingParticles;

	private GameObject parent;
	private float attackTimer = 0f;
	private bool isChargingAttack = false;
	private bool isDoingAttack = false;
	private float chargeAttackTimer = 0f;
	private Animator iaAnimator;
	private float originalSpeed = 0f;
	private float direction = 0f;
	private OutlineChanging outlineChanger;
	private bool interrupted = false;
	
	private bool isPlayerInsideAttack = false;

	public override void initialize(){
		//Called when the game starts it sets the starting parameters
		attackType = AttackType.BoarChargeAttack;
		whileChargingParticles.SetActive (true);
		whileChargingParticles.GetComponent<ParticleSystem> ().Stop ();
	}

	void OnTriggerEnter(Collider col) {
		if(!parent.GetComponent<IAController>().isDead && !interrupted){
			if(col.tag == "Player"){
				GameManager.player.GetComponent<Rigidbody>().velocity = GameManager.player.transform.up * velocityAppliedToPlayer;
				GameManager.player.GetComponent<PlayerController>().getHurt(damage,col.ClosestPointOnBounds(transform.position));
			}else if(col.gameObject.tag == "Enemy"){
				col.gameObject.GetComponent<Rigidbody>().velocity += col.gameObject.transform.up * velocityAppliedToEnemy;
			}
		}
	}
	
	void OnCollisionEnter(Collision col) {
		if(!parent.GetComponent<IAController>().isDead && !interrupted){
			if(col.gameObject.tag == "Player"){
				GameManager.player.GetComponent<Rigidbody>().velocity = GameManager.player.transform.up * velocityAppliedToEnemy;
				GameManager.player.GetComponent<PlayerController>().getHurt(damage,col.contacts[0].point);
			}else if(col.gameObject.tag == "Enemy"){
				col.gameObject.GetComponent<Rigidbody>().velocity += col.gameObject.transform.up * velocityAppliedToEnemy;
			}
		}
	}

	//Called when the attack is started
	public override void startAttack(){
	
		bool isRight = parent.GetComponent<CharacterController>().getIsLookingRight();
		if(isRight){
			direction = 1f;
		}else{
			direction = -1f;
		}
		isFinished = false;
		interrupted = false;
		StartCoroutine ("attack");
	}

	private IEnumerator attack(){
		chargeParticles.SetActive (true);
		originalSpeed = parent.GetComponent<CharacterController> ().speed;
		parent.GetComponent<CharacterController> ().speed = chargeSpeed;
		iaAnimator.SetTrigger("isChargingChargeAttack");

		float timer = 0f;
		while(timer<timeBeforeCharge && !interrupted && !parent.GetComponent<IAController>().isDead){
			timer+=Time.deltaTime;
			float ratio = timer/timeBeforeCharge;
			Color newColor = Color.Lerp (Color.black, Color.red, ratio);
			outlineChanger.setOutlineColor (newColor);
			yield return null;
		}

		//yield return new WaitForSeconds (timeBeforeCharge);
		chargeParticles.SetActive (false);
		iaAnimator.SetBool("isDoingChargeAttack",true);
		yield return null;
		GetComponent<Collider> ().enabled = true;
		whileChargingParticles.GetComponent<ParticleSystem> ().Play ();
		
		timer = 0f;
		parent.layer = LayerMask.NameToLayer ("Dashing");
	
		while(timer<=timeItLastsCharge && !interrupted && !parent.GetComponent<IAController>().isDead){
			timer+=Time.deltaTime;
			parent.GetComponent<IAController> ().Move(direction);
			
			yield return null;
		}

		parent.layer = LayerMask.NameToLayer ("Enemy");

		interrupted = false;
		outlineChanger.setOutlineColor (Color.black);
		//whileChargingParticles.SetActive (false);
		whileChargingParticles.GetComponent<ParticleSystem> ().Stop ();
		iaAnimator.SetBool("isDoingChargeAttack",false);
		isFinished = true;
		parent.GetComponent<CharacterController> ().speed = originalSpeed;
		GetComponent<Collider> ().enabled = false;
		yield return null;
	}

	
	public override void interruptAttack(){
		interrupted = true;
	}

	public override void informParent(GameObject parentObject){
		transform.parent = parentObject.transform;
		transform.rotation = parentObject.transform.rotation;
		//transform.eulerAngles = new Vector3 (0f, 0f, 0f);
		transform.localPosition = localPosition;
		parent = parentObject;
		iaAnimator = parent.GetComponent<IAController> ().getIAAnimator ();
		outlineChanger = parent.GetComponent<OutlineChanging> ();
	}
}
