using UnityEngine;
using System.Collections;

public class MundusBaseAttack : Attack {


	public GameObject attackParticles;
	public float speedTier1;
	public float speedTier2;
	public float speedTier3;
	public float timeToChargeTier1;
	public float timeToChargeTier2;
	public float timeToChargeTier3;

	//Private variables
	private float speed;
	private float timeToCharge;
	private GameObject parent;
	private Animator iaAnimator;
	private OutlineChanging outlineChanger;
	private bool interrupted = false;
	private bool hasHurtPlayer = false;
	private ParticleSystem attackParticlesRight,attackParticlesLeft;
	private float timeItLasts = 2f;

	private Vector3 playerDirectionRight,playerDirectionLeft;

	GameObject leftClawParticles;
	GameObject rightClawParticles;
	
	public override void initialize(){
		attackType = AttackType.MundusBaseAttack;
		setTier (1);
	}

	public override void otherCollisionEnter(GameObject enemy,Vector3 point){
		if(enemy.layer.Equals(LayerMask.NameToLayer("Player")) && !hasHurtPlayer){
			GameManager.playerController.getHurt(damage,point);
			hasHurtPlayer = true;
			setParticles(false);
		}
	}

	private void setOriginalParticlesPosition(){
		leftClawParticles.transform.position = parent.GetComponent<IAControllerMundus> ().leftClaw.transform.position;
		rightClawParticles.transform.position = parent.GetComponent<IAControllerMundus> ().rightClaw.transform.position;
	}

	private void setParticles(bool setEnabled){
		attackParticlesRight.GetComponent<Collider> ().enabled = setEnabled;
		attackParticlesLeft.GetComponent<Collider> ().enabled = setEnabled;
		if (setEnabled) {
			attackParticlesRight.Play ();
			attackParticlesLeft.Play ();
		} else {
			attackParticlesRight.Stop ();
			attackParticlesLeft.Stop ();
		}
	}

	IEnumerator doAttack(){
		hasHurtPlayer = false;
		setOriginalParticlesPosition();

		setParticles (true);

		float timer = 0f;
		iaAnimator.SetBool ("isChargingBaseAttack", true);
		while (timer<timeToCharge) {
			timer+=Time.deltaTime;
			float ratio = timer/timeToCharge;
			outlineChanger.setOutlineColor(Color.Lerp(Color.black,Color.red,ratio));
			setOriginalParticlesPosition();
			yield return null;
		}

		playerDirectionRight = GameManager.player.GetComponent<Rigidbody>().worldCenterOfMass - rightClawParticles.transform.position;
		playerDirectionLeft = GameManager.player.GetComponent<Rigidbody>().worldCenterOfMass - leftClawParticles.transform.position;
		Vector3 particlesMovementRight = playerDirectionRight.normalized * speed;
		Vector3 particlesMovementLeft = playerDirectionLeft.normalized * speed;

		iaAnimator.SetBool ("isChargingBaseAttack", false);
		timer = 0f;
		while (timer<timeItLasts) {
			timer+=Time.deltaTime;
			leftClawParticles.transform.position += particlesMovementLeft * Time.deltaTime;
			rightClawParticles.transform.position += particlesMovementRight * Time.deltaTime;
			yield return null;
		}

		setParticles (false);

		outlineChanger.setOutlineColor(Color.black);
		isFinished = true;
		interrupted = false;
	}

	public override void startAttack(){
		if(isFinished){
			StartCoroutine("doAttack");
			isFinished = false;
			interrupted = false;
		}
	}

	public virtual void setTier(int tier){
		if(tier==1){
			speed= speedTier1;
			timeToCharge = timeToChargeTier1;
		}else if(tier==2){
			speed= speedTier2;
			timeToCharge = timeToChargeTier2;
		}else if(tier==3){
			speed= speedTier3;
			timeToCharge = timeToChargeTier3;
		}
	}
	
	public override void informParent(GameObject parentObject){
		transform.parent = parentObject.transform;
		transform.rotation = parentObject.transform.rotation;
		transform.position = parentObject.GetComponent<Rigidbody>().worldCenterOfMass + (parentObject.transform.forward*parentObject.GetComponent<WalkOnMultiplePaths>().centerToExtremesDistance*1.3f);
		parent = parentObject;
		iaAnimator = parent.GetComponent<IAController> ().getIAAnimator ();
		outlineChanger = parent.GetComponent<OutlineChanging> ();
		
		leftClawParticles = GameObject.Instantiate (attackParticles);
		leftClawParticles.GetComponent<AttackCollider> ().attack = gameObject;
		rightClawParticles = GameObject.Instantiate (attackParticles);
		rightClawParticles.GetComponent<AttackCollider> ().attack = gameObject;
		
		setOriginalParticlesPosition ();
		attackParticlesRight = rightClawParticles.GetComponent<ParticleSystem> ();
		attackParticlesLeft = leftClawParticles.GetComponent<ParticleSystem> ();
	}
}
