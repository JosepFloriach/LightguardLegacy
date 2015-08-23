using UnityEngine;
using System.Collections;

public class DamageOnCollide : MonoBehaviour {

	public int damage;
	public float cooldown = 1f;
	private float cooldownTimer = 0f;
	void OnCollisionStay(Collision collision){
		if(collision.gameObject.layer.Equals(LayerMask.NameToLayer("Player")) && cooldownTimer>cooldown){
			cooldownTimer = 0f;
			GameManager.playerController.getHurt(damage,collision.contacts[0].point);
		}
	}

	void OnTriggerStay(Collider collider){
		if(collider.gameObject.layer.Equals(LayerMask.NameToLayer("Player")) && cooldownTimer>cooldown){
			cooldownTimer = 0f;
			GameManager.playerController.getHurt(damage,collider.ClosestPointOnBounds(transform.position));
		}
	}
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
			cooldownTimer += Time.deltaTime;
	}
}
