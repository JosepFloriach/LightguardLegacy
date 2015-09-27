using UnityEngine;
using System.Collections;

public class HideOnEnter : MonoBehaviour {
	MeshRenderer renderer;

	void Awake(){
		renderer = GetComponent<MeshRenderer> ();
	}

	void OnTriggerEnter(Collider c){
		if (c.tag == "Player") {
			renderer.enabled = false;
		}
	}

    void OnTriggerExit(Collider c){
		if (c.tag == "Player") {
			renderer.enabled = true;
		}
	}
}
