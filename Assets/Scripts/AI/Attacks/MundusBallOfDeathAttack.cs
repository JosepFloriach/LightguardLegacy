using UnityEngine;
using System.Collections;

public class MundusBallOfDeathAttack : Attack {

	public float timeToChargeTier1;
	public float timeToChargeTier2;
	public float timeToChargeTier3;

	public float timeToSummonBall = 0.7f;
	public float maxScaleSummoningParticles = 2f;
	public GameObject ballOfDeath;
	public GameObject particlesSpawnBallOfDeath;

	public int numBalls = 6;
	//Private variables
	private GameObject parent;
	private Animator iaAnimator;
	private OutlineChanging outlineChanger;
	private bool interrupted = false;
	private Vector3 localPositionRight, localPositionLeft;
	private GameObject backSpawningGO;
	private float timeToCharge = 2f;
	
	public override void initialize(){
		attackType = AttackType.MundusBallOfDeath;
		particlesSpawnBallOfDeath.SetActive (false);
		setTier (1);
	}
	
	public override void enemyCollisionEnter(GameObject enemy,Vector3 point){
		//GameManager.player.GetComponent<PlayerController> ().getHurt (damage);
	}
	
	public override void startAttack(){
		if(isFinished){
			StartCoroutine("doAttack");
			isFinished = false;
			interrupted = false;
		}
	}

	IEnumerator doAttack(){

		float timer = 0f;

		iaAnimator.SetBool ("isChargingBallAttack", true);

		while (timer<timeToCharge) {
			timer+=Time.deltaTime;
			float ratio = timer/timeToCharge;
			outlineChanger.setOutlineColor(Color.Lerp(Color.black,Color.red,ratio));
			yield return null;
		}

		iaAnimator.SetBool ("isChargingBallAttack", false);

		for (int i =0; i<numBalls; i++) {
			Vector3 ballPosition;
			float direction = 0f;
			if(i%2 == 0){
				ballPosition = transform.position;
				direction = 1f;
			}else{
				ballPosition = backSpawningGO.transform.position;
				direction = -1f;
			}

			particlesSpawnBallOfDeath.transform.localScale = new Vector3 (0f, 0f, 0f);
			particlesSpawnBallOfDeath.SetActive (true);
			particlesSpawnBallOfDeath.transform.position = ballPosition;
			timer = 0f;
			while (timer<timeToCharge) {
				timer+=Time.deltaTime;
				float ratio = timer/timeToCharge;
				float proportionScale = ratio * maxScaleSummoningParticles;
				particlesSpawnBallOfDeath.transform.localScale = new Vector3(proportionScale,proportionScale,proportionScale);
				yield return null;
			}

			//We create a new instance because there can be multiple ocurrences at the same time
			GameObject ballOfDeathInstance = GameObject.Instantiate (ballOfDeath) as GameObject;
			ballOfDeathInstance.transform.position = ballPosition;
			ballOfDeathInstance.GetComponent<BallOfDeath> ().setDirectionAndDamage (direction, damage, particlesSpawnBallOfDeath);
			particlesSpawnBallOfDeath.SetActive (false);

			timer = 0f;
			while (timer<timeToSummonBall) {
				timer += Time.deltaTime;
				yield return null;
			}
		}
		iaAnimator.SetTrigger ("isBallOfDeathAttackFinished");
		outlineChanger.setOutlineColor(Color.black);
		isFinished = true;
		interrupted = false;
	}
	
	public override void interruptAttack(){
		interrupted = true;
		outlineChanger.setOutlineColor(Color.black);
	}

	public override void setTier(int tier){
		if(tier==1){
			timeToCharge = timeToChargeTier1;
		}else if(tier==2){
			timeToCharge = timeToChargeTier2;
		}else if(tier==3){
			timeToCharge = timeToChargeTier3;
		}
	}
	
	public override void informParent(GameObject parentObject){
		transform.parent = parentObject.transform;
		transform.rotation = parentObject.transform.rotation;
		transform.position = parentObject.GetComponent<Rigidbody>().worldCenterOfMass + (parentObject.transform.forward*parentObject.GetComponent<WalkOnMultiplePaths>().centerToExtremesDistance*4f);
		backSpawningGO = new GameObject ();
		backSpawningGO.transform.parent = parentObject.transform;
		backSpawningGO.name = "Back Spawning GO";
		backSpawningGO.transform.position = parentObject.GetComponent<Rigidbody>().worldCenterOfMass -(parentObject.transform.forward * parentObject.GetComponent<WalkOnMultiplePaths> ().centerToExtremesDistance * 4f);
		parent = parentObject;
		iaAnimator = parent.GetComponent<IAController> ().getIAAnimator ();
		outlineChanger = parent.GetComponent<OutlineChanging> ();
	}
}
