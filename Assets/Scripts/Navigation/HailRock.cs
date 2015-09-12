using UnityEngine;
using System.Collections;

public class HailRock : MonoBehaviour {

	bool isDisappearing = false;

	void OnCollisionEnter(Collision collision){
		if (collision.gameObject.tag == "Planet" && !isDisappearing) {
			StartCoroutine(disappearCorroutine());
		}
	}

	IEnumerator disappearCorroutine(){
		isDisappearing = true;
		GetComponentInChildren<DamageOnCollide>().enabled = false;
		GetComponentInChildren<OutlineChanging> ().changeColorOverTime (Color.clear, 1f);
		GetComponent<Collider>().enabled = false;
		GetComponent<GravityBody> ().enabled = false;
		GetComponent<Rigidbody> ().isKinematic = true;
		GetComponent<Rigidbody> ().velocity = Vector3.zero;
		yield return new WaitForSeconds (0.7f);
		Destroy (gameObject);
	}
}
