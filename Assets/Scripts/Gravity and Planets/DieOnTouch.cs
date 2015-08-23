using UnityEngine;
using System.Collections;

public class DieOnTouch : MonoBehaviour {

	public bool collision = true;
	public bool trigger = true;

	void OnCollisionEnter (Collision collider) {
		if(collider.gameObject.tag == "Player" && collision && isActiveAndEnabled){
			GameManager.playerController.kill();
		}
	}

	void OnTriggerEnter (Collider collider) {
		if(collider.gameObject.tag == "Player" && trigger && isActiveAndEnabled){
			GameManager.playerController.kill();
		}
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
