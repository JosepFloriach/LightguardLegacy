using UnityEngine;
using System.Collections;

public class MonkeyBananaAttack : Attack {

	//Public Variables
	public float timeToChargeAttack;
		//Determines the parabola effect
	public float minBananaSpeed = 3f;
	public float maxBananaSpeed = 6f;
	public float bananaGravity = 18f;

	//GameObjects
	public GameObject bananaPrefab;

	//Private variables
	private GameObject parent;
	private Animator iaAnimator;
	private OutlineChanging outlineChanger;

	private Vector3 playerOriginalPosition;

	public override void initialize(){
		attackType = AttackType.MonkeyBananaAttack;
	}

	public override void enemyCollisionEnter(GameObject enemy,Vector3 point){
		GameManager.player.GetComponent<PlayerController> ().getHurt (damage,point);
	}
	
	public override void startAttack(){
		isFinished = false;
		StartCoroutine("doAttack");
	}

	private IEnumerator doAttack(){
		//Charge attack
		playerOriginalPosition = GameManager.player.GetComponent<Rigidbody> ().worldCenterOfMass;
		float timer = 0f;
		while(timer<=timeToChargeAttack){
			timer+=Time.deltaTime;
			float ratio = timer/timeToChargeAttack;
			outlineChanger.setOutlineColor(Color.Lerp(Color.black,Color.red,ratio));
			yield return null;
		}
		//Throw banana
		outlineChanger.setOutlineColor(Color.black);
		GameObject banana = GameObject.Instantiate (bananaPrefab);
		//Finishs
		banana.transform.position = parent.GetComponent<Rigidbody>().worldCenterOfMass;
		Vector3 direction = playerOriginalPosition - transform.position;
		float forwardSpeed = (Random.value * (maxBananaSpeed - minBananaSpeed))+minBananaSpeed;
		float distance = Vector3.Distance (banana.transform.position,playerOriginalPosition);
		float timeToImpact = distance / forwardSpeed;
		float upSpeed = (bananaGravity * timeToImpact) / 2f;
		//Minimo 45 grados
		if(upSpeed<forwardSpeed){
			upSpeed = forwardSpeed;
		}
		Vector3 throwDirection = (direction.normalized * forwardSpeed) + (parent.transform.up * upSpeed);
		banana.GetComponent<Banana> ().setParameters (throwDirection,parent.transform.up*-bananaGravity, this);
		isFinished = true;
		yield return null;
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
