using UnityEngine;
using System.Collections;

public class HydraFlameAttack : Attack {

	private IAControllerHydra iaParent;

	private bool hasHurtPlayer = false;
	
	public override void initialize(){
		attackType = AttackType.HydraFlameAttack;
		GetComponentInChildren<Collider> ().enabled = false;
	}
	
	public override void startAttack(){
		StartCoroutine (doFlameAttack ());
		isFinished = false;
	}

	public override void otherCollisionEnter(GameObject enemy,Vector3 point){
		if(enemy.layer.Equals(LayerMask.NameToLayer("Player")) && !hasHurtPlayer){
			GameManager.playerController.getHurt(damage,point);
			hasHurtPlayer = true;
		}
	}

	IEnumerator doFlameAttack(){
		hasHurtPlayer = false;
		iaParent.getIAAnimator().SetTrigger("isChargingFlameAttack");
		yield return new WaitForSeconds (1.5f);
		iaParent.getIAAnimator().SetTrigger("isDoingFlameAttack");
		GetComponentInChildren<Collider> ().enabled = true;
		GetComponent<ParticleSystem> ().Play ();

		yield return new WaitForSeconds (2.5f);
		GetComponentInChildren<Collider> ().enabled = false;
		yield return null;
		isFinished = true;
		GetComponent<ParticleSystem> ().Stop ();
	}
	
	public override void informParent(GameObject parentObject){
		iaParent = parentObject.GetComponent<IAControllerHydra> ();
		
		transform.parent = iaParent.headPosition.transform;
		transform.rotation = iaParent.headPosition.transform.rotation;
		transform.position = iaParent.headPosition.transform.position;
	}
}
