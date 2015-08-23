using UnityEngine;
using System.Collections;

public class SwitchCameraMode : MonoBehaviour {

	Vector3 camUp;
	CameraFollowingPlayer cameraController;
	// Use this for initialization
	void Start () {
		camUp = this.transform.up;
		cameraController = GameManager.mainCamera.GetComponent<CameraFollowingPlayer>();
		cameraController.setStaticUp(camUp);

	}
		
	void OnTriggerEnter(Collider other) {
	if (other.tag == "Player") {
		
		cameraController.regularMode = true;
	}
	}
	
	void OnTriggerExit(Collider other) {
		if (other.tag == "Player") {
	
			cameraController.regularMode = false;
		}
		
	}
	
	
}
