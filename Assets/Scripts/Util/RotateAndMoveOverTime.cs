using UnityEngine;
using System.Collections;

public class RotateAndMoveOverTime : MonoBehaviour {

	public Vector3 objectiveLocalPosition;
	public Vector3 objectiveLocalRotation;

	public void changeOverTime(float timeToChange){
		StartCoroutine (changePosition (timeToChange));
	}

	private IEnumerator changePosition(float time){
		Vector3 originalPosition = transform.localPosition;
		Vector3 originalRotation = transform.localEulerAngles;
		float timer = 0f;
		while(timer<time){
			timer+=Time.deltaTime;
			float proportion = timer/time;
			transform.localPosition = Vector3.Lerp(originalPosition,objectiveLocalPosition,proportion);
			transform.localEulerAngles = Vector3.Lerp(originalRotation,objectiveLocalRotation,proportion);
			yield return null;
		}
		transform.localPosition = objectiveLocalPosition;
		transform.localEulerAngles = objectiveLocalRotation;
	}
}
