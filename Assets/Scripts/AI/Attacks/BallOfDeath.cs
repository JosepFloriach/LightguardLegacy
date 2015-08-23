using UnityEngine;
using System.Collections;

public class BallOfDeath : MonoBehaviour {

	public float timeToDespawn = 5f;
	private float direction;
	private int damage;
	private int actualDamage;
	float timer = 0f;
	Vector3 originalScale;
	private bool activated = false;

	public void setDirectionAndDamage(float direction,int damage,GameObject parent){
		this.damage = damage;
		actualDamage = damage;
		this.direction = direction;
		GetComponent<CharacterController> ().setOriginalOrientation ();
		GetComponent<CharacterController> ().Move (direction);
		timer = 0f;
		transform.parent = parent.transform.parent;
		originalScale = parent.transform.localScale;
		transform.localScale = originalScale;
		activated = true;
	}

	void Update(){
		timer += Time.deltaTime;
		float invertRatio = 1f - (timer / timeToDespawn);
		actualDamage = (int)(damage * invertRatio);
		if (actualDamage < 1) {
			actualDamage = 1;
		}
		transform.localScale = originalScale * invertRatio;
		if (invertRatio < 0.3f) {
			GetComponent<ParticleSystem>().Stop();
		}
		if (invertRatio < 0.1f) {
			Destroy(gameObject);
		}
	}

	void OnCollisionEnter(Collision collision){
		if (collision.gameObject.layer.Equals (LayerMask.NameToLayer("Player")) && activated) {
			GameManager.playerController.getHurt(actualDamage,collision.contacts[0].point);
			//Should be soft destroyed, not directly
			Destroy(gameObject);
		}
	}

}
