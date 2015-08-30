using UnityEngine;
using System.Collections;

public class AttackCollider : MonoBehaviour {

	public GameObject attack;
	public GameObject debug;
	private bool wasEnabled = false;

	void OnTriggerEnter(Collider other) {
		if(attack!=null){
			if(other.tag.Equals("Enemy")){
				attack.GetComponent<Attack>().enemyCollisionEnter(other.gameObject,other.ClosestPointOnBounds(transform.position));
			}else{
				attack.GetComponent<Attack>().otherCollisionEnter(other.gameObject,other.ClosestPointOnBounds(transform.position));
			}
		}
		if (debug != null) {
			debug.SetActive (true);
			wasEnabled = false;
		}

	}

	void OnTriggerExit(Collider other) {
		if(attack!=null){
			if(other.tag.Equals("Enemy")){
				attack.GetComponent<Attack>().enemyCollisionExit(other.gameObject);
			}else{
				attack.GetComponent<Attack>().otherCollisionExit(other.gameObject);
			}
		}
	}

	void OnCollisionEnter(Collision other) {
		if(attack!=null){
			if(other.gameObject.tag.Equals("Enemy")){
				attack.GetComponent<Attack>().enemyCollisionEnter(other.gameObject,other.contacts[0].point);
			}else{
				attack.GetComponent<Attack>().otherCollisionEnter(other.gameObject,other.contacts[0].point);
				attack.GetComponent<Attack>().otherCollisionEnter(other);
			}
		}
	}
	
	void OnCollisionExit(Collision other) {
		if(attack!=null){
			if(other.gameObject.tag.Equals("Enemy")){
				attack.GetComponent<Attack>().enemyCollisionExit(other.gameObject);
			}else{
				attack.GetComponent<Attack>().otherCollisionExit(other.gameObject);
			}
		}
	}

	void Update(){
		if (debug != null) {
			if (GetComponent<Collider> ().enabled) {
				if(!wasEnabled){
					if(debug.GetComponent<ParticleSystem>()!=null){
						debug.GetComponent<ParticleSystem>().Play ();
					}
				}
				wasEnabled = true;
			} else {
				wasEnabled = false;
				//debug.SetActive (false);
			}
		}
	}
}
