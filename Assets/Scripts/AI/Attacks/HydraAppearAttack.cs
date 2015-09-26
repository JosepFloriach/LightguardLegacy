using UnityEngine;
using System.Collections;

public class HydraAppearAttack : Attack {
	
	private IAControllerHydra iaParent;
	
	private bool hasHurtPlayer = false;
	
	public override void initialize(){
		attackType = AttackType.HydraAppearAttack;
	}
	
	public override void startAttack(){
		StartCoroutine (doAppearAttack ());
		isFinished = false;
	}
	
	public override void otherCollisionEnter(GameObject enemy,Vector3 point){
		if(enemy.GetComponent<HydraPlatform>()!=null){
			enemy.GetComponent<HydraPlatform>().hasBeenTouched();
			if(iaParent.platformDestroyed !=null && !enemy.Equals(iaParent.platformDestroyed)){
				iaParent.platformDestroyed.GetComponent<HydraPlatform>().repositionPlatform();
			}
			iaParent.platformDestroyed = enemy;
		}
	}
	
	IEnumerator doAppearAttack(){
		iaParent.GetComponent<IAControllerHydra> ().enableHydraHitting (false);
		hasHurtPlayer = false; 
		iaParent.mesh.GetComponent<SkinnedMeshRenderer> ().enabled = false;
		yield return new WaitForSeconds (2f);
		GetComponentInChildren<ParticleSystem> ().Play ();
		yield return new WaitForSeconds (4f);
		iaParent.GetComponent<IAControllerHydra> ().enableHydraHitting (true);
		GetComponentInChildren<ParticleSystem> ().Stop ();
		iaParent.mesh.GetComponent<SkinnedMeshRenderer> ().enabled = true;
		iaParent.getIAAnimator().SetBool("isHidden",false);
		iaParent.GetComponent<CharacterController> ().LookLeftOrRight (iaParent.getPlayerDirection ());
		yield return new WaitForSeconds (5f);
		isFinished = true;
	}
	
	public override void informParent(GameObject parentObject){
		iaParent = parentObject.GetComponent<IAControllerHydra> ();
		
		transform.parent = iaParent.transform;
		transform.rotation = iaParent.hydraBody.transform.rotation;
		transform.position = iaParent.hydraBody.transform.position;
	}

}
