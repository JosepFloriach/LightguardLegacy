using UnityEngine;
using System.Collections;

public class NeckController : MonoBehaviour {
	
	public float maxDegreeRotation = 60f;

	private bool rotatingNeck = false;
	private bool lockedAngle = false;
	Vector3 originalAngle;
	Vector3 objectivePosition;

	// Update is called once per frame
	void Update () {
	
	}

	public void setObjectivePosition(Vector3 objectivePos){
		rotatingNeck = true;
		objectivePosition = objectivePos;
	}

	public void stopRotating(){
		rotatingNeck = false;
	}

	public void LockAngle(){
		lockedAngle = true;
	}

	public void UnlockAngle(){
		lockedAngle = false;
	}

	void LateUpdate(){

		if (rotatingNeck) {
			if(!lockedAngle){
				Vector3 directionHealthFirefly = objectivePosition - transform.position;
				Vector3 directionPlayer = GameManager.player.transform.forward;
				float newAngle = Util.getAngleFromVectorAToB (directionPlayer, directionHealthFirefly);
				float zAngle = 0f;
				if (newAngle > 90f || newAngle < -90f) {
						if (newAngle < 0f) {
								newAngle = 360f - newAngle;
						}
						newAngle = -(90f - (newAngle - 90f));
						zAngle = 180f;

				}
				if (newAngle > maxDegreeRotation) {
						newAngle = maxDegreeRotation;
				}
				if (newAngle < -maxDegreeRotation) {
						newAngle = -maxDegreeRotation;
				}
				originalAngle = new Vector3 (newAngle, 0f, zAngle);
			}
			transform.eulerAngles += originalAngle;
		}

	}
}
