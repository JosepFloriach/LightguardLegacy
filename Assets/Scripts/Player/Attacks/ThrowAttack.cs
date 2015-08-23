using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ThrowAttack : Attack, AnimationSubscriber {
	public float positionInFrontPlayer = 0.5f;
	public float timeToActivate = 0.1f;
	public float timeToDeactivate = 0.4f;

	private AnimationEventBroadcast eventHandler;
	private GameObject tongue;
	private Xft.XWeaponTrail weaponEffects;
	private float timeOfAnimation;
	private List<GameObject> enemiesHit;
	private bool hasHitEnemy;

	public override void initialize() {
		attackType = AttackType.ThrowAttack;
		eventHandler = GameManager.playerAnimator.gameObject.GetComponent<AnimationEventBroadcast>();
		eventHandler.subscribe(this);
		tongue = GameManager.player.GetComponent<PlayerController> ().playerTongueColliderObject;
		//weaponEffects = weapon.GetComponentInChildren<Xft.XWeaponTrail>();
	}

	public override void enemyCollisionEnter(GameObject enemy,Vector3 point){
		if(!enemiesHit.Contains(enemy) && !enemy.GetComponent<IAController>().isDead){
			enemiesHit.Add(enemy);
			enemy.GetComponent<IAController>().getHurt(damage,point,true);
			enemy.GetComponent<IAController>().hitCanSendFlying();
			Vector3 direction = enemy.GetComponent<Rigidbody>().worldCenterOfMass - GameManager.player.transform.position;
			enemy.GetComponent<IAController>().sendFlyingByConsecutiveHits(direction * 3f);
			//enemy.GetComponent<IAController> ().interruptAttack ();
			GameManager.comboManager.addCombo ();
			
			if(!hasHitEnemy){
				hasHitEnemy = true;
				GameManager.lightGemEnergyManager.addPoints(1);
			}
		}
	}

	private void enableHitbox(){
		GameManager.audioManager.PlaySound(3);
		enemiesHit = new List<GameObject> (0);
		hasHitEnemy = false;
		tongue.GetComponentInChildren<AttackCollider> ().attack = gameObject;
		tongue.GetComponentInChildren<Collider> ().enabled = true;
		//weaponEffects.Activate();
	}

	private void disableHitbox(){
		tongue.GetComponentInChildren<AttackCollider> ().attack = null;
		tongue.GetComponentInChildren<Collider> ().enabled = false;
		isFinished = true;
		//weaponEffects.StopSmoothly (0.5f);
	}
	
	/*private IEnumerator doThrowAttack(){
		enemiesHit = new List<GameObject>(0);
		weaponEffects.Activate();
		hasHitEnemy = false;
		isFinished = false;
		yield return new WaitForSeconds(timeToActivate);
		weapon.GetComponentInChildren<AttackCollider> ().attack = gameObject;
		weapon.GetComponentInChildren<Collider> ().enabled = true;
		yield return new WaitForSeconds (timeToDeactivate);
		isFinished = true;
		weapon.GetComponentInChildren<AttackCollider> ().attack = gameObject;
		weapon.GetComponentInChildren<Collider> ().enabled = false;
		weaponEffects.StopSmoothly(0.1f);
	}*/
	
	public override void startAttack(){
		if (isFinished) {
			GameManager.playerAnimator.SetTrigger ("isDoingThrowAttack");
			isFinished = false;
		}
	}

	void AnimationSubscriber.handleEvent(string idEvent) {
		switch (idEvent) {
		case "start": 
			//StartCoroutine ("doThrowAttack");
			enableHitbox();
			break;
		case "end":
			disableHitbox();
			//disableHitbox();
			break;
		case "done":
			//endCombo();
			break;
		case "combo":
			//allowChaining();
			break;
		default: 
			
			break;
		}
		
	}

	string AnimationSubscriber.subscriberName() {
		return  "ThrowAttack";	
	}
}
