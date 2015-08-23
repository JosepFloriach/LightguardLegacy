using UnityEngine;
using System.Collections;

public class DestroyRocksOnHit : MonoBehaviour {

	public GameObject particles;
	public int numberOfHitsNecessary = 3;
	public float cooldownHit = 0.5f;
	private int hitsMade = 0;
	private float lastHit;
	void Start(){
		lastHit = Time.time;
	}
	void OnTriggerEnter(Collider other) {
		if(other.gameObject.layer == LayerMask.NameToLayer("PlayerWeapon")){
			if(((Time.time - lastHit)>cooldownHit)){
				lastHit = Time.time;
				hitsMade++;
				particles.GetComponent<ParticleSystem>().Play();
				if(hitsMade>=numberOfHitsNecessary){
					GetComponentInParent<FirstPlanetBlockPathRocks>().informRocksHit();
				}
			}
		}
	}
}
