using UnityEngine;
using System.Collections;

public class GalaxyCameraZone : MonoBehaviour {


	public GameObject cameraPositionOnGalaxyOverview;

	void Awake(){
	}

	void OnTriggerEnter(Collider collider){
		if (collider.tag == "Player") {
			GameManager.actualGalaxy = this;
		}
	}



}
