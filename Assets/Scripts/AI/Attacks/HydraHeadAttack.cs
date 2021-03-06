﻿using UnityEngine;
using System.Collections;

public class HydraHeadAttack : Attack {
	
	private IAControllerHydra iaParent;
	private bool hasHurtPlayer = false;
	
	public override void initialize(){
		attackType = AttackType.HydraHeadAttack;
		GetComponentInChildren<Collider> ().enabled = false;
	}
	
	public override void startAttack(){
		StartCoroutine (doHeadAttack ());
		isFinished = false;
	}

	public override void otherCollisionEnter(GameObject enemy,Vector3 point){
		if(enemy.layer.Equals(LayerMask.NameToLayer("Player")) && !hasHurtPlayer){
			GameManager.playerController.getHurt(damage,point);
			hasHurtPlayer = true;
		}
	}

	IEnumerator doHeadAttack(){
		iaParent.getIAAnimator().ResetTrigger("isDoingHeadAttack");
		hasHurtPlayer = false;
		iaParent.getIAAnimator().SetTrigger("isChargingHeadAttack");
		yield return new WaitForSeconds (1.2f);
		iaParent.getIAAnimator().SetTrigger("isDoingHeadAttack");
		yield return new WaitForSeconds (0.5f);
		GetComponentInChildren<Collider> ().enabled = true;
		yield return new WaitForSeconds (0.2f);
		GetComponentInChildren<Collider> ().enabled = false;
		yield return new WaitForSeconds (0.4f);
		isFinished = true;
	}
	
	public override void informParent(GameObject parentObject){
		iaParent = parentObject.GetComponent<IAControllerHydra> ();

		transform.parent = iaParent.headPosition.transform;
		transform.rotation = iaParent.headPosition.transform.rotation;
		transform.position = iaParent.headPosition.transform.position;
	}
}
