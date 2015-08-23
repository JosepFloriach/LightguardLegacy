using UnityEngine;
using System.Collections;

public class JumpingAttack : Attack {

	private GameObject parent;
	private float direction = 0f;

	public override void initialize(){
		attackType = AttackType.JumpingAttack;
	}
	public override void startAttack(){
		bool isRight = parent.GetComponent<IAController>().getIsLookingRight();
		if(isRight){
			direction = 1f;
		}else{
			direction = -1f;
		}
		isFinished = false;
		
		StartCoroutine ("jump");
	}

	private IEnumerator jump() {
		isFinished = false;
		parent.GetComponent<IAController> ().Jump ();
		isFinished = true;
		yield return null;
	}

	public override void informParent(GameObject parentObject){
		transform.parent = parentObject.transform;
		transform.rotation = parentObject.transform.rotation;
		transform.position = parentObject.GetComponent<Rigidbody>().worldCenterOfMass + (parentObject.transform.forward*parentObject.GetComponent<WalkOnMultiplePaths>().centerToExtremesDistance*1.5f);
		parent = parentObject;
	}
}
