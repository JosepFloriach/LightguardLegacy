using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MundusSpawnEnemiesAttack : Attack {

	public float timeGoingUp = 2f;

	public GameObject[] enemiesTier2;
	public GameObject[] enemiesTier3;
	//Private variables
	private GameObject[] enemies;
	private GameObject parent;
	private Animator iaAnimator;
	private OutlineChanging outlineChanger;
	private bool interrupted = false;
	private List<GameObject> enemiesAlive;
	
	public override void initialize(){
		attackType = AttackType.MundusSpawningAttack;
		setTier (2);
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
		parent.GetComponent<GravityBody> ().setHasToApplyForce (false);
		parent.GetComponent<Rigidbody> ().isKinematic = true;
		while(timer<timeGoingUp){
			timer+=Time.deltaTime;
			parent.transform.position+= Time.deltaTime * parent.transform.up;
			yield return null;
		}

		parent.GetComponent<IAControllerMundus> ().setProtecting();
		spawnEnemies ();

		while(areEnemiesAlive()){
			yield return null;
		}
		parent.GetComponent<IAControllerMundus> ().stopProtecting();

		timer = 0f;
		while(timer<timeGoingUp){
			timer+=Time.deltaTime;
			parent.transform.position-= Time.deltaTime * parent.transform.up;
			yield return null;
		}

		parent.GetComponent<GravityBody> ().setHasToApplyForce (true);
		parent.GetComponent<Rigidbody> ().isKinematic = false;
		isFinished = true;
	}

	private void spawnEnemies(){
		enemiesAlive = new List<GameObject> (0);
		int distance = 1;
		foreach(GameObject enemy in enemies){
			distance++;
			GameObject newEnemy = GameObject.Instantiate(enemy) as GameObject;
			float direction = 1f;
			if(distance%2 == 0){
				direction = -1f;
			}
			newEnemy.transform.position = transform.position + (parent.transform.forward * 2f * (direction) * distance);
			enemiesAlive.Add(newEnemy);
			parent.GetComponent<IAControllerMundus>().addEnemySpawned(newEnemy);
		}
	}

	private bool areEnemiesAlive(){
		bool allFinished = true;
		foreach(GameObject enemy in enemiesAlive){
			
			if(enemy!=null && !enemy.GetComponent<IAController>().isDead){
				allFinished = false;
				break;
			}
		}

		return !allFinished;
	}
	
	public override void interruptAttack(){
		interrupted = true;
		outlineChanger.setOutlineColor(Color.black);
	}

	public override void setTier(int tier){
		if(tier == 2){
			enemies = enemiesTier2;
		}else if(tier== 3){
			enemies = enemiesTier3;
		}
	}
	
	public override void informParent(GameObject parentObject){
		transform.parent = parentObject.transform;
		transform.rotation = parentObject.transform.rotation;
		transform.position = parentObject.GetComponent<Rigidbody>().worldCenterOfMass;
		parent = parentObject;
		iaAnimator = parent.GetComponent<IAController> ().getIAAnimator ();
		outlineChanger = parent.GetComponent<OutlineChanging> ();
	}
}
