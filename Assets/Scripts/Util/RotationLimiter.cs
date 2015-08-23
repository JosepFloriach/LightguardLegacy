using UnityEngine;
using System.Collections;

public class RotationLimiter : MonoBehaviour {

	public float rotationLimitPerSecond;
	private Vector3 eulerAngles;
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void LateUpdate () {
		if(eulerAngles==null){
			eulerAngles = transform.localEulerAngles;
		}else{
			Vector3 newAngles = transform.localEulerAngles;
			float angularSpeed = (Vector3.Angle(newAngles,eulerAngles)/Time.deltaTime);
			if(angularSpeed>rotationLimitPerSecond){
				newAngles = Vector3.Lerp(eulerAngles,newAngles,rotationLimitPerSecond/angularSpeed);
				transform.localEulerAngles = newAngles;
			}
			eulerAngles = newAngles;
		}
	}
}
