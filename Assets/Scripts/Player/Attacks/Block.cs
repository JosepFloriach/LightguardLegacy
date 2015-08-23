using UnityEngine;
using System.Collections;

public class Block : MonoBehaviour {
	public float initialRadius;
	public float radiusLostOnDiff;
	public float radiusLostOnHit;
	public float blockCooldown = 1.0f;

	private float currentRadius;
	private bool cooldownFinished = true;
	private bool isDoingBlock = false;
	private GameObject block;

	public bool getIsDoingBlock() {
		return isDoingBlock;
	}

	public bool isCooldownFinished() {
		return cooldownFinished;
	}

	void Start(){
		GetComponent<Renderer> ().enabled = false;
	}
	IEnumerator doBlock(){
		GetComponent<Renderer> ().enabled = true;
		isDoingBlock = true;
		cooldownFinished = false;
		for (float radius = initialRadius; radius >=0;){
			float previousRadius = gameObject.transform.localScale.x;
			if (GameManager.player.GetComponent<PlayerController>().isHit () ){
				radius = previousRadius - (radiusLostOnHit*Time.deltaTime);
			} else {
				radius = previousRadius - (radiusLostOnDiff*Time.deltaTime);
			}
			if (radius < 0) {
				this.enabled = false;
			} else {
				gameObject.transform.localScale = new Vector3(radius,radius,radius);
			}
			yield return null;
		}
		isDoingBlock = false;
		yield return new WaitForSeconds (blockCooldown);
		cooldownFinished = true;
	}

	public void StartBlock() {

		this.transform.position = GameManager.player.GetComponent<Rigidbody> ().worldCenterOfMass;
		this.transform.parent = GameManager.player.transform;

		this.transform.localScale = new Vector3 (initialRadius, initialRadius, initialRadius);
		this.enabled = true;
		StartCoroutine ("doBlock");
	}
}
