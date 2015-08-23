using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GuardBreakAttack : Attack {
	
	public float positionInFrontPlayer = 0.5f;
	public float timeToActivate = 0.1f;
	public float timeToDeactivate = 0.4f;

	private bool hasHitEnemy;
	private GameObject weapon;
	private List<GameObject> enemiesHit;

	private Xft.XWeaponTrail weaponEffects;

	public override void initialize() {
		attackType = AttackType.GuardBreaker;
		weapon = GameManager.player.GetComponent<PlayerController> ().weapon;
		weaponEffects = weapon.GetComponentInChildren<Xft.XWeaponTrail>();
		//weaponEffects.StopSmoothly(0.1f);
	}

	protected override void update(){

	}

	public override void enemyCollisionEnter(GameObject enemy,Vector3 point){
		if(!enemiesHit.Contains(enemy) && !enemy.GetComponent<IAController>().isDead){
			enemiesHit.Add(enemy);
			enemy.GetComponent<IAController>().getHurt(damage,(enemy.transform.position),true);
			enemy.GetComponent<IAController> ().breakGuard ();
			GameManager.comboManager.addCombo ();
			if(!hasHitEnemy){
				hasHitEnemy = true;
				GameManager.lightGemEnergyManager.addPoints(1);
		}
		}
	}

	private IEnumerator doGuardBreak(){
		enemiesHit = new List<GameObject>(0);
		weaponEffects.Activate();
		hasHitEnemy = false;
		GameManager.playerAnimator.SetTrigger("isDoingGuardBreaker");
		//triggerBox.transform.position = GameManager.player.GetComponent<Rigidbody> ().worldCenterOfMass + GameManager.player.transform.forward.normalized * positionInFrontPlayer;
		isFinished = false;
		yield return new WaitForSeconds(timeToActivate);
		weapon.GetComponentInChildren<AttackCollider> ().attack = gameObject;
		weapon.GetComponentInChildren<Collider> ().enabled = true;
		yield return new WaitForSeconds (timeToDeactivate);
		isFinished = true;
		weapon.GetComponentInChildren<AttackCollider> ().attack = null;
		weapon.GetComponentInChildren<Collider> ().enabled = false;
		weaponEffects.StopSmoothly(0.1f);
	}
	
	public override void startAttack(){
		StartCoroutine ("doGuardBreak");
	}

}
