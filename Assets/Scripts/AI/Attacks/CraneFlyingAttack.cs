using UnityEngine;
using System.Collections;

public class CraneFlyingAttack : Attack {

	//NOT IMPLEMENTED YET!
	
	//Private variables
	private GameObject parent;
	private Animator iaAnimator;
	private OutlineChanging outlineChanger;
	private bool isActive = false;
	private bool doDamage = false;
	public float timeToChargeAttack = 1f;
	public ParticleSystem particles;

	private Vector3 playerOriginalPosition;
	private bool interrupted = false;
	
	public override void initialize(){
		attackType = AttackType.CraneFlyingAttack;
	}
	
	public override void otherCollisionEnter(GameObject enemy,Vector3 position){
		if(isActive && enemy.tag.Equals("Player") && doDamage && !isFinished && !interrupted){
			isActive = false;
			GameManager.player.GetComponent<PlayerController> ().getHurt (damage,position);
		}
	}

	public void onHitGround(){
		if(parent!=null){
			parent.GetComponent<IAControllerCrane> ().resetActionToCallOnTouchGround ();
			outlineChanger.setOutlineColor (Color.black);
			iaAnimator.SetBool ("isFlyingAttacking", false);
			isFinished = true;
			doDamage = false;
			parent.GetComponent<IAControllerCrane> ().resetOriginalPreferredHeight ();
			parent.GetComponent<IAControllerCrane> ().resetOriginalUpSpeed ();
			particles.Stop ();
			interrupted = false;
		}
	}
	
	public override void startAttack(){
		if(isFinished){
			interrupted = false;
			isFinished = false;
			StartCoroutine("doAttack");
		}
	}
	
	private IEnumerator doAttack(){
		iaAnimator.SetBool ("isFlyingAttacking", true);

		if(!interrupted){
			parent.GetComponent<IAControllerCrane> ().setActionToCallOnTouchGround (onHitGround);
			isActive = true;
		}

		float timer = 0f;
		while(timer<timeToChargeAttack/2f && !interrupted){
			timer+=Time.deltaTime;
			float ratio = timer/timeToChargeAttack;
			outlineChanger.setOutlineColor(Color.Lerp(Color.black,Color.red,ratio));
			yield return null;
		}
		parent.GetComponent<IAControllerCrane> ().preferredHeight = 0f;
		parent.GetComponent<IAControllerCrane> ().setUpSpeed (4f);
		
		if (!interrupted) {
			doDamage = true;
			particles.Play ();
		}
	}

	public override void interruptAttack(){
		interrupted = true;
		doDamage = false;
	}
	
	public override void informParent(GameObject parentObject){
		transform.rotation = parentObject.transform.rotation;
		transform.position = parentObject.GetComponent<Rigidbody>().worldCenterOfMass + (parentObject.transform.forward*parentObject.GetComponent<WalkOnMultiplePaths>().centerToExtremesDistance*1.3f);
		parent = parentObject;
		transform.position = parent.GetComponent<IAControllerCrane> ().feet.transform.position;
		transform.parent = parent.GetComponent<IAControllerCrane> ().feet.transform;
		iaAnimator = parent.GetComponent<IAController> ().getIAAnimator ();
		outlineChanger = parent.GetComponent<OutlineChanging> ();
	}
}
